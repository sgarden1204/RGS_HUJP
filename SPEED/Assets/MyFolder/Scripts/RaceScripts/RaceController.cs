using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//photon.MonoBehaviour, IComparable<RaceController> 상속
public class RaceController : Photon.MonoBehaviour, IComparable<RaceController>
{
    public CheckPoint currentCheckPoint;

    public int lapsCompleted = 0;
    public int totalLaps = 3;
    public int currentRanking = 4;
    public int checkpointPassed = 0;

    public float distanceTraveled = 0;
    //distanceTraveled: 주행거리
    public float raceTime = 0;

    public RaceState state = RaceState.COUNTDOWN;
    //초기 RaceState state는 COUNTDOWN이다


    void Start () {
        //첫 체크포인트
        currentCheckPoint = GameObject.Find("Checkpoint1").GetComponent<CheckPoint>();
	}
	
	void Update () {
        UpdateDistanceTraveled();
        if(state == RaceState.RACING)
        {
            raceTime += Time.deltaTime;
        }
        else if(state == RaceState.FINISHED && PhotonNetwork.isMasterClient)
        {
            photonView.RPC("CorrectedTimeAndRacking", PhotonTargets.All, raceTime, currentRanking);
            //PhotonView.RPC("CorrectedTimeAndRanking", PhotonTargets.All, raceTime, currentRanking);
        }
	}

    public bool WrongWay
    {
        get
        {
            //correct: 정확한, 올바른
            Vector3 correctDirection = currentCheckPoint.transform.position - transform.position;
            //방향 = 현재 체크포인트의 위치 - this 위치

            //Dot product
            //product: 곱셈
            //내적: 두 벡터의 정사형의 곱
            //정사형:어떤 물체를 수직으로 비추었을때 생기는 그림자
            //forward(0,0,1) 즉 z방향
            if(Vector3.Dot(transform.forward, correctDirection) < 0)
            {
                return true;
            }

            return false;
        }
    }

    void UpdateDistanceTraveled()
    {
        Vector3 distance = transform.position - currentCheckPoint.transform.position;
        //magnitude: 규모
        //magnitude == distance 거리비교 함수
        distanceTraveled = 1000 * checkpointPassed + (1000 - distance.magnitude);
        //주행거리 = 1000 * 통과한 체크포인트의 수 + (1000 - 거리);
    }

    int IComparable<RaceController>.CompareTo(RaceController other)
    {
        if(state == RaceState.FINISHED)
        {
            //나는 들어왔는데 남의 차는 안들어왔다
            if (other.state != RaceState.FINISHED)
            {
                return -1;
            }

            // 현재 랭킹 < 다른차 랭킹
            else
            {
                return currentRanking < other.currentRanking ? -1 : 1;
            }
        }

        //내 주행거리가 > 남들 주행거리
        if (distanceTraveled > other.distanceTraveled)
            return -1;
        
        //내 주행거리 < 남들 주행거리
        else if (distanceTraveled < other.distanceTraveled)
            return 1;

        else
            return 0;

        // currentRanking(현재 랭킹) + return value(리턴 값)
    }

    void OnTriggerEnter(Collider other)
    {
        //other = 경기중인 차

        if(other.tag == "Bullet")
        {
            return;
        }

        CheckPoint checkPoint = other.GetComponent<CheckPoint>();
        //자동차의 현재 체크포인트를 받는다.

        if(checkPoint != currentCheckPoint)
        {

        }
        else //체크포인트가 맞다면
        {
            checkpointPassed++;

            //체크포인트가 finishline = true 라면
            if (checkPoint.finishline)
                lapsCompleted++;

            //완료한 랩이 마지막 랩이면
            if(lapsCompleted == totalLaps)
            {
                CarInput input = GetComponent<CarInput>();
                input.controlable = false;
                input.steer = 0;
                input.handbreak = 1;
                input.accel = 0;
                state = RaceState.FINISHED;
                //입력중지, 모든값 0, 레이스 상태 FINISHED
            }

            currentCheckPoint = checkPoint.next;
            //다음체크포인트
        }
    }

    [PunRPC]
    public void CorrectedTimeAndRanking(float time, int ranking)
    {
        raceTime = time;
        currentRanking = ranking;
        state = RaceState.FINISHED;
        //모든 RaceState 상태를 FINISHED로 바꾼다

        lapsCompleted--;
        //무한 충돌 방지

        enabled = false;
        //enabled = false 이 스크립트의 Update를 비활성
    }
}
