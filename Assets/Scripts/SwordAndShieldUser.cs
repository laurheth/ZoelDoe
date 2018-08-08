using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAndShieldUser : MonoBehaviour {
    public GameObject Sword;
    public GameObject Shield;
    Collider swordbox;
    Rigidbody swordrb;
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
    public float shieldMovement;
    float TorsoWobble;
    //float armlength;
    public float footsize;
    //public float crouchThreshold;
    float TorsoBaseline;
    float crouch;
    float crouchto;
    float crouchangle;
    float TorsoHeight;
    public float stabTime;
    public float stabSpeed;
    public int facing;
    Quaternion restpos;
    float legLength;
    CapsuleCollider capsuleCollider;
    float speed;
    float targetShieldHeight;
    float currentShieldHeight;
    Vector3 shieldPosBase;
    Vector3 shieldPos;
    Vector3 swordPosBase;
    Vector3 swordPos;

    public void Awake()
    {
        //base.Start();
        facing = 1;
        crouch = 1f;
        crouchto = 1f;
        crouchangle = 0f;
        speedfraction = 1f;
        capsuleCollider = GetComponent<CapsuleCollider>();
        BaseSwordRot = Sword.transform.localRotation;
        swordPosBase = Sword.transform.localPosition;
        swordbox = Sword.GetComponent<Collider>();
        swordrb = Sword.GetComponent<Rigidbody>();
        attacking = false;
        LimbDict = new Dictionary<Limb, Transform>();
        for (int i = 0; i < LimbIDs.Length;i++) {
            LimbDict.Add(LimbIDs[i].LimbID, LimbIDs[i].LimbObj.transform);
        }
        time = 0f;
        TorsoBaseline = LimbDict[Limb.Torso].localPosition.y;
        legLength = LimbDict[Limb.ULegR].localPosition[1] + capsuleCollider.height / 2f;
        TorsoWobble = legLength * (1-Mathf.Cos(stepAngle * Mathf.PI / 360f));
        SetSpeed();
        shieldPosBase = Shield.transform.position - transform.position;//-LimbDict[Limb.Torso].position;
        //if (shieldPosBase[0]<0) {
        //    shieldPosBase[0] *= -1;
        //}
        //Debug.Log(shieldPosBase);
        //SetShieldHeight(shieldPosBase[1]);
        shieldPosBase[1] = 0;
        currentShieldHeight = 0.8f;
        SetShieldHeight(currentShieldHeight);
//TorsoHeight=LimbDict[Limb.Torso].localPosition
        Crouch(1f);
        //shieldPos=transform.position + shieldPosBase;
        //Shield.transform.position = transform.position+shieldPosBase;
    }

    public void SetSpeed(float newspeed=1f) {
        speed = newspeed;
        //Debug.Log("Speed: " + speed);
        stepPeriod = (2 * stepAngle * legLength / speed) * (Mathf.PI / 180f);
    }

    public void SetSpeedFraction(float currentspeed) {
        speedfraction = currentspeed / speed;
    }

    [System.Serializable]
    public class LimbIdent {
        public Limb LimbID;
        public GameObject LimbObj;
    }

    public void SetCrouch(float newcrouchfraction) {
        crouchto = Mathf.Clamp(newcrouchfraction, 0.1f, 1f);
    }

    void Crouch(float crouchfraction) {
        crouch = Mathf.Lerp(crouch,Mathf.Clamp(crouchfraction, 0.1f, 1f),shieldMovement*Time.deltaTime);
        crouchangle = 90-180f*Mathf.Asin(crouch)/Mathf.PI;
        TorsoHeight = TorsoBaseline - (legLength-footsize)*(1-crouch);// + Mathf.Min(((-legLength/2) * (1-crouch))+footsize,0);
        Debug.Log("crch:" + Mathf.Min(((-legLength / 2) * (1 - crouch)) + footsize, 0));
    }

    /*private void FixedUpdate()
    {
        rb.MovePosition(rb.position - transform.right * speed * speedfraction * Time.fixedDeltaTime);
    }*/

    private void LateUpdate()
    {
        ArmCompute(LimbDict[Limb.LArmL], LimbDict[Limb.UArmL], swordPos);
        restpos = Quaternion.LookRotation(transform.forward);
        if (!attacking)
        {
            Sword.transform.localRotation = BaseSwordRot;
            swordPos = transform.position + swordPosBase
                             + ((facing < 0) ? (-2f * swordPosBase[0] * Vector3.right) : (Vector3.zero));
        }
        else
        {
            Sword.transform.rotation = restpos * Quaternion.Euler(0, 0, 90);
        }
        Shield.transform.rotation = restpos;
    }

    public void Update()
    {
        //base.Update();
        restpos = Quaternion.LookRotation(transform.forward);
        if (!attacking) {
            Sword.transform.localRotation = BaseSwordRot;
            swordPos = transform.position + swordPosBase
                             + ((facing < 0) ? (-2f * swordPosBase[0] * Vector3.right) : (Vector3.zero));
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
            LimbDict[Limb.LLegR].localRotation = Quaternion.Slerp(LimbDict[Limb.LLegR].localRotation, Quaternion.Euler(0,0,0+2*crouchangle), 0.2f);
            LimbDict[Limb.LLegL].localRotation = Quaternion.Slerp(LimbDict[Limb.LLegL].localRotation, Quaternion.Euler(0, 0, 0+ 2*crouchangle), 0.2f);
            LimbDict[Limb.ULegR].localRotation = Quaternion.Slerp(LimbDict[Limb.ULegR].localRotation, Quaternion.Euler(0, 0, 180- crouchangle), 0.2f);
            LimbDict[Limb.ULegL].localRotation = Quaternion.Slerp(LimbDict[Limb.ULegL].localRotation, Quaternion.Euler(0, 0, 180- crouchangle), 0.2f);
            LimbDict[Limb.Torso].localPosition = Vector3.Lerp(LimbDict[Limb.Torso].localPosition,new Vector3(0, TorsoHeight, 0),0.2f);
        }
        else
        {
            time += speedfraction/crouch * Time.deltaTime;
            //Debug.Log(lowerLegAngle.ToString()+" "+time.ToString()+" "+stepPeriod.ToString());
            LimbDict[Limb.LLegR].localRotation = Quaternion.Euler(0, 0, 2*crouchangle+lowerLegAngle * Mathf.PingPong(time, stepPeriod) / stepPeriod);
            LimbDict[Limb.ULegR].localRotation = Quaternion.Euler(0, 0, -crouchangle +180 + (stepAngle) * (Mathf.PingPong(time, stepPeriod) / stepPeriod - 0.5f));

            LimbDict[Limb.ULegL].localRotation = Quaternion.Euler(0, 0, -crouchangle +180 + (stepAngle) * (Mathf.PingPong(time - stepPeriod, stepPeriod) / stepPeriod - 0.5f));
            LimbDict[Limb.LLegL].localRotation = Quaternion.Euler(0, 0, 2*crouchangle +lowerLegAngle * Mathf.PingPong(time - stepPeriod, stepPeriod) / stepPeriod);

            LimbDict[Limb.Torso].localPosition = new Vector3(0, TorsoHeight - 2f * TorsoWobble * Mathf.PingPong(time - stepPeriod / 2, stepPeriod / 2) / (stepPeriod), 0);

        }
        ShieldHeightMove();
        Crouch(crouchto);
        /*if (facing * (playerobj.transform.position.x - transform.position.x) > 0)
        {
            facing *= -1;
            transform.rotation *= Quaternion.Euler(0, 180, 0);

        } */ 
            //}
    }

    void ShieldHeightMove() {
        /*if (currentShieldHeight<crouchThreshold) {
            Crouch(0.75f);
        }
        else {
            Crouch(1f);
        }*/
        currentShieldHeight = Mathf.Lerp(currentShieldHeight, targetShieldHeight, shieldMovement * Time.deltaTime);
        shieldPos = shieldPosBase + transform.up * currentShieldHeight;
        shieldPos[0] *= transform.right[0];
        shieldPos += transform.position;
        //Shield.transform.position = shieldPos;
        ArmCompute(LimbDict[Limb.LArmR], LimbDict[Limb.UArmR], shieldPos);
    }

    public void SetShieldHeight(float newheight) {
        targetShieldHeight = newheight;
        //transform.position+shieldPosBase + transform.up * newheight;
    }

    public void Stab (float stabheight, bool scalebycapsule=true) {
        Debug.Log(stabheight);
        Debug.Log(capsuleCollider.height);
        if (scalebycapsule)
        {
            StartCoroutine(StabCoroutine(stabheight * capsuleCollider.height));
        }
        else {
            StartCoroutine(StabCoroutine(stabheight));
        }
    }

    // Assumes arms of equal length
    // Treats like an isocelese triangle, base is line from shoulder to hand,
    // Vertex angle is elbow
    void ArmCompute(Transform ForeArm, Transform UpperArm, Vector3 targetpos) {
        Vector3 BaseVector = targetpos - UpperArm.position;
        float offsetangle = Vector3.Angle(-transform.right, BaseVector);
        if (BaseVector[1] > 0) { offsetangle *= -1; }
        float armlength = (ForeArm.position - UpperArm.position).magnitude;

        float height = Mathf.Sqrt(armlength * armlength - BaseVector.sqrMagnitude / 4);
        if (float.IsNaN(height)) {
            height = 0f;
        }

        float baseAngle = Mathf.Asin(height / armlength);

        float elbowAngle = Mathf.PI - 2 * baseAngle;
        //Debug.Log(UpperArm.position);
        UpperArm.localEulerAngles=new Vector3(0, 0, 180f*baseAngle/Mathf.PI + offsetangle+90);
        ForeArm.localEulerAngles = new Vector3(0, 0, elbowAngle * 180f/Mathf.PI+180f);

    }

    // Stab animation
    IEnumerator StabCoroutine(float stabheight)
    {
        Debug.Log("Stabbing: " + stabheight);
        swordbox.enabled = true; // turn on swordbox to do damage

        // point forward and get ready to stab!
        /*swordrb.MoveRotation(Quaternion.Euler(0, 0, -90 * facing));
        swordrb.MovePosition(transform.position + transform.up * stabheight);*/
        swordPos = transform.position + transform.up * stabheight;
        attacking = true;
        float timepassed = 0f;
        yield return null;

        // Move sword out
        while (timepassed < Mathf.Abs(stabTime))
        {
            timepassed += Time.deltaTime;
            swordPos=transform.position + transform.up * stabheight
                                 - transform.right * timepassed * stabSpeed;
            yield return null;

        }
        timepassed = 0f;

        // Move sword back in
        while (timepassed < Mathf.Abs(stabTime))
        {
            timepassed += Time.deltaTime;
            swordPos=transform.position + transform.up * stabheight
                              - transform.right * (stabTime - timepassed) * stabSpeed;
            yield return null;
        }
        attacking = false;
        // Return to rest position.
        swordPos=transform.position + swordPosBase
                             + ((facing < 0) ? (-2f * swordPosBase[0] * Vector3.right) : (Vector3.zero));
        //swordrb.MoveRotation(transform.rotation * BaseSwordRot);
        yield return null;

        // No longer attacking

        swordbox.enabled = false;
    }

}
