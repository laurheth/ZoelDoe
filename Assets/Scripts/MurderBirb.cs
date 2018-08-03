using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurderBirb : MonoBehaviour {
    GameObject playerobj;
    Player player;
    Rigidbody playerrb;
    public int hp;
    public float speed;
    public float flightprob;
    public float jumpprob;
    public float jumpspeed;
    //public float acceleration;
    public int damage;
    public float awakedistsqr;
    bool awakened;
    int trackid;
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        playerobj = GameObject.FindGameObjectWithTag("Player");
        player = playerobj.GetComponent<Player>();
        playerrb = playerobj.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        awakened = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!awakened)
        {
            if ((playerrb.position - rb.position).sqrMagnitude < awakedistsqr)
            {
                awakened = true;
            }
            else
            {
                return;
            }
        }
        rb.MovePosition(rb.position+speed*Time.deltaTime*(playerrb.position-rb.position).normalized);
        if (playerrb.position[0]<rb.position[0]) {
            rb.MoveRotation(Quaternion.LookRotation(Vector3.back));
        }
        else {
            rb.MoveRotation(Quaternion.LookRotation(Vector3.forward));
        }
        if (Random.value<jumpprob) {
            rb.AddForce(Vector3.up * jumpspeed, ForceMode.VelocityChange);
        }
        if (Random.value<flightprob) {
            rb.useGravity = !rb.useGravity;
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag) {
            case "Player":
                player.damage(damage);
                break;
            case "sword":
                Damage(collision.gameObject.GetComponent<sword>().damage);
                break;
        }

    }

    public void Damage(int dmg) {
        hp -= dmg;
        if (hp<=0) {
            transform.parent.gameObject.GetComponent<LevelGenerator>().UnTrack(trackid);
            Destroy(gameObject);
        }
    }

    public void SetID(int tid) {
        trackid = tid;
    }
}
