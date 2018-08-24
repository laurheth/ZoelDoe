using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliCannon : Monster {
    Transform propellor;
    Transform gun;
    public float gunrotatespeed;
    float gunangle;
    float propangle;
    float currentgunangle;
    float basethrust;
    Vector3 playerdirection;
    Vector3 GunAdjust;
    Vector3 targPos;
    public Vector3 offset;
    public float GraceDist;
    public float propspeed;
    public float speed;
    public float bulletspeed;
    public GameObject bullet;
    public float[] MinMaxFireRate;
    float timeToNextBullet;
    float basepropspeed;
	// Use this for initialization
	public override void Start () {
        base.Start();
        propangle = 0;
        targPos = playerrb.position+offset;
        currentgunangle = 0f;
        GunAdjust = new Vector3(0, 90, 0);
        propellor = transform.Find("Base/Propellor");
        gun = transform.Find("Base/Cannon");
        playerdirection = Vector3.zero;
        basepropspeed = propspeed;
        basethrust = rb.mass*Physics.gravity.magnitude/propspeed;
        //proprotation = Vector3.zero;
	}

    private void FixedUpdate()
    {
        // Apply thrust
        rb.AddForce(propspeed * basethrust * transform.up * Time.fixedDeltaTime, ForceMode.Impulse);
        if (!awakened) { return; }
        targPos = playerrb.position + offset;

        // vertical motion
        if (rb.position[1]+GraceDist < targPos[1] && rb.velocity[1]<speed) {
            propspeed = basepropspeed * 1.1f;
        }
        else if (rb.position[1]-GraceDist > targPos[1] && rb.velocity[1] > -speed){
            propspeed = basepropspeed / 1.1f;
        }
        else {
            propspeed = basepropspeed;
        }

        // horizontal motion
        if (rb.position[0]-GraceDist > targPos[0] && rb.velocity[0] > -speed) {
            propangle = 10;
            propspeed /= Mathf.Cos(propangle * Mathf.PI/180);
        }
        else if (rb.position[0]+GraceDist < targPos[0] && rb.velocity[0] < speed) {
            propangle = -10;
            propspeed /= Mathf.Cos(propangle * Mathf.PI / 180);
        }
        else {
            propangle = 0;
        }

        // Apply angle
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.Euler(0, 0, propangle), 0.2f));



    }

    // Update is called once per frame
    public override void Update () {
        base.Update();

        propellor.Rotate(Vector3.up * propspeed * Time.deltaTime);

        if (!awakened) {
            timeToNextBullet = MinMaxFireRate[1];
            return;
        }
        timeToNextBullet -= Time.deltaTime;
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

        if (timeToNextBullet < 0) {
            GameObject newbullet=Instantiate(bullet, gun.position, gun.rotation,transform.parent);
            newbullet.GetComponent<Rigidbody>().velocity = bulletspeed *( gun.localRotation* Vector3.left);

            timeToNextBullet = Random.Range(MinMaxFireRate[0], MinMaxFireRate[1]);
        }

	}
}
