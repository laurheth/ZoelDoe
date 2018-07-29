using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject shield;
    public GameObject sword;
    public float acceleration;
    public float maxspeed;
    public float minJumpSpeed;
    public float maxJumpSpeed;
    public float stabTime;
    public float stabSpeed;
    public int swordvertex;
    public int shieldvertex;
    float horizspeed;
    float horizaxis;
    float vertaxis;
    float facing;
    Rigidbody rb;
    Rigidbody shieldrb;
    Rigidbody swordrb;
    Vector3 shieldpos;
    Vector3 swordpos;
    Quaternion swordangle;
    BoxCollider swordbox;
    LineRenderer bodyrend;
    Vector3[] bodyverts;

    bool jumping;
    bool attacking;
    //Vector3 facing;
    // Use this for initialization
    void Start()
    {
        facing = 1;
        rb = GetComponent<Rigidbody>();
        shieldrb = shield.GetComponent<Rigidbody>();
        shieldpos = shieldrb.position - rb.position;

        swordpos = sword.transform.localPosition;
        swordangle = sword.transform.localRotation;
        swordbox = sword.GetComponent<BoxCollider>();
        swordbox.enabled = false;
        swordrb = sword.GetComponent<Rigidbody>();
        bodyrend = GetComponent<LineRenderer>();
        bodyverts = new Vector3[bodyrend.positionCount];
        bodyrend.GetPositions(bodyverts);
        //facing = Vector3.right;
    }

    private void LateUpdate()
    {
        bodyverts[swordvertex] = sword.transform.localPosition - .43f * sword.transform.up;
        if (facing<0) {
            bodyverts[swordvertex][0] += .86f * sword.transform.up[0];
        }
        bodyverts[shieldvertex] = shield.transform.localPosition;
        bodyrend.SetPositions(bodyverts);
    }

    void Update()
    {
        horizaxis = Input.GetAxisRaw("Horizontal") * acceleration * Time.deltaTime;
        vertaxis = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(horizaxis) > 0)
        {
            if (!attacking)
            {
                if (horizaxis > 0)
                {
                    facing = 1;
                    rb.MoveRotation(Quaternion.LookRotation(Vector3.forward));
                }
                else
                {
                    facing = -1;
                    rb.MoveRotation(Quaternion.LookRotation(Vector3.back));
                }
            }
            horizspeed += horizaxis;//Input.GetAxis("Horizontal") * acceleration * Time.deltaTime;

            if (horizspeed > maxspeed) { horizspeed = maxspeed; }
            else if (horizspeed < -maxspeed) { horizspeed = -maxspeed; }
        }
        else
        {
            if (Mathf.Abs(horizspeed) < (acceleration * Time.deltaTime))
            {
                horizspeed = 0f;
            }
            else
            {
                horizspeed -= acceleration * Time.deltaTime * Mathf.Sign(horizspeed);
            }
        }
        rb.MovePosition(rb.position + Vector3.right * horizspeed);

        if (!attacking)
        {
            shieldrb.MovePosition(rb.position + 0.5f * facing * Vector3.right +
                                  Vector3.up * (vertaxis / 2f));
        }

        if (jumping == false && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * maxJumpSpeed, ForceMode.VelocityChange);
            jumping = true;
        }


        if (Input.GetButtonUp("Jump") && rb.velocity[1] > minJumpSpeed)
        {
            rb.AddForce(Vector3.down * (rb.velocity[1] - minJumpSpeed), ForceMode.VelocityChange);
        }

        if (!attacking && Input.GetButtonDown("Fire1"))
        {

            StartCoroutine(Stab(vertaxis));
        }

    }


    IEnumerator Stab (float stabheight) {
        attacking = true;
        swordrb.MoveRotation(Quaternion.Euler(0,0,-90*facing));
        swordrb.MovePosition(rb.position + transform.up * stabheight / 2f);
        //float stabdist=0f;
        float timepassed=0f;
        yield return null;
        while (timepassed<Mathf.Abs(stabTime)) {
            timepassed += Time.deltaTime;
            swordrb.MovePosition(rb.position + transform.up * stabheight / 2f
                                 + sword.transform.up*timepassed * stabSpeed);
            yield return null;
        }
        timepassed = 0f;
        while (timepassed < Mathf.Abs(stabTime))
        {
            timepassed += Time.deltaTime;
            swordrb.MovePosition(rb.position + transform.up * stabheight / 2f
                                 + sword.transform.up * (stabTime-timepassed) * stabSpeed);
            yield return null;
        }
        swordrb.MovePosition(rb.position + swordpos
                             + ((facing<0) ? (-2f*swordpos[0]*Vector3.right) : (Vector3.zero)));
        swordrb.MoveRotation(rb.rotation*swordangle);
        yield return null;
        attacking = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumping = false;
    }
}

