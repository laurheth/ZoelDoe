using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAndShieldUser : Monster {
    public GameObject Sword;
    public GameObject Shield;
    Quaternion BaseSwordRot;
    public enum Limb { UArmL, LArmL, UArmR, LArmR, ULegL, ULegR, LLegL, LLegR, BootL, BootR, Head, Torso };
    public LimbIdent[] LimbIDs;
    Dictionary<Limb, Transform> LimbDict;
    bool attacking;
    float time;
    public float speedfraction;
    float stepPeriod;
    public float stepAngle;
    public float lowerLegAngle;
    float TorsoWobble;
    float TorsoBaseline;
    Quaternion restpos;
    float legLength;
    CapsuleCollider capsuleCollider;

    public override void Start()
    {
        base.Start();
        speedfraction = 1f;
        capsuleCollider = GetComponent<CapsuleCollider>();
        BaseSwordRot = Sword.transform.localRotation;
        attacking = false;
        LimbDict = new Dictionary<Limb, Transform>();
        for (int i = 0; i < LimbIDs.Length;i++) {
            LimbDict.Add(LimbIDs[i].LimbID, LimbIDs[i].LimbObj.transform);
        }
        time = 0f;
        TorsoBaseline = LimbDict[Limb.Torso].localPosition.y;
        legLength = LimbDict[Limb.ULegR].localPosition[1] + capsuleCollider.height / 2f;
        TorsoWobble = legLength * (1-Mathf.Cos(stepAngle * Mathf.PI / 360f));
        stepPeriod = (2 * stepAngle * legLength / speed)*(Mathf.PI/180f);
    }

    [System.Serializable]
    public class LimbIdent {
        public Limb LimbID;
        public GameObject LimbObj;
    }

    public override void Update()
    {
        base.Update();
        restpos = Quaternion.LookRotation(transform.forward);
        if (!attacking) {
            Sword.transform.localRotation = BaseSwordRot;
        }
        else {
            Sword.transform.rotation=restpos*Quaternion.Euler(0,0,90);
        }
        Shield.transform.rotation = restpos;

        /*LimbDict[Limb.BootL].transform.rotation = restpos;
        LimbDict[Limb.BootR].transform.rotation = restpos;*/
        //if (awakened) {
        if (Mathf.Approximately(speedfraction, 0f))
        {
            time = stepPeriod / 2f;
            LimbDict[Limb.LLegR].localRotation = Quaternion.Slerp(LimbDict[Limb.LLegR].localRotation, Quaternion.identity, 0.2f);
            LimbDict[Limb.LLegL].localRotation = Quaternion.Slerp(LimbDict[Limb.LLegL].localRotation, Quaternion.identity, 0.2f);
            LimbDict[Limb.ULegR].localRotation = Quaternion.Slerp(LimbDict[Limb.ULegR].localRotation, Quaternion.Euler(0, 0, 180), 0.2f);
            LimbDict[Limb.ULegL].localRotation = Quaternion.Slerp(LimbDict[Limb.ULegL].localRotation, Quaternion.Euler(0, 0, 180), 0.2f);
            LimbDict[Limb.Torso].localPosition = Vector3.Lerp(LimbDict[Limb.Torso].localPosition,new Vector3(0, TorsoBaseline, 0),0.2f);
        }
        else
        {
            time += speedfraction * Time.deltaTime;
            LimbDict[Limb.LLegR].localRotation = Quaternion.Euler(0, 0, lowerLegAngle * Mathf.PingPong(time, stepPeriod) / stepPeriod);
            LimbDict[Limb.ULegR].localRotation = Quaternion.Euler(0, 0, 180 + (stepAngle) * (Mathf.PingPong(time, stepPeriod) / stepPeriod - 0.5f));

            LimbDict[Limb.ULegL].localRotation = Quaternion.Euler(0, 0, 180 + (stepAngle) * (Mathf.PingPong(time - stepPeriod, stepPeriod) / stepPeriod - 0.5f));
            LimbDict[Limb.LLegL].localRotation = Quaternion.Euler(0, 0, lowerLegAngle * Mathf.PingPong(time - stepPeriod, stepPeriod) / stepPeriod);

            LimbDict[Limb.Torso].localPosition = new Vector3(0, TorsoBaseline - 2f * TorsoWobble * Mathf.PingPong(time - stepPeriod / 2, stepPeriod / 2) / (stepPeriod), 0);

        }
            
            //}
    }

}
