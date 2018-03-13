using System;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using Photon;

public class NetworkCar : PunBehaviour
{
    private CarInput carInput;
	private Rigidbody rb;

	private Vector3 correctPlayerPos;
	private Quaternion correctPlayerRot;
	private Vector3 currentVelocity;
	private float updateTime = 0;

    private void Awake()
    {
		carInput = GetComponent<CarInput>();
		rb = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        //만약 Photonview가 내것이 아닐경우 -> 상대방 자동차 확인
		if (!photonView.isMine) {
			Vector3 projectedPosition = this.correctPlayerPos + currentVelocity * (Time.time - updateTime);
			transform.position = Vector3.Lerp (transform.position, projectedPosition, Time.deltaTime * 4);
			transform.rotation = Quaternion.Lerp (transform.rotation, this.correctPlayerRot, Time.deltaTime * 4);
		}
	}

    /// 포톤 직렬화
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
        //자동차의 정보를 전송
		if (stream.isWriting) {

			stream.SendNext((float)carInput.steer);
			stream.SendNext((float)carInput.accel);
			stream.SendNext((float)carInput.handbreak);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(rb.velocity);
		}
        //상대의 정보를 수신
		else {
			carInput.steer = (float)stream.ReceiveNext();
			carInput.accel = (float)stream.ReceiveNext();
			carInput.handbreak = (float)stream.ReceiveNext();
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
			currentVelocity = (Vector3)stream.ReceiveNext();
			updateTime = Time.time;
		} 
	}
}

