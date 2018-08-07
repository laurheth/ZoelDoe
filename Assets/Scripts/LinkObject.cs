using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkObject : MonoBehaviour {
    public string currentScreen;
    public string targetScreen;
    public LevelGenerator levelGenerator;
    public bool active;
    public int xidnum;
    bool switching;
    public Vector3 stepdirection;
    private void Awake()
    {
        switching = false;
        active = true;
        xidnum = 0;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag  =="Player") {
            active = true;
        }
        //Debug.Log(active);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Entered");
        if (!switching && active && other.gameObject.tag=="Player" && !other.attachedRigidbody.isKinematic) {
            //Debug.Log("Switch?");
            switching = true;
            StartCoroutine(Switching());
            //cameraScript.jumpToPos();
        }
    }

    IEnumerator Switching() {
        bool justfall = false;
        GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");
        if (Mathf.Approximately((stepdirection - Vector3.down).magnitude, 0f))
        {
            justfall = true;
            //yield return new WaitForSeconds(0.5f);
        }
        //yield return null;
        yield return PlayerObj.GetComponent<Player>().movecoroutine(stepdirection,justfall,1.5f);

        levelGenerator.SwitchScreen(targetScreen);
        Vector3 movelocation = Vector3.zero;
        int miny = 200;
        int rememberx = -1;
        //Debug.Log("this xid" + xidnum);
        foreach (GameObject linkobj in GameObject.FindGameObjectsWithTag("Linker"))
        {
            if (linkobj.GetComponent<LinkObject>().targetScreen.Equals(currentScreen))
            {
                linkobj.GetComponent<LinkObject>().active = false;
                if ((int)(linkobj.transform.position[1]) <= miny)
                {
                    movelocation = linkobj.transform.position;
                    miny = (int)movelocation[1];
                    //Debug.Log("That:" + linkobj.GetComponent<LinkObject>().xidnum);
                    if (linkobj.GetComponent<LinkObject>().xidnum == xidnum) {
                        rememberx = (int)movelocation[0];
                        //Debug.Log("Found: breaking");
                        //break;
                    }
                }
            }
        }
        movelocation[2] = 0f;
        //movelocation[1] += 0.5f;
        if (rememberx > 0) { movelocation[0]=rememberx; }
        movelocation -= stepdirection;
        PlayerObj.transform.position = movelocation;
        PlayerObj.gameObject.GetComponent<Player>().MoveOne(stepdirection,justfall);
        FindObjectOfType<Camera>().GetComponent<CameraScript>().jumpToPos();
    }

    public void SetDirection (string direction) {
        stepdirection = Vector3.zero;
        switch(direction) {
            case "right":
                stepdirection = Vector3.right;
                break;
            case "left":
                stepdirection = Vector3.left;
                break;
            case "up":
                stepdirection = Vector3.up;
                break;
            case "down":
                stepdirection = Vector3.down;
                break;
            default:
                stepdirection = Vector3.zero;
                break;
        }
        transform.position += stepdirection;
    }
}
