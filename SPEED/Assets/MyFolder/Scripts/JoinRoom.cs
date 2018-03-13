using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoom : MonoBehaviour {

    public string RoomName { get; set; }
    //룸네임

    public void Join()
    {
        PhotonNetwork.JoinRoom(RoomName);
        //포톤 네트워크 룸 이름으로 방 입장
    }
}
