using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDoor : MonoBehaviour {
    public List<GameObject> Doorparts;
    public List<int> TrackID;
    public GameObject Lock;
    bool isMaster;
    public bool closed;
    public BoxCollider collide;
    //int TrackID;
	// Use this for initialization
	void Awake () {
        collide = GetComponent<BoxCollider>();
        closed = true;
        isMaster = true;
        Doorparts = new List<GameObject>();
        TrackID = new List<int>();
        Doorparts.Add(transform.Find("Door").gameObject);
        Lock = transform.Find("Lock").gameObject;
	}

    // Switch on/off
    public void changeMaster(bool newState) {
        isMaster = newState;
    }

    public void mergeDoors(LockDoor otherdoor) {

        Destroy(otherdoor.Lock);
        otherdoor.Lock = null;

        foreach (GameObject doorprt in otherdoor.Doorparts) {
            Doorparts.Add(doorprt);
        }
        otherdoor.collide.enabled = false;

        SetID(otherdoor.GetID());

        otherdoor.enabled = false;

        Vector3 lockPos = Vector3.zero;

        foreach (GameObject doorprt in Doorparts) {
            lockPos += doorprt.transform.position;
        }

        lockPos /= Doorparts.Count;

        collide.center = lockPos-transform.position;
        collide.size = new Vector3(0.4f, Doorparts.Count+0.2f, 1f);

        Lock.transform.position = lockPos;

    }

    public void OpenDoor() {
        closed = false;

        LevelGenerator levelGenerator = transform.parent.gameObject.GetComponent<LevelGenerator>();
        foreach (int id in TrackID) {
            levelGenerator.UnTrack(id);
        }
        StartCoroutine(OpenAnimation());
    }

    IEnumerator OpenAnimation() {
        float timePassed = 0;
        float ysize = 1f;
        while (timePassed<0.5f) {
            timePassed += Time.deltaTime;
            ysize /= 1.1f;
            foreach (GameObject doorprt in Doorparts) {
                doorprt.transform.position =
                           Vector3.Lerp(doorprt.transform.position,
                                        Lock.transform.position, 0.1f);
                doorprt.transform.localScale = new Vector3(0.4f, ysize, 0.4f);
            }
            yield return null;
        }
        Destroy(gameObject);
        foreach (GameObject doorprt in Doorparts) {
            Destroy(doorprt);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "LockDoor" && isMaster) {
            LockDoor otherDoor = collision.gameObject.GetComponent<LockDoor>();
            otherDoor.changeMaster(false);
            changeMaster(otherDoor);
            mergeDoors(otherDoor);
        }
    }

    public void SetID(int ID) {
        TrackID.Add(ID);
    }

    public int GetID() {
        return TrackID[0];
    }
}
