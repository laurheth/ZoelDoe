using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBug : Monster {
    public float speed;
    public float bonelength;
    public float t;
    public GameObject skull;
    public GameObject[] UpLegs;
    public float jumpheight;
    float jumpspeed;
    //public GameObject[] DownLegs;
    public Vector3[] Feets;
    BoxCollider box;
    int numononeside;
    int numfeets;
    float jumptime;
    int facing;
    //bool needtocorrect;
    bool[] StaticFoot;
    //int[] zoffset;
	// Use this for initialization
	public override void Start() {
        jumpspeed = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity[1]) * jumpheight);
        numfeets = UpLegs.Length;
        facing = 1;
        base.Start();
        numononeside = 0;
        box = GetComponent<BoxCollider>();
        Feets = new Vector3[numfeets];
        StaticFoot = new bool[numfeets];
        //zoffset = new int[4];
        for (int i = 0; i < numfeets;i++) {
            //UpLegs[i].transform.parent = null;
            //DownLegs[i].transform.parent = null;
            Feets[i] = box.size[1]*transform.up/2f+ transform.right * bonelength*((4f/ numfeets) *i-1.5f) + bonelength*transform.forward*Mathf.Pow(-1,i);
            BendLeg(i);
            StaticFoot[i] = true;
        }
        //needtocorrect = false;
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();

        if (awakened) { jumptime += Time.deltaTime; }
        numononeside = 0;
        //if (!awakened) { return; }
        for (int i = 0; i < numfeets;i++) {
            if (StaticFoot[i] && (Mathf.Abs(Feets[i][0])>bonelength*1.5f)) {
                StartCoroutine(Reposition(i));
            }

            BendLeg(i);
        }
        if (facing*(playerobj.transform.position.x-transform.position.x)<0) {
            facing *= -1;
            transform.rotation *= Quaternion.Euler(0, 180, 0);
            /*for (int i = 0; i < 4;i++) {
                Feets[i][0] *= -1; //Quaternion.Euler(0, 180, 0) * Feets[i];
                Feets[i][2] *= -1;
            }*/
        }
        if (jumptime > 5f)
        {
            rb.AddForce(jumpspeed * Vector3.up, ForceMode.VelocityChange);
            jumptime = -Random.Range(0f,5f);
        }
        //skull.transform.position = transform.position - 0.05f*Vector3.forward;
	}

    private void FixedUpdate()
    {
        if (awakened)
        {
            rb.MovePosition(transform.position + transform.right * speed * Time.fixedDeltaTime);
            for (int i = 0; i < numfeets;i++) {
                Feets[i] -= transform.right*speed * Time.fixedDeltaTime;
            }
        }
    }

    void BendLeg(int FootNum) {
        Vector3 relative = Feets[FootNum];
        Vector3 eulers = UpLegs[FootNum].transform.rotation.eulerAngles;
        Quaternion LookRot = Quaternion.LookRotation(relative);
        Quaternion LookOnlyY = Quaternion.Euler(eulers[0], LookRot.eulerAngles[1]+90, eulers[2]);
        UpLegs[FootNum].transform.rotation = LookOnlyY;
    }

    public override void Die()
    {
        skull.transform.parent = transform.parent;
        skull.GetComponent<Rigidbody>().isKinematic = false;
        skull.GetComponent<Collider>().enabled = true;
        for (int i = 0; i < numfeets;i++) {
            UpLegs[i].transform.parent = transform.parent;
            UpLegs[i].GetComponent<Rigidbody>().isKinematic = false;
            UpLegs[i].GetComponent<Collider>().enabled = true;
            Transform childobj=UpLegs[i].transform.Find("LLeg");
            if (childobj != null) {
                childobj.parent = transform.parent;
                childobj.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                childobj.gameObject.GetComponent<Collider>().enabled = true;
            }
        }
        base.Die();
    }

    IEnumerator Reposition(int FootNum) {
        StaticFoot[FootNum] = false;
        Feets[FootNum] += transform.up * 0.1f;
        Vector3 TargPosition;
        TargPosition = box.size[1] * transform.up / 2f + transform.right * bonelength * Random.Range(0.4f,1.4f) + bonelength * transform.forward * Mathf.Pow(-1, FootNum);

            //transform.position - transform.up *bonelength + transform.right * bonelength*1.49f;
        while ((TargPosition-Feets[FootNum]).magnitude > 0.1f) {
            //Debug.Log(TargPosition);
            Feets[FootNum] = Vector3.Lerp(Feets[FootNum], TargPosition, t);
            yield return null;
        }
        StaticFoot[FootNum] = true;
        yield return null;
    }

}
