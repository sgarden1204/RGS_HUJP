using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine.UI;

#pragma warning disable 612, 618
//findchild warning 제거

public class MenuManager : PunBehaviour
{
    public Transform startButton;
    public Transform nickname_InputPanel;
    public Transform createAndJoinRoomPanel;
    public Transform carAndTrackPanel;
    public Transform[] trackArrowButton;
    public Transform[] playerSet;

    public Sprite emptyCarSprite;
    public Sprite[] carSprite;
    public Sprite[] trackSprite;

    private List<JoinRoom> rooms = new List<JoinRoom>();

    public InputField inputNickname;

    public Text messages;

    public Image trackImg;

    private int carIndex = 0;
    private int trackIndex = 0;

    void Start()
    {
        //로드 신으로 불려질 경우 Start()함수 호출
        //프로세스가 종료되기까지 포톤 네트워크는 연결상태를 지속한다
        //따라서 로드 신으로 불려오면 닉네임 인풋패널을 활성화 하지 않고 리턴
        if (PhotonNetwork.connected)
        {
            createAndJoinRoomPanel.gameObject.SetActive(true);
            return;
        }
        messages.text = "";
        nickname_InputPanel.gameObject.SetActive(true);

        //닉네임 패널 활성 & 포톤네트워크 연결시 활성
    }

    //룸 리스트 업데이트를 받는다
    public override void OnReceivedRoomListUpdate()
    {

        foreach (JoinRoom RoomButton in rooms)
        {
            Destroy(RoomButton.gameObject);
        }

        rooms.Clear();
        //룸 초기화

        int i = 0;

        foreach (RoomInfo room in PhotonNetwork.GetRoomList())
        {
            //룸 정보를 받아오지 못한다면
            if (!room.IsOpen)
                continue;

            GameObject buttonPrefab = Resources.Load<GameObject>("Prefab/RoomButton");
            //GameObject buttonPrefab = Resources.Load<GameObject>("MyFolder/Prefab/RoomButton");
            Debug.Log(buttonPrefab);
            GameObject roomButton = Instantiate<GameObject>(buttonPrefab);
            roomButton.GetComponent<JoinRoom>().RoomName = room.Name;
            //게임 오브젝트 = 프리팹화 된 UI 를 넣고 생성한다

            string info = room.Name.Trim() + " (" + room.PlayerCount + "/" + room.MaxPlayers + ")";
            //Trim() : 다듬다
            roomButton.GetComponentInChildren<Text>().text = info;
            //룸의 이름을 설정

            rooms.Add(roomButton.GetComponent<JoinRoom>());
            //Room List에 추가
            roomButton.transform.SetParent(createAndJoinRoomPanel, false);
            //레이스 패널에 하위 항목으로 추가 /월드 포지션 x

            i++;
            roomButton.transform.position = new Vector3((150.0f * i), 200.0f);
            //roomButton.transform.position.Set(0, i * 120, 0);
            //룸버튼 위치 설정
            //position.set이 안먹힌다 따라서 position을 직접 대입하는식으로 수정
            //800 * 600 해상도에 맞춰서 설정했다
        }
    }

    //닉네임 임력 함수
    //OK Button 에서 호출
    public void InputNickname()
    {
        PhotonNetwork.player.NickName = inputNickname.text;
        PhotonNetwork.ConnectUsingSettings("v1.0");
        messages.text = "CONNECTING...";
    }

     // 조인 로비
    public override void OnConnectedToMaster()
    {
        Debug.Log("조인 로비는 언제 호출됩니까?");
        PhotonNetwork.JoinLobby();
        messages.text = "ENTER LOBBY...";
    }


    public override void OnJoinedLobby()
    {
        Debug.Log("온 조인 로비 함수 OnConnectedToMaster호출시 호출?");
        nickname_InputPanel.gameObject.SetActive(false);
        messages.gameObject.SetActive(false);
        //닉네임 패널과 하단 출력 메시지를 없앤다

        createAndJoinRoomPanel.gameObject.SetActive(true);
        //방 UI 활성화
    }

    //CreateButton에서 호출
    public void CreateGame()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        //룸 옵션 -> 최대 인원 4명까지

        PhotonNetwork.CreateRoom(PhotonNetwork.player.NickName, options, TypedLobby.Default);
        //룸 생성
        foreach (Transform tb in trackArrowButton)
        {
            tb.gameObject.SetActive(true);
        }
        //트랙 골르는 레프트 라이트 애로우 버튼 생성 활성화
    }


    //방생성 실패시 만들어진다
    //방생성 실패하는 이유는 중복된 ID를 허용했기 때문이다
    //따라서 PlayerName에 0을 붙인다.
    public override void OnPhotonCreateRoomFailed(object[] codeMessage)
    {
        if ((short)codeMessage[0] == ErrorCode.GameIdAlreadyExists)
        {
            PhotonNetwork.playerName += "0";
            CreateGame();
        }
    }

    //아이디가 중복인지 체크하는 함수
    //밑에 OnJoinedRoom에서 사용
    private bool checkSameNameOnPlayersList(string name)
    {
        foreach (PhotonPlayer pp in PhotonNetwork.otherPlayers)
        {
            if (pp.NickName.Equals(name))
            {
                return true;
            }
        }

        return false;
    }


    public override void OnJoinedRoom()
    {

        if (checkSameNameOnPlayersList(PhotonNetwork.playerName))
        {
            string newName = PhotonNetwork.playerName;
            do
            {
                newName += "0";
            } while (checkSameNameOnPlayersList(newName));

            PhotonNetwork.playerName = newName;
        }
        //같은이름 체크

        carAndTrackPanel.gameObject.SetActive(true);
        createAndJoinRoomPanel.gameObject.SetActive(false);
        //방 패널 오픈
        SetCustomProperties(PhotonNetwork.player, 0, PhotonNetwork.playerList.Length - 1);
        // Properties : 재산, 소유
    }

    //CreateRoom호출시 호출
    public override void OnCreatedRoom()
    {
        startButton.gameObject.SetActive(true);
        SetCustomProperties(PhotonNetwork.player, 0, PhotonNetwork.playerList.Length - 1);
    }

    //JoinRoom호출시 호출
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.isMasterClient)
        {
            SetCustomProperties(newPlayer, 0, PhotonNetwork.playerList.Length - 1);
            photonView.RPC("UpdateTrack", PhotonTargets.All, trackIndex);
        }
    }

    //Scene 변경시 호출
    public override void OnPhotonPlayerDisconnected(PhotonPlayer disconnetedPlayer)
    {
        if (PhotonNetwork.isMasterClient)
        {
            int playerIndex = 0;
            foreach (PhotonPlayer p in PhotonNetwork.playerList)
            {
                SetCustomProperties(p, (int)p.CustomProperties["car"], playerIndex++);
            }
        }
    }

    //포톤 플레이어 특징 바꾸는 함수
    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        UpdatePlayerList();
    }

    //플레이어 리스트 업데이트
    public void UpdatePlayerList()
    {
        Debug.Log("updating");
        ClearPlayersGUI();
        int playerIndex = 0;

        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            Transform playerMenu = playerSet[playerIndex++];

            //플레이어 라면
            if (p == PhotonNetwork.player)
            {
                playerMenu.FindChild("LeftButton").gameObject.SetActive(true);
                playerMenu.FindChild("RightButton").gameObject.SetActive(true);
            }
            //Contain : 포함하다
            if (p.CustomProperties.ContainsKey("car"))
            {
                playerMenu.FindChild("PlayerCarImg").GetComponent<Image>().sprite = carSprite[(int)p.CustomProperties["car"]];
                playerMenu.FindChild("PlayerNickName").GetComponent<Text>().text = p.NickName.Trim();
            }
        }
    }

    private void ClearPlayersGUI()
    {
        //화살표 2개와 비어있는차 그리고 닉네임을 지운다
        foreach (Transform t in playerSet)
        {

            t.FindChild("PlayerCarImg").GetComponent<Image>().sprite = emptyCarSprite;
            t.FindChild("PlayerNickName").GetComponent<Text>().text = "";
            t.FindChild("LeftButton").gameObject.SetActive(false);
            t.FindChild("RightButton").gameObject.SetActive(false);
        }
    }

    //다음차
    public void NextCar()
    {
        carIndex = (carIndex + 1) % carSprite.Length;
        SetCustomProperties(PhotonNetwork.player, carIndex, (int)PhotonNetwork.player.CustomProperties["spawn"]);
    }

    //이전차
    public void PreviousCar()
    {
        carIndex--;
        if (carIndex < 0)
            carIndex = carSprite.Length - 1;
        SetCustomProperties(PhotonNetwork.player, carIndex, (int)PhotonNetwork.player.CustomProperties["spawn"]);
    }

    //다음 트랙
    public void NextTrack()
    {
        trackIndex = (trackIndex + 1) % trackSprite.Length;
        photonView.RPC("UpdateTrack", PhotonTargets.All, trackIndex);
    }

    //이전 트랙
    public void PreviousTrack()
    {
        trackIndex--;
        if (trackIndex < 0)
            trackIndex = trackSprite.Length - 1;
        photonView.RPC("UpdateTrack", PhotonTargets.All, trackIndex);
    }


    //PunRPC 모든 유저에게 Remote Procedure Call 원격 절차호출 
    [PunRPC]
    public void UpdateTrack(int index)
    {
        trackIndex = index;
        trackImg.sprite = trackSprite[trackIndex];
    }

    //StartButton 에서 호출
    public void CallLoadRace()
    {
        PhotonNetwork.room.IsOpen = false;
        photonView.RPC("LoadRace", PhotonTargets.All);
    }


    //로드 레벨 Race + 숫자를 통해 불러온다.
    [PunRPC]
    public void LoadRace()
    {
        PhotonNetwork.LoadLevel("Track" + (trackIndex + 1));
    }

    
    //해쉬 테이블을 이용 customProperties 설정
    private void SetCustomProperties(PhotonPlayer player, int car, int position)
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() { { "spawn", position }, { "car", car } };
        player.SetCustomProperties(customProperties);
    }

    //BackButton 에서 호출
    public void ResetToMenu()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Main");
    }

}

#pragma warning restore 612, 618
//#pragma warning restore CS0618