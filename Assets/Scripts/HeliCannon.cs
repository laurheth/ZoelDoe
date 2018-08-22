using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliCannon : Monster {
    Transform propellor;
    Transform gun;
    public float gunrotatespeed;
    float gunangle;
    float currentgunangle;
    Vector3 playerdirection;
    Vector3 GunAdjust;
    public float propspeed;
	// Use this for initialization
	public override void Start () {
        base.Start();
        currentgunangle = 0f;
        GunAdjust = new Vector3(0, 90, 0);
        propellor = transform.Find("Base/Propellor");
        gun = transform.Find("Base/Cannon");
        playerdirection = Vector3.zero;
        //proprotation = Vector3.zero;
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();

        propellor.Rotate(transform.up * propspeed * Time.deltaTime);
        playerdirection = playerobj.transform.position - gun.position;

        gunangle = 180 * Mathf.Atan(playerdirection[1] / playerdirection[0]) / Mathf.PI;

        if ((playerdirection[1]<0 && gunangle < 0) || playerdirection[1]>0 && playerdirection[0]>0) { gunangle += 180; }

        if (gunangle > -45 && gunangle < 225)
        {
            Debug.Log(gunangle);
            currentgunangle = Mathf.Lerp(currentgunangle, gunangle, gunrotatespeed * Time.deltaTime);
        }

        //gun.rotation = Quaternion.LookRotation(playerobj.transform.position-gun.position)* Quaternion.Euler(GunAdjust);
        //playerdirection = playerobj.transform.position - gun.position;



        gun.rotation=Quaternion.Euler(0, 0, currentgunangle);//* Quaternion.Euler(GunAdjust);
	}
}
