using UnityEngine;

//[AddComponentMenu("Vehicle/AntiRollBar")]

public class AntiRollBar : MonoBehaviour
{
    public WheelCollider WheelL;
    public WheelCollider WheelR;

    public float AntiRoll = 5000.0f;

    //안티롤 바 : 자동차의 롤링을 적게하는 장치
    //자동차의 기우는 현상을 바로잡게 해주는 스크립트
    //보면서 처음알았다. 자동차 용어는 어렵다

    public void FixedUpdate()
    {
        WheelHit hit;

        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = WheelL.GetGroundHit(out hit);
        if (groundedL)
        {
            travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;
        }

        bool groundedR = WheelR.GetGroundHit(out hit);
        if (groundedR)
        {
            travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;
        }

        float antiRollForce = (travelL - travelR) * AntiRoll;

        if (groundedL)
        {
            GetComponent<Rigidbody>().AddForceAtPosition(WheelL.transform.up * -antiRollForce, WheelL.transform.position);
        }

        if (groundedR)
        {
            GetComponent<Rigidbody>().AddForceAtPosition(WheelR.transform.up * antiRollForce, WheelR.transform.position);
        }
    }
}