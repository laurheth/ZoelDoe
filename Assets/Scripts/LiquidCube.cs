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
    public Vector2 movement;
    Vector3 position;
    Vector3 basepos;
    Vector2 offset;
    Renderer rend;
	// Use this for initialization
	void Start () {
        //int randint = Random.Range(0, 4);

        rend = GetComponent<Renderer>();
        Frequency = WaveSpeed / WaveLength;
        WaveNumber = 1 / WaveLength;
        basepos = transform.position + BaseHeight * Vector3.up;
        position = basepos;

        offset = new Vector2(-position[0], -position[2])/3f;
        Update();
	}
	
	// Update is called once per frame
	void Update () {
        position[1] = basepos[1];
        position[1] += WaveHeight * Mathf.Sin(2 * Mathf.PI * ( Frequency * (Time.time)));
        transform.position = position;
        offset += movement * Time.deltaTime;
        rend.material.SetTextureOffset("_MainTex",offset);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player") {
            other.gameObject.GetComponent<Player>().damage(1000);
        }
    }
}
