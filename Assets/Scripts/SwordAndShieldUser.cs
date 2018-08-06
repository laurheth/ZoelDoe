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
    float TorsoBaseline;
    public float stabTime;
    public float stabSpeed;
    int facing;
    Quaternion restpos;
    float legLength;
    CapsuleCollider capsuleCollider;
    float speed;
    float targetShieldHeight;
    float currentShieldHeight;
    Vector3 shieldPosBase;
    Vector3 shieldPos;
    Vector3 swordPosBase;

    public void Awake()
    {
        //base.Start();
        facing = 1;
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
        shieldPosBase = Shield.transform.position-transform.position;
        //SetShieldHeight(shieldPosBase[1]);
        shieldPosBase[1] = 0;
        currentShieldHeight = 0.8f;
        SetShieldHeight(currentShieldHeight);
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

    /*private void FixedUpdate()
    {
        rb.MovePosition(rb.position - transform.right * speed * speedfraction * Time.fixedDeltaTime);
    }*/

    public void Update()
    {
        //base.Update();
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
            //Debug.Log(lowerLegAngle.ToString()+" "+time.ToString()+" "+stepPeriod.ToString());
            LimbDict[Limb.LLegR].localRotation = Quaternion.Euler(0, 0, lowerLegAngle * Mathf.PingPong(time, stepPeriod) / stepPeriod);
            LimbDict[Limb.ULegR].localRotation = Quaternion.Euler(0, 0, 180 + (stepAngle) * (Mathf.PingPong(time, stepPeriod) / stepPeriod - 0.5f));

            LimbDict[Limb.ULegL].localRotation = Quaternion.Euler(0, 0, 180 + (stepAngle) * (Mathf.PingPong(time - stepPeriod, stepPeriod) / stepPeriod - 0.5f));
            LimbDict[Limb.LLegL].localRotation = Quaternion.Euler(0, 0, lowerLegAngle * Mathf.PingPong(time - stepPeriod, stepPeriod) / stepPeriod);

            LimbDict[Limb.Torso].localPosition = new Vector3(0, TorsoBaseline - 2f * TorsoWobble * Mathf.PingPong(time - stepPeriod / 2, stepPeriod / 2) / (stepPeriod), 0);

        }
        ShieldHeightMove();
        /*if (facing * (playerobj.transform.position.x - transform.position.x) > 0)
        {
            facing *= -1;
            transform.rotation *= Quaternion.Euler(0, 180, 0);

        } */ 
            //}
    }

    void ShieldHeightMove() {
        currentShieldHeight = Mathf.Lerp(currentShieldHeight, targetShieldHeight, shieldMovement * Time.deltaTime);
        shieldPos = shieldPosBase + transform.up * currentShieldHeight;
        shieldPos[0] *= transform.right[0];
        shieldPos += transform.position;
        Shield.transform.position = shieldPos;
    }

    public void SetShieldHeight(float newheight) {
        targetShieldHeight = newheight;
        //transform.position+shieldPosBase + transform.up * newheight;
    }

    public void Stab (float stabheight) {
        StartCoroutine(StabCoroutine(stabheight*capsuleCollider.height));
    }

    // Stab animation
    IEnumerator StabCoroutine(float stabheight)
    {
        Debug.Log("Stabbing: " + stabheight);
        swordbox.enabled = true; // turn on swordbox to do damage
        attacking = true;
        // point forward and get ready to stab!
        swordrb.MoveRotation(Quaternion.Euler(0, 0, -90 * facing));
        swordrb.MovePosition(transform.position + transform.up * stabheight);

        float timepassed = 0f;
        yield return null;

        // Move sword out
        while (timepassed < Mathf.Abs(stabTime))
        {
            timepassed += Time.deltaTime;
            swordrb.MovePosition(transform.position + transform.up * stabheight
                                 + Sword.transform.up * timepassed * stabSpeed);
            yield return null;
        }
        timepassed = 0f;

        // Move sword back in
        while (timepassed < Mathf.Abs(stabTime))
        {
            timepassed += Time.deltaTime;
            swordrb.MovePosition(transform.position + transform.up * stabheight
                                 + Sword.transform.up * (stabTime - timepassed) * stabSpeed);
            yield return null;
        }

        // Return to rest position.
        swordrb.MovePosition(transform.position + swordPosBase
                             + ((facing < 0) ? (-2f * swordPosBase[0] * Vector3.right) : (Vector3.zero)));
        swordrb.MoveRotation(transform.rotation * BaseSwordRot);
        yield return null;

        // No longer attacking
        attacking = false;
        swordbox.enabled = false;
    }

}
