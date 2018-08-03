using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public enum ItemType { None, Key, Health, Magic};
    public ItemType itemType;
    public int Quantity;
    int trackid;
    // Use this for initialization
    /*void Start () {
		
	}*/

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit:" + other.gameObject.tag);
        if (other.gameObject.tag=="sword") {
            other.transform.parent.gameObject.GetComponent<Player>().GetItem(itemType, Quantity);
            transform.parent.gameObject.GetComponent<LevelGenerator>().UnTrack(trackid);
            Destroy(gameObject);
        }
    }

    public void SetID(int tid)
    {
        trackid = tid;
    }

}
