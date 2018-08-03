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
    bool[] StaticFoot;
	// Use this for initialization
	public override void Start() {
        base.Start();
        Feets = new Vector3[4];
        StaticFoot = new bool[4];
        for (int i = 0; i < 4;i++) {
            //UpLegs[i].transform.parent = null;
            //DownLegs[i].transform.parent = null;
            Feets[i] = transform.position - bonelength*transform.up + transform.right * bonelength*(i-1.5f);
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

    void BendLeg(int FootNum) {
        Vector3 ABVector = Feets[FootNum] - transform.position;
        Vector3 PerpVector = Vector3.Cross(ABVector, transform.forward*(Feets[FootNum].x-transform.position.x));
        PerpVector = (PerpVector.normalized) *
            Mathf.Sqrt(Mathf.Pow(bonelength, 2) + ABVector.sqrMagnitude / 4f);
        //PerpVector = Vector3.zero;
        UpLegs[FootNum].transform.position = transform.position + ABVector / 4f - PerpVector/2f;
        DownLegs[FootNum].transform.position = Feets[FootNum] - ABVector / 4f - PerpVector / 2f;
        UpLegs[FootNum].transform.rotation = Quaternion.LookRotation(ABVector + PerpVector)*Quaternion.Euler(0, 90, 0);
        DownLegs[FootNum].transform.rotation = Quaternion.LookRotation(ABVector - PerpVector)*Quaternion.Euler(0, 90, 0);
    }

    IEnumerator Reposition(int FootNum) {
        StaticFoot[FootNum] = false;
        Feets[FootNum] += transform.up * 0.1f;
        Vector3 TargPosition = transform.position - transform.up *bonelength + transform.right * bonelength*1.49f;
        while ((TargPosition-Feets[FootNum]).magnitude > 0.1f) {
            Debug.Log(TargPosition);
            Feets[FootNum] = Vector3.Lerp(Feets[FootNum], TargPosition, t);
            yield return null;
        }
        StaticFoot[FootNum] = true;
        yield return null;
    }
}
