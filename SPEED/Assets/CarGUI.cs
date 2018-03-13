using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
//using 스탠다드 에셋

//Photon.모노
public class CarGUI : Photon.MonoBehaviour {

    public Text speedGUI;
    public Text rankingGUI;
    public Text lapGUI;
    public Text messagesGUI;

    private Sprite carIcon;

    private Rigidbody rigidBody;

    private RaceController raceController;

	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        raceController = GetComponent<RaceController>();
	}
	
	void Update () {
        //MPH = 2.223693629f
        //KPH = 3.6f;
        speedGUI.text = ((int)(rigidBody.velocity.magnitude * 3.1f)) + "";
        lapGUI.text = (raceController.lapsCompleted + 1) + "/" + raceController.totalLaps;
        rankingGUI.text = "" + raceController.currentRanking;

        //레이싱 중이면
        if(raceController.checkpointPassed > 0 && raceController.state != RaceState.FINISHED)
        {
            if (raceController.WrongWay)
            {
                messagesGUI.text = "Wrong Way!";
            }
            else
            {
                messagesGUI.text = "";
            }
        }
	}

    //Sprite를 반환
    public Sprite GetIcon()
    {
        if(carIcon == null)
        {
            //owner: 주인
            int carIndex = (int)photonView.owner.CustomProperties["car"];
            carIcon = Resources.Load<Sprite>("icon" + carIndex);
        }

        return carIcon;
    }
}
