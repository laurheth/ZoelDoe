using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleFloor : MonoBehaviour
{
    public float shakeAmplitude;
    public float shakeFreq;
    public float shakeDuration;
    bool activated;
    Transform childObj;
    private void Awake()
    {
        activated = false;
        childObj = transform.Find("CrumbleFloorObj");
        childObj.gameObject.GetComponent<Collider>().enabled = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="Player" && !activated) {
            activated = true;
            StartCoroutine(ShakeAndFall());
        }
    }

    IEnumerator ShakeAndFall() {
        //Transform childObj = transform.Find("CrumbleFloorObj");
        Rigidbody rb = childObj.gameObject.GetComponent<Rigidbody>();
        Vector3 basepos = transform.position;
        float t = 0;
        while (t<shakeDuration) {
            t += Time.deltaTime;
            rb.MovePosition(basepos
                            +Vector3.right * shakeAmplitude
                            *Mathf.Sin( 2*t*shakeFreq*Mathf.PI ));
            yield return null;
        }
        rb.isKinematic = false;
        childObj.gameObject.GetComponent<Collider>().enabled = true;
        childObj.parent = transform.parent;
        rb.velocity = Vector3.zero;
        Destroy(gameObject);
    }
}
