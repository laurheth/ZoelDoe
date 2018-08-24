using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    Color startColor;
    Color endColor;
    Color color;
    //Material material;
    Renderer rend;
    float fadealpha;
    float fadeTime;
    float t;
    //float timefrac;
    float startSize;
    float endSize;
	// Use this for initialization
	void Awake () {
        t = 0f;
        fadealpha = 1f;
        fadeTime = 0.5f;
        startColor = Color.yellow;
        endColor = Color.red;
        color = Color.yellow;
        startSize = 0.1f;
        endSize = 1f;
        //material = GetComponent<Renderer>().material;
        rend = GetComponent<Renderer>();
	}
    public void SetSize(float start, float end) {
        startSize = start;
        endSize = end;
    }
	// Update is called once per frame
	void Update () {
        if (t > fadeTime) { Destroy(gameObject); return; }
        t += Time.deltaTime;
        //Vector3.on

        fadealpha = Mathf.Max(fadeTime - t,0);

        transform.localScale=((1-fadealpha)*(endSize-startSize) + startSize ) * Vector3.one;

        color = new Color(Mathf.Lerp(startColor.r, endColor.r, 1 - fadealpha),
                          Mathf.Lerp(startColor.g, endColor.g, 1 - fadealpha),
                          Mathf.Lerp(startColor.b, endColor.b, 1 - fadealpha));
        rend.material.SetColor("_EmissionColor", color);
        rend.material.SetColor("_Color", new Color(0, 0, 0, fadealpha));
	}
}
