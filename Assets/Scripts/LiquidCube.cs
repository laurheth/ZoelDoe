using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidCube : MonoBehaviour {
    public float BaseHeight;
    public float WaveLength;
    float Frequency;
    float WaveNumber;
    public float WaveHeight;
    public float WaveSpeed;
    Vector3 position;
    Vector3 basepos;
	// Use this for initialization
	void Start () {
        Frequency = WaveSpeed / WaveLength;
        WaveNumber = 1 / WaveLength;
        basepos = transform.position + BaseHeight * Vector3.up;
        position = basepos;
        Update();
	}
	
	// Update is called once per frame
	void Update () {
        position[1] = basepos[1];
        position[1] += WaveHeight * Mathf.Sin(2 * Mathf.PI * ( Frequency * (Time.time) + WaveNumber*position[0]));
        transform.position = position;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player") {
            other.gameObject.GetComponent<Player>().damage(1000);
        }
    }
}
