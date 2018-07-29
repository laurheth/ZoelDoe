using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public Texture2D LevelToLoad; 
    public ColorPrefab[] colorprefabs;
	// Use this for initialization
	void Awake () {
        GenerateMap();
	}

    void GenerateMap() {

        Color pixelcolor;
        //int k;
        for (int i = 0; i < LevelToLoad.width;i++) {
            for (int j = 0; j < LevelToLoad.height;j++) {
                //k = 0;
                pixelcolor = LevelToLoad.GetPixel(i, j);
                if (pixelcolor.a<0.001f) { continue; }
                Debug.Log(pixelcolor);
                foreach (ColorPrefab colorprefab in colorprefabs) {
                    //Debug.Log(colorprefab.color);
                    if (Mathf.Approximately(pixelcolor.r,colorprefab.color.r) &&
                        Mathf.Approximately(pixelcolor.g, colorprefab.color.g) &&
                        Mathf.Approximately(pixelcolor.b, colorprefab.color.b)) {
                        Instantiate(colorprefab.prefab,new Vector3(i,j,0),
                                    Quaternion.identity,transform);
                    }
                }

            }
        }
    }

    [System.Serializable]
    public class ColorPrefab {
        public Color color;
        public GameObject prefab;
    }
	
}
