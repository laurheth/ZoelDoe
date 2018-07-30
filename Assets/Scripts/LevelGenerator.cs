using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    
    public Texture2D LevelToLoad;

    public ColorPrefab[] colorprefabs;
    public ColorPrefab[] monsters;
    Color defaultbg;
	// Use this for initialization
	void Awake () {
        defaultbg = new Color(0,0,0,0);
        GenerateMap();
	}

    void GenerateMap() {

        Color pixelcolor;
        int k;
        for (int i = 0; i < LevelToLoad.width;i++) {
            for (int j = 0; j < LevelToLoad.height;j++) {
                k = 0;
                pixelcolor = LevelToLoad.GetPixel(i, j);
                if (Mathf.Approximately(pixelcolor.a,0f)) { continue; }
                if (pixelcolor.a < 0.8f) { k = 1; }
                Debug.Log(pixelcolor);
                PixelToObject(pixelcolor, i, j, k);

            }
        }
    }

    void PixelToObject(Color pixelcolor,int i, int j, int k) {
        foreach (ColorPrefab colorprefab in colorprefabs)
        {
            //Debug.Log(colorprefab.color);
            if (Mathf.Approximately(pixelcolor.r, colorprefab.color.r) &&
                Mathf.Approximately(pixelcolor.g, colorprefab.color.g) &&
                Mathf.Approximately(pixelcolor.b, colorprefab.color.b))
            {
                Instantiate(colorprefab.prefab, new Vector3(i, j, k),
                            Quaternion.identity, transform);
                if (k > 0 && Mathf.Approximately(defaultbg.a, 0f))
                {
                    defaultbg = colorprefab.color;
                    defaultbg.a = 0.5f;
                }
                if (colorprefab.isObject && !Mathf.Approximately(defaultbg.a,0f))
                {
                    PixelToObject(defaultbg, i, j, 1);
                    //Instantiate(defaultbg, new Vector3(i, j, 1), Quaternion.identity, transform);
                }
            }
        }
    }

    [System.Serializable]
    public class ColorPrefab {
        public Color color;
        public GameObject prefab;
        public bool isObject;
    }
	
}
