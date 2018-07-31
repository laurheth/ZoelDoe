using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public TextAsset LevelToLoad;
    public ObjLegend[] ObjectLegend;
    ObjLegend[] SymbolLegend;
	// Use this for initialization
	void Awake () {
        //defaultbg = new Color(0,0,0,0);
        GenerateMap();
	}

    void GenerateMap() {


    }

    void PixelToObject(string Symbol,int i, int j, int k) {
        foreach (ObjLegend symLegend in SymbolLegend)
        {
            //Debug.Log(colorprefab.color);
            if (Symbol.Contains(symLegend.ObjectName))
            {
                Instantiate(symLegend.prefab, new Vector3(i, j, k),
                            Quaternion.identity, transform);
            }
        }
    }

    /*
    [System.Serializable]
    public class ColorPrefab {
        public Color color;
        public GameObject prefab;
        public bool isObject;
    }*/

    [System.Serializable]
    public class ObjLegend {
        public string ObjectName;
        public GameObject prefab;
    }
	
}
