using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
    protected GameObject playerobj;
    Player player;
    protected Rigidbody playerrb;
    protected Transform playerShield;
    protected float playerShieldRelHeight;
    public int hp;
    /*public float speed;
    public float flightprob;
    public float jumpprob;
    public float jumpspeed;*/
    //public float acceleration;
    public int damage;
    //public float speed;
    public float awakedistsqr;
    protected bool awakened;
    int trackid;
    protected Rigidbody rb;
	// Use this for initialization
	public virtual void Start () {
        playerobj = GameObject.FindGameObjectWithTag("Player");
        player = playerobj.GetComponent<Player>();
        playerrb = playerobj.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        awakened = false;
        //playerShield = GameObject.FindGameObjectWithTag("PlayerShield").transform;
        playerShield = playerobj.GetComponent<SwordAndShieldUser>().Shield.transform;
        Debug.Log("shields found:"+GameObject.FindGameObjectsWithTag("PlayerShield").Length);
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("PlayerShield").Length;i++) {
            Debug.Log("Shield "+i+" " +GameObject.FindGameObjectsWithTag("PlayerShield")[i].name);
            Debug.Log(GameObject.FindGameObjectsWithTag("PlayerShield")[i].transform.position);
        }
	}
	
	// Update is called once per frame
	public virtual void Update () {
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
        playerShieldRelHeight = playerShield.position.y - transform.position.y;
        //playerShield.name = "aaaa";
        //Debug.Log("playershield: " + playerShield.position);

	}

    public virtual void OnCollisionEnter(Collision collision)
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
            Die();
        }
    }

    public virtual void Die() {
        transform.parent.gameObject.GetComponent<LevelGenerator>().UnTrack(trackid);
        Destroy(gameObject);
    }

    public void SetID(int tid) {
        trackid = tid;
    }
}
