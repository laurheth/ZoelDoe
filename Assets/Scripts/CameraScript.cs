using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    GameObject player;
    Rigidbody playerrb;
    public Vector3 offset;
    Vector3 defaultoffset;
    public float t;

    public GameObject LevGen;
    float MinX, MaxX, MinZ, MaxZ;
    Vector2 EdgeDist;
    Vector3 targpos;
    Camera cam;
    //LevelGenerator levgenscript;
    // Use this for initialization
    void Start()
    {
        //defaultoffset = offset + Vector3.zero;
        player = GameObject.FindGameObjectWithTag("Player");
        transform.position = player.transform.position + offset;
        targpos = transform.position;
        playerrb = player.GetComponent<Rigidbody>();
        //levgenscript = LevGen.GetComponent<LevelGenerator>();
        //CalculateEdges();
        jumpToPos();
    }
	
	// Update is called once per frame
	void Update () {
        FindTarget();
        transform.position = Vector3.Lerp(transform.position, targpos,t);
	}

    void FindTarget() {
        targpos = playerrb.position + offset;
        if (targpos[0] < MinX) { targpos[0] = MinX; }
        if (targpos[0] > MaxX) { targpos[0] = MaxX; }
        if (targpos[1] < MinZ) { targpos[1] = MinZ; }
        if (targpos[1] > MaxZ) { targpos[1] = MaxZ; }
        //return targpos;
    }

    public void jumpToPos() {
        FindTarget();
        transform.position = targpos;
    }

    public void SaveOffset() {
        defaultoffset = offset + Vector3.zero;
    }

    void CalculateEdges() {
        cam = GetComponent<Camera>();
        float baseedge = Mathf.Abs(offset[2] * Mathf.Tan(2 * Mathf.PI * cam.fieldOfView / 720));//-0.5f;
        EdgeDist = new Vector2(baseedge * cam.aspect, baseedge);
        //Debug.Log(EdgeDist);
    }

    public void ResetOffset() {
        //Debug.Log(defaultoffset);
        offset = defaultoffset + Vector3.zero;
        //Debug.Log(offset);
    }

    public void SetBoundaries(int minx, int maxx, int minz, int maxz) {
        //Debug.Log("minx:"+minx);
        //Debug.Log("minz:"+minz);
        //Debug.Log("maxx:"+maxx);
        //Debug.Log("maxz:"+maxz);

        bool movecloser = false;
        int breaker = 0;
        do
        {
            if (movecloser) {
                offset[2] += 0.1f;
                movecloser = false;
            }
            CalculateEdges();

            MinX = minx + EdgeDist[0] - offset[0];
            MaxX = maxx - EdgeDist[0] - offset[0];

            if (MaxX < MinX)
            {
                //Debug.Log("x=x");
                MaxX = (MaxX + MinX) / 2f;
                MinX = MaxX + 0f;
                movecloser = true;
            }

            MinZ = minz + EdgeDist[1] - offset[1];
            MaxZ = maxz - EdgeDist[1] - offset[1];

            if (MaxZ < MinZ)
            {
                //Debug.Log("z=z");
                MaxZ = (MaxZ + MinZ) / 2f;
                MinZ = MaxZ + 0f;
                movecloser = true;
            }

            //Debug.Log("MinX:" + MinX);
            //Debug.Log("MinZ:" + MinZ);
            //Debug.Log("MaxX:" + MaxX);
            //Debug.Log("MaxZ:" + MaxZ);
            breaker++;
        } while (movecloser && breaker<100);

    }
}
