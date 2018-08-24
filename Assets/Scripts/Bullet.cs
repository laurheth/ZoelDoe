using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="Player") {
            collision.gameObject.GetComponent<Player>().damage(damage);
        }

        Destroy(gameObject);

    }
}
