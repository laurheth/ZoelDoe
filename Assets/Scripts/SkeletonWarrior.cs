using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonWarrior : Monster {
    public float speed;
    SwordAndShieldUser limbScript;
    int facing;
    bool advancing;
    float playerdist;
    public float shieldT;
    //float shieldHeight;

    public float stopdist;
	// Use this for initialization
	public override void Start () {
        base.Start();
        limbScript = GetComponent<SwordAndShieldUser>();
        limbScript.SetSpeed(speed);
        limbScript.SetSpeedFraction(0f);
        facing = 1;
	}

    private void FixedUpdate()
    {
        if (awakened && advancing)
        {
            rb.MovePosition(rb.position - transform.right * speed*Time.deltaTime);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (awakened)
        {
            playerdist = (playerobj.transform.position.x - transform.position.x);
            if (facing * playerdist > 0)
            {
                facing *= -1;
                transform.rotation *= Quaternion.Euler(0, 180, 0);
            }

            if (Mathf.Abs(playerdist) < stopdist)
            {

                    limbScript.SetSpeedFraction(0f);
                    advancing = false;

            }
            else 
            {
                limbScript.SetSpeedFraction(speed);
                advancing = true;
            }
            //playerShieldRelHeight
            /*shieldHeight = Mathf.Lerp(shieldHeight,
                                    Mathf.Clamp(playerShieldRelHeight, 0, 1), shieldT*Time.deltaTime);*/
            limbScript.SetShieldHeight(Mathf.Clamp(playerShieldRelHeight, 0, 1));

        }
    }
}
