using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RaceState
{
    WAIT_ANOTHERPLAYER, COUNTDOWN, RACING, FINISHED
}
// WAIT_ANOTHERPLAYER: 다른 플레이어들이 연결될때 까지 기다리는 상태
// COUNTDOWN: 시작하기전 카운트 다운 상태
// RACING: 경기가 진행중인 상태
// FINISHED: 경기 종료 상태