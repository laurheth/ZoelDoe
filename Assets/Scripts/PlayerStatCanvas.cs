using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatCanvas : MonoBehaviour {
    public GameObject HeartPrefab;
    public GameObject KeyPrefab;
    GameObject HeartTrack;
    GameObject KeyTrack;
	// Use this for initialization
	void Start () {
        
        HeartTrack = transform.Find("HeartTrack").gameObject;
        KeyTrack = transform.Find("KeyTrack").gameObject;

        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().AddStatCanvas(this);
	}
	
    public void SetHearts(int num) {
        SetTrack(HeartTrack, HeartPrefab, num);
    }

    public void SetKeys(int num)
    {
        SetTrack(KeyTrack, KeyPrefab, num);
    }

    void SetTrack(GameObject thistrack, GameObject thisprefab, int thisnum) {
        foreach (Transform childobj in thistrack.transform) {
            Destroy(childobj.gameObject);
        }
        if (thisnum > 0)
        {
            for (int i = 0; i < thisnum; i++)
            {
                Instantiate(thisprefab, thistrack.transform);
            }
        }
    }

}
