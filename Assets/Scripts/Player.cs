using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public GameObject shield;
    //public GameObject sword;
    //public GameObject statCanvasObj;
    public float acceleration;
    public float maxspeed;
    public float minJumpSpeed;
    public float maxJumpSpeed;
    //public float stabTime;
    //public float stabSpeed;
    //public int swordvertex;
    //public int shieldvertex;
    public float shieldSpeed;
    public float terminalVelocity;
    public int hp;
    int numkeys;
    float horizspeed;
    float horizaxis;
    float vertaxis;
    float currentShieldHeight;
    public float crouchThreshold;
    float facing;
    bool dead;
    bool nocontrol;
    Rigidbody rb;
    Rigidbody shieldrb;
    Rigidbody swordrb;
    Vector3 shieldpos;
    Vector3 swordpos;
    Quaternion swordangle;
    BoxCollider swordbox;
    //LineRenderer bodyrend;
    Vector3[] bodyverts;
    PlayerStatCanvas statCanvas;
    SwordAndShieldUser limbScript;
    Vector2 inputvector;

    bool jumping;
    bool attacking;
    //Vector3 facing;
    // Use this for initialization
    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length > 1)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        inputvector = Vector2.zero;
        dead = false;
        facing = 1;
        rb = GetComponent<Rigidbody>();
        //shieldrb = shield.GetComponent<Rigidbody>();
        //shieldpos = shieldrb.position - rb.position;

        //swordpos = sword.transform.localPosition;
        //swordangle = sword.transform.localRotation;
        //swordbox = sword.GetComponent<BoxCollider>();
        //swordbox.enabled = false;
        //swordrb = sword.GetComponent<Rigidbody>();
        //bodyrend = GetComponent<LineRenderer>();
        //bodyverts = new Vector3[bodyrend.positionCount];
        //bodyrend.GetPositions(bodyverts);
        //facing = Vector3.right;
        transform.parent = null;
        nocontrol = false;
        //transform.position += Vector3.up;// * 0.5f;
        //statCanvas = statCanvasObj.GetComponent<PlayerStatCanvas>();
        limbScript = GetComponent<SwordAndShieldUser>();
        limbScript.SetSpeed(maxspeed);
        limbScript.SetSpeedFraction(0f);
        //limbScript.facing = -1;
    }

    /*private void LateUpdate()
    {
        // Set vertex positions to be attached to hilt of sword and centre of shield.
        bodyverts[swordvertex] = sword.transform.localPosition - .43f * sword.transform.up;
        if (facing < 0)
        {
            bodyverts[swordvertex][0] += .86f * sword.transform.up[0];
        }
        bodyverts[shieldvertex] = shield.transform.localPosition;
        bodyrend.SetPositions(bodyverts);
    }*/

    private void FixedUpdate()
    {
        if (rb.velocity[1] < -terminalVelocity)
        {
            rb.velocity = new Vector3(rb.velocity[0], -terminalVelocity, rb.velocity[2]);
        }
        // Movement using above horizspeed.
        if (!nocontrol)
        {
            rb.MovePosition(rb.position + Vector3.right * horizspeed * Time.fixedDeltaTime);
        }
    }


    void Update()
    {
        if (dead)
        {
            return;
        }
        // inputs

        horizaxis = Input.GetAxisRaw("Horizontal");// * acceleration * Time.deltaTime;
        vertaxis = Input.GetAxisRaw("Vertical");
        inputvector[0] = horizaxis;
        inputvector[1] = vertaxis;
        if (inputvector.sqrMagnitude>1) {
            horizaxis = inputvector.normalized[0];
            vertaxis= inputvector.normalized[1];
        }

        vertaxis=vertaxis*0.8f+0.5f;
        if (Mathf.Abs(horizaxis) > 0)
        {
            if (!attacking)
            {
                // change facing, but only if not mid-attack
                if (horizaxis > 0)
                {
                    facing = 1;
                    rb.MoveRotation(Quaternion.LookRotation(Vector3.back));
                }
                else
                {
                    facing = -1;
                    rb.MoveRotation(Quaternion.LookRotation(Vector3.forward));
                }
            }
            // Accelerate!
            //horizspeed += horizaxis;//Input.GetAxis("Horizontal") * acceleration * Time.deltaTime;
            horizspeed = Mathf.Lerp(horizspeed, horizaxis * maxspeed,(acceleration/maxspeed)*Time.deltaTime);

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

        limbScript.SetSpeedFraction(horizspeed);

        // Move shield based on vertical axis, but only if not attacking
        if (!attacking)
        {
            currentShieldHeight = vertaxis;
            // Move shield smoothly
            /*if (Mathf.Abs(vertaxis - currentShieldHeight) > shieldSpeed * Time.deltaTime)
            {
                currentShieldHeight += shieldSpeed *
                    Time.deltaTime * Mathf.Sign(vertaxis - currentShieldHeight);
            }
            else
            {
                currentShieldHeight = vertaxis;
            }*/
            // Set shield height here
            /*shieldrb.MovePosition(rb.position + 0.5f * facing * Vector3.right +
                                  Vector3.up * (currentShieldHeight / 2f));*/
            limbScript.SetShieldHeight(currentShieldHeight);
            if (currentShieldHeight<crouchThreshold) {
                limbScript.SetCrouch(0.5f);
            }
            else {
                limbScript.SetCrouch(1f);
            }
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
            limbScript.Stab(currentShieldHeight,false);
            //StartCoroutine(Stab(currentShieldHeight));

        }

    }



    // Stab animation
    /*IEnumerator Stab(float stabheight)
    {
        swordbox.enabled = true; // turn on swordbox to do damage
        attacking = true;
        // point forward and get ready to stab!
        swordrb.MoveRotation(Quaternion.Euler(0, 0, -90 * facing));
        swordrb.MovePosition(rb.position + transform.up * stabheight / 2f);

        float timepassed = 0f;
        yield return null;

        // Move sword out
        while (timepassed < Mathf.Abs(stabTime))
        {
            timepassed += Time.deltaTime;
            swordrb.MovePosition(rb.position + transform.up * stabheight / 2f
                                 + sword.transform.up * timepassed * stabSpeed);
            yield return null;
        }
        timepassed = 0f;

        // Move sword back in
        while (timepassed < Mathf.Abs(stabTime))
        {
            timepassed += Time.deltaTime;
            swordrb.MovePosition(rb.position + transform.up * stabheight / 2f
                                 + sword.transform.up * (stabTime - timepassed) * stabSpeed);
            yield return null;
        }

        // Return to rest position.
        swordrb.MovePosition(rb.position + swordpos
                             + ((facing < 0) ? (-2f * swordpos[0] * Vector3.right) : (Vector3.zero)));
        swordrb.MoveRotation(rb.rotation * swordangle);
        yield return null;

        // No longer attacking
        attacking = false;
        swordbox.enabled = false;
    }*/

    // No jumping on collision.
    // Doubles for allowing walljumps in the future? Maybe?

    private void OnCollisionEnter(Collision collision)
    {
        jumping = false;
        if (collision.gameObject.tag=="MonsterSword") {
            damage(collision.gameObject.GetComponent<sword>().damage);
        }
    }

    public void damage(int dmg)
    {
        hp -= dmg;
        statCanvas.SetHearts(hp);
        if (hp <= 0)
        {
            dead = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public void MoveOne(Vector3 stepdist,bool justfall=false)
    {
        Debug.Log(stepdist);
        horizspeed = 0;
        StartCoroutine(movecoroutine(stepdist,justfall));
    }

    public IEnumerator movecoroutine (Vector3 stepdist,bool justfall=false,float multiplier=2f) {
        rb.isKinematic = true;
        int breaker = 0;
        nocontrol = true;
        //rb.MovePosition(rb.position - stepdist);
        float linkspeed = (justfall ? terminalVelocity : maxspeed);
        limbScript.SetSpeedFraction(linkspeed);
        float sethorizspeed=maxspeed * (stepdist[0]);
        yield return null;
        stepdist *= multiplier;
        //float stepsize=stepdist/
        while (!(Mathf.Approximately(stepdist.magnitude, 0f)) && breaker < 100)
        {
            rb.MovePosition(rb.position + stepdist.normalized * linkspeed * Time.deltaTime);
            if (Mathf.Abs(stepdist.magnitude) > Mathf.Abs(linkspeed * Time.deltaTime))
            {
                stepdist -= stepdist.normalized * linkspeed * Time.deltaTime;
            }
            else {
                stepdist = Vector3.zero;
            }
            //Debug.Log(stepdist);
            breaker++;
            yield return null;
        }
        horizspeed = sethorizspeed;
        limbScript.SetSpeedFraction(sethorizspeed);
        nocontrol = false;
        rb.isKinematic = false;
    }

    public void AddStatCanvas(PlayerStatCanvas psc) {
        statCanvas = psc;
        statCanvas.SetHearts(hp);
        statCanvas.SetKeys(numkeys);
    }

    public void GetKey(int adjust=1) {
        numkeys += adjust;
        statCanvas.SetKeys(numkeys);
    }

    public void GetItem(Item.ItemType itemType, int quantity) {
        switch (itemType) {
            case Item.ItemType.Key:
                GetKey(1);
                break;
            default:
                break;
        }
    }

}

