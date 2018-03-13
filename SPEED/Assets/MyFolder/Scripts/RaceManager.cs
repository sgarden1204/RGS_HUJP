using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class RaceManager : PunBehaviour {

    public static RaceManager instance;

    public SmoothFollow cameraControl;

    public Text lapText;
    public Text rankingText;
    public Text speedText;
    public Text countdownText;
    public Text ammoText;

    public Image weaponImg;

    public Transform playerBoardpanel;
    public Transform raceEndPanel;

    [HideInInspector]
    public RaceController race;

    private List<RaceController> raceControllers = new List<RaceController>();
    private List<PlayerBoardGUI> playersPanel = new List<PlayerBoardGUI>();
    private List<PlayerBoardGUI> endPanel = new List<PlayerBoardGUI>();

    private int loadedPlayers = 0;

    public float raceTime = 0;
    public double startTimestamp = 0;

    private RaceState state = RaceState.WAIT_ANOTHERPLAYER;

    public Transform[] spawnPoints;

	void Start () {

        instance = this;

        CreateCar();
        photonView.RPC("ConfirmLoad", PhotonTargets.All);

        int count = raceEndPanel.childCount;

        for (int i = 0; i < count; i++)
        {
            endPanel.Add(raceEndPanel.GetChild(i).GetComponent<PlayerBoardGUI>());
        }
        count = playerBoardpanel.childCount;

        for (int i = 0; i < count; i++)
        {
            playersPanel.Add(playerBoardpanel.GetChild(i).GetComponent<PlayerBoardGUI>());
        }
	}
	
	void Update () {
        raceTime += Time.deltaTime;

        SortCars();

        switch (state)
        {
            case RaceState.WAIT_ANOTHERPLAYER:

                if(PhotonNetwork.isMasterClient)
                {
                    CheckCountdown();
                }

                break;

            case RaceState.COUNTDOWN:

                countdownText.text = "" + (1 + (int)(startTimestamp - PhotonNetwork.time));
                if(PhotonNetwork.time >= startTimestamp)
                {
                    StartRace();
                }

                break;

            case RaceState.RACING:

                if (raceTime > 3)
                {
                    countdownText.text = "";
                }

                else
                {
                    countdownText.text = "GO!";
                }

                break;

            case RaceState.FINISHED:
                UpdatePlayerPanel(endPanel);
                break;

        }

        if (race.state == RaceState.FINISHED)
        {
            state = RaceState.FINISHED;

            raceEndPanel.gameObject.SetActive(true);
        }

        UpdatePlayerPanel(playersPanel);
    }

    private void SortCars()
    {
        raceControllers.Sort();
        int ranking = 1;

        foreach(RaceController c in raceControllers)
        {
            c.currentRanking = ranking;
            ranking++;
        }
    }

    private void UpdatePlayerPanel(List<PlayerBoardGUI> list)
    {
        foreach(RaceController c in raceControllers)
        {
            list[c.currentRanking - 1].SetCar(c);
        }
    }

    [PunRPC]
    public void ConfirmLoad()
    {
        loadedPlayers++;
    }

    private void CheckCountdown()
    {
        bool takingTooLong = raceTime >= 5;
        bool finishedLoading = loadedPlayers == PhotonNetwork.playerList.Length;
        if(takingTooLong || finishedLoading)
        {
            photonView.RPC("StartCountdown", PhotonTargets.All, PhotonNetwork.time + 4);
        }
    }

    private void CreateCar()
    {
        int pos = (int)PhotonNetwork.player.CustomProperties["spawn"];
        int carNumber = (int)PhotonNetwork.player.CustomProperties["car"];
        Transform spawn = spawnPoints[pos];

        GameObject car = PhotonNetwork.Instantiate("Car" + carNumber, spawn.position, spawn.rotation, 0);

        race = car.GetComponent<RaceController>();

        //이게 되나? 적용된 스크립트 프리팹
        car.GetComponent<RaceController>().currentRanking = pos + 1;

        cameraControl.target = car.transform;

        car.GetComponent<CarGUI>().enabled = true;
        car.GetComponent<CarGUI>().lapGUI = lapText;
        car.GetComponent<CarGUI>().rankingGUI = rankingText;
        car.GetComponent<CarGUI>().speedGUI = speedText;
        car.GetComponent<CarGUI>().messagesGUI = countdownText;

        //car.GetComponent<WeaponManager>().weaponImageGUI = weaponImageGUI;
        //car.GetComponent<WeaponManager>().weaponTextGUI = weaponTextGUI;

        //car.GetComponent<CarGUI>().SendColor(Color.black);
    }

    public void StartRace()
    {
        state = RaceState.RACING;

        GameObject[] cars = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject go in cars)
        {
            raceControllers.Add(go.GetComponent<RaceController>());
            go.GetComponent<CarInput>().enabled = true;
            go.GetComponent<RaceController>().currentCheckPoint = GameObject.Find("Checkpoint1").GetComponent<CheckPoint>();
            go.GetComponent<RaceController>().state = RaceState.RACING;
        }
    }

    public void StartCountdown(double startTimestamp)
    {
        state = RaceState.COUNTDOWN;

        this.startTimestamp = startTimestamp;
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        RaceController toRemove = null;

        foreach(RaceController rc in raceControllers)
        {
            if(rc.photonView.owner == null)
            {
                toRemove = rc;
            }
        }

        raceControllers.Remove(toRemove);
    }

    public void ResetToMenu()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MenuTest");
    }
}
