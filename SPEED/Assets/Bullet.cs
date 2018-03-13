using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
//using Photon

public class Bullet : PunBehaviour {

    public enum BulletType { Ranged, Fixed};

    public BulletType type;

    public float projectileSpeed = 100;

    public float lifetime = 3;

    [HideInInspector]
    public bool local;

    private bool hitCall = false;

    private BulletEffect bulletEffect;

	void Start () {

        this.bulletEffect = GetComponent<BulletEffect>();

	}
	
	void Update () {
		
        if(type == BulletType.Ranged)
        {
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime, Space.Self);
        }

        if(!local)
        {
            return;
        }

        if(lifetime <= 0 && !hitCall)
        {
            EventWasHit();
            hitCall = true;
        }
	}

    public void EventHit(GameObject car)
    {
        bulletEffect.Hit(car);
    }

    public void EventWasHit()
    {
        bulletEffect.PreFinish();
    }
}
