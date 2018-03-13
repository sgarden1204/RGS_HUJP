using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
//유니티 스탠다드에셋 using

public class CarInput : MonoBehaviour {

    private CarController car;
    //유니티 스탠다드 에셋

    //Steer: 조종하다
    public float steer { get; set; }

    //Accelerator: 가속장치
    public float accel { get; set; }

    //Handbreak = Sidebreak;
    public float handbreak { get; set; }

    public bool controlable = false;

	void Start () {
        car = GetComponent<CarController>();
	}

    private void FixedUpdate()
    {
        if(controlable)
        {
            GetInput();
        }

        ApplyInput();
    }

    protected virtual void GetInput()
    {
        steer = Input.GetAxis("Horizontal");
        accel = Input.GetAxis("Vertical");
        handbreak = Input.GetAxis("Jump");
        //혹시 여기에 브레이크?
    }

    protected virtual void ApplyInput()
    {
        car.Move(steer, accel, accel, handbreak);
        //가운데 세번째는 footbrake
    }

    //Flip: 확 뒤집다
    //자동차가 확 뒤집혔다 = 전복
    private bool Flipped()
    {
        //sqrMagnitude: 거리제곱 반환
        //transfrom.up : y값 * dot product * vector3.down(0,-1,0) 
        if(GetComponent<Rigidbody>().velocity.sqrMagnitude < 0.01 && Vector3.Dot(transform.up, Vector3.down) > 0)
        {
            return true;
        }

        return false;
    }

    //Unflip: 전복된걸 다시 뒤집다
    private void Unflip()
    {
        Vector3 angles = transform.eulerAngles;
        //transfrom.eulerAngles 이 오브젝트의 오일러각을 가져옴
        //eulerAngle이란 일반적으로 xyz좌표계, 왼손 좌표계
        angles.z = 0;
        transform.eulerAngles = angles;
    }

}
