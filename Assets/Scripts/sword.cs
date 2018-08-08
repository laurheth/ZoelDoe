using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sword : MonoBehaviour {

    SwordAndShieldUser parentLimbs;

    public int damage;

    private void Start()
    {
        parentLimbs = GetComponentInParent<SwordAndShieldUser>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Sword collision registered");
        if (collision.gameObject.tag=="Shield" || collision.gameObject.tag=="PlayerShield") {
            //Debug.Log("attempt cancel?");
            parentLimbs.CancelStab();
        }
        //else if (collision.gameObject.tag == "Shield")
    }
}
