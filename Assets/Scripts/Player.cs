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
    public float shieldSpeed;
    public float terminalVelocity;
    float horizspeed;
    float horizaxis;
    float vertaxis;
    float currentShieldHeight;
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
        // Set vertex positions to be attached to hilt of sword and centre of shield.
        bodyverts[swordvertex] = sword.transform.localPosition - .43f * sword.transform.up;
        if (facing<0) {
            bodyverts[swordvertex][0] += .86f * sword.transform.up[0];
        }
        bodyverts[shieldvertex] = shield.transform.localPosition;
        bodyrend.SetPositions(bodyverts);
    }

    private void FixedUpdate()
    {
        if (rb.velocity[1] < -terminalVelocity) { 
            rb.velocity = new Vector3(rb.velocity[0],-terminalVelocity,rb.velocity[2]);
        }
    }

    void Update()
    {
        // inputs
        horizaxis = Input.GetAxisRaw("Horizontal") * acceleration * Time.deltaTime;
        vertaxis = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(horizaxis) > 0)
        {
            if (!attacking)
            {
                // change facing, but only if not mid-attack
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
            // Accelerate!
            horizspeed += horizaxis;//Input.GetAxis("Horizontal") * acceleration * Time.deltaTime;

            if (horizspeed > maxspeed) { horizspeed = maxspeed; }
            else if (horizspeed < -maxspeed) { horizspeed = -maxspeed; }
        }
        else
        {
            // No input, slow down!
            if (Mathf.Abs(horizspeed) < (acceleration * Time.deltaTime))
            {
                horizspeed = 0f;
            }
            else
            {
                horizspeed -= acceleration * Time.deltaTime * Mathf.Sign(horizspeed);
            }
        }
        // Movement using above horizspeed.
        rb.MovePosition(rb.position + Vector3.right * horizspeed);

        // Move shield based on vertical axis, but only if not attacking
        if (!attacking)
        {
            // Move shield smoothly
            if (Mathf.Abs(vertaxis-currentShieldHeight) > shieldSpeed*Time.deltaTime)
            {
                currentShieldHeight += shieldSpeed *
                    Time.deltaTime * Mathf.Sign(vertaxis - currentShieldHeight);
            }
            else {
                currentShieldHeight = vertaxis;
            }
            // Set shield height here
            shieldrb.MovePosition(rb.position + 0.5f * facing * Vector3.right +
                                  Vector3.up * (currentShieldHeight/2f));
        }

        // If not jumping already, do a jump!
        if (jumping == false && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * maxJumpSpeed, ForceMode.VelocityChange);
            jumping = true;
        }

        // Slow down jump on release. Allows variable jump height.
        if (Input.GetButtonUp("Jump") && rb.velocity[1] > minJumpSpeed)
        {
            rb.AddForce(Vector3.down * (rb.velocity[1] - minJumpSpeed), ForceMode.VelocityChange);
        }

        // Attack!
        if (!attacking && Input.GetButtonDown("Fire1"))
        {

            StartCoroutine(Stab(currentShieldHeight));
        }

    }



    // Stab animation
    IEnumerator Stab (float stabheight) {
        swordbox.enabled = true; // turn on swordbox to do damage
        attacking = true;
        // point forward and get ready to stab!
        swordrb.MoveRotation(Quaternion.Euler(0,0,-90*facing));
        swordrb.MovePosition(rb.position + transform.up * stabheight / 2f);
        
        float timepassed=0f;
        yield return null;

        // Move sword out
        while (timepassed<Mathf.Abs(stabTime)) {
            timepassed += Time.deltaTime;
            swordrb.MovePosition(rb.position + transform.up * stabheight / 2f
                                 + sword.transform.up*timepassed * stabSpeed);
            yield return null;
        }
        timepassed = 0f;

        // Move sword back in
        while (timepassed < Mathf.Abs(stabTime))
        {
            timepassed += Time.deltaTime;
            swordrb.MovePosition(rb.position + transform.up * stabheight / 2f
                                 + sword.transform.up * (stabTime-timepassed) * stabSpeed);
            yield return null;
        }

        // Return to rest position.
        swordrb.MovePosition(rb.position + swordpos
                             + ((facing<0) ? (-2f*swordpos[0]*Vector3.right) : (Vector3.zero)));
        swordrb.MoveRotation(rb.rotation*swordangle);
        yield return null;

        // No longer attacking
        attacking = false;
        swordbox.enabled = false;
    }

    // No jumping on collision.
    // Doubles for allowing walljumps in the future? Maybe?
    private void OnCollisionEnter(Collision collision)
    {
        jumping = false;
    }
}

