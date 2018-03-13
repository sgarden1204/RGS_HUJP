using UnityEngine;
using System.Collections;

//namespace 제거

public class SmoothFollow : MonoBehaviour
{

    //[SerializeField]
    //private Transform target;
    public Transform target;
    //타겟


	//[SerializeField]
	//private float distance = 10.0f;
    public float distance = 10.0f;
    //거리

	//[SerializeField]
	//private float height = 5.0f;
    public float height = 5.0f;
    //높이

    //[SerializeField]
    //private float rotationDamping;
    public float rotationDamping;
    //회전 감폭

    //[SerializeField]
    //private float heightDamping;
    public float heightDamping;
    //높이 감폭

    [AddComponentMenu("cameraControl/SmoothFollow")]
    //드래그앤 드랍이 아닌 코드상에서 적용시키는 방법

	void LateUpdate()
	{
		if (!target)
			return;
        //빠른 탈출

        //var 담기는 값에 따라 변화하는 변수
        //유니티에서는 var보다 명시적인 형식이 더 좋은것같다.
        //JAVA에 익숙하지 않아서 그런지도 모르겠다
		var wantedRotationAngle = target.eulerAngles.y;
		var wantedHeight = target.position.y + height;
        //원하는 회전 각도 = 타겟의 오일러각도 y
        //원하는 높이 = 타겟의 위치 + 높이


		var currentRotationAngle = transform.eulerAngles.y;
		var currentHeight = transform.position.y;

		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        //현재 회전 각도 = 보간각도(현재 회전 각, 원하는 회전각, 회전 감폭 * 델타타임)

		// Damp the height
		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
        //현재 높이 = 보간(현재 높이, 원하는 높이, 높이 감폭 * 델타 타임)

		// Convert the angle into a rotation
		var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        //현재 회전

		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;
        //카메라 위치 = 타겟의 위치
        //카메라 위치 = 카메라 위치 - 현재 회전각 * 거리

		transform.position = new Vector3(transform.position.x ,currentHeight , transform.position.z);
        //카메라의 높이 위치 설정

		transform.LookAt(target);
        //룩엣 함수 타겟을 바라보게함
	}
}
