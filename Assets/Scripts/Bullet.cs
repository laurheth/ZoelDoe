using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public int damage;
    public GameObject explosion;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="Player") {
            collision.gameObject.GetComponent<Player>().damage(damage);
        }

        GameObject exp=Instantiate(explosion, transform.position,transform.rotation,transform.parent);
        //exp.GetComponent<Explosion>().SetSize(0.4f, 1);
        Destroy(gameObject);

    }
}
