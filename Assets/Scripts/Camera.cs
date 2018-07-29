using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {
    GameObject player;
    Rigidbody playerrb;
    public Vector3 offset;
    public float t;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        transform.position = player.transform.position + offset;
        playerrb = player.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, playerrb.position +offset,t);
	}
}
