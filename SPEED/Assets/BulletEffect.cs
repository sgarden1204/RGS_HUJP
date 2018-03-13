using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon;

// Base class to create bullet effects
public class BulletEffect : PunBehaviour
{

    //히트시 효과음
    public virtual void Hit(GameObject car)
    {

    }

    // destroy this bullet and other copies over network in case of hit the track or other gameobjects with tag "World"
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "World")
        {
            PreFinish();
        }
    }

   //먼저 내껄 제거하고 전체 제거
    public void PreFinish()
    {
        Finish();
        photonView.RPC("Finish", PhotonTargets.Others);
    }


    [PunRPC]
    public virtual void Finish()
    {
        Destroy(gameObject);
    }
}