using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBug : Monster {
    public float speed;
    public float bonelength;
    public float t;
    public GameObject skull;
    public GameObject[] UpLegs;
    public GameObject[] DownLegs;
    public Vector3[] Feets;
    BoxCollider box;
    bool[] StaticFoot;
    //int[] zoffset;
	// Use this for initialization
	public override void Start() {
        base.Start();
        box = GetComponent<BoxCollider>();
        Feets = new Vector3[4];
        StaticFoot = new bool[4];
        //zoffset = new int[4];
        for (int i = 0; i < 4;i++) {
            //UpLegs[i].transform.parent = null;
            //DownLegs[i].transform.parent = null;
            Feets[i] = transform.position - box.size[1]*transform.up/2f+ transform.right * bonelength*(i-1.5f) + bonelength*transform.forward*Mathf.Pow(-1,i);
            BendLeg(i);
            StaticFoot[i] = true;
        }

	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
        //if (!awakened) { return; }
        for (int i = 0; i < 4;i++) {
            if (StaticFoot[i] && Mathf.Abs(Feets[i][0]-transform.position.x)>bonelength*1.5f) {
                StartCoroutine(Reposition(i));
            }
            BendLeg(i);
        }
        skull.transform.position = transform.position - 0.05f*Vector3.forward;
	}

    private void FixedUpdate()
    {
        if (awakened)
        {
            rb.MovePosition(transform.position + transform.right * speed * Time.fixedDeltaTime);
        }
    }

    void BendLeg(int FootNum) {/*
        Vector3 ABVector = Feets[FootNum] - transform.position;
        Vector3 PerpVector = Vector3.Cross(ABVector, transform.forward*(Feets[FootNum].x-transform.position.x));
        PerpVector = (PerpVector.normalized) *
            Mathf.Sqrt(Mathf.Pow(bonelength, 2) + ABVector.sqrMagnitude / 4f);
        //PerpVector = Vector3.zero;
        UpLegs[FootNum].transform.position = transform.position + ABVector / 4f - PerpVector/2f;
        DownLegs[FootNum].transform.position = Feets[FootNum] - ABVector / 4f - PerpVector / 2f;
        UpLegs[FootNum].transform.rotation = Quaternion.LookRotation(ABVector + PerpVector)*Quaternion.Euler(0, 90, 0);
        DownLegs[FootNum].transform.rotation = Quaternion.LookRotation(ABVector - PerpVector)*Quaternion.Euler(0, 90, 0);*/
        //UpLegs[FootNum].transform.localRotation = Quaternion.LookRotation((Feets[FootNum] - transform.position).normalized);// + bonelength * Vector3.up);
        //UpLegs[FootNum].transform.rota

        /*float anglediff = (UpLegs[FootNum].transform.rotation.eulerAngles[1]
                           - Quaternion.LookRotation(Feets[FootNum]).eulerAngles[1]);
        UpLegs[FootNum].transform.localRotation *= Quaternion.Euler(0, anglediff, 0);*/
        Vector3 relative = Feets[FootNum] - transform.position;
        Vector3 eulers = UpLegs[FootNum].transform.rotation.eulerAngles;
        Quaternion LookRot = Quaternion.LookRotation(relative);
        Quaternion LookOnlyY = Quaternion.Euler(eulers[0], LookRot.eulerAngles[1]+90, eulers[2]);
        UpLegs[FootNum].transform.rotation = LookOnlyY;
    }

    IEnumerator Reposition(int FootNum) {
        StaticFoot[FootNum] = false;
        Feets[FootNum] += transform.up * 0.1f;
        Vector3 TargPosition = transform.position - box.size[1] * transform.up / 2f + transform.right * bonelength * 1.49f + bonelength * transform.forward * Mathf.Pow(-1, FootNum);
            //transform.position - transform.up *bonelength + transform.right * bonelength*1.49f;
        while ((TargPosition-Feets[FootNum]).magnitude > 0.1f) {
            Debug.Log(TargPosition);
            Feets[FootNum] = Vector3.Lerp(Feets[FootNum], TargPosition, t);
            yield return null;
        }
        StaticFoot[FootNum] = true;
        yield return null;
    }
}
