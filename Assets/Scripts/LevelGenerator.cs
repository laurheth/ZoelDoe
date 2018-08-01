using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public TextAsset LevelToLoad;
    public GameObject linkobj;
    public ObjLegend[] ObjectLegend;
    List<ObjLegend> SymbolLegend;
    public Camera cam;
    CameraScript cameraScript;
    XmlDocument dunxml;
    XmlNodeList mapnodes;
	// Use this for initialization
	void Awake () {
        //defaultbg = new Color(0,0,0,0);
        SymbolLegend = new List<ObjLegend>();
        cameraScript = cam.GetComponent<CameraScript>();
        LoadDungeon(LevelToLoad);
	}

    void LoadDungeon(TextAsset dungeon) {
        dunxml = new XmlDocument();
        dunxml.LoadXml(dungeon.text);
        XmlNodeList legendnodes = dunxml.GetElementsByTagName("tile");
        //SymbolLegend = new ObjLegend[legendnodes.Count];
        SymbolLegend.Clear();
        foreach (XmlNode tiletype in legendnodes)
        {
            ObjLegend newtile = new ObjLegend();
            foreach (XmlNode tileelement in tiletype.ChildNodes)
            {
                if (tileelement.Name == "symbol")
                {
                    newtile.ObjectName = tileelement.InnerText;
                    //Debug.Log(tileelement.InnerText);
                }
                else if (tileelement.Name == "isobject")
                {
                    newtile.prefab = SymbolToObject(tileelement.InnerText, ObjectLegend);
                    //Debug.Log(newtile.prefab);
                    //Debug.Log(tileelement.InnerText);
                }
            }
            SymbolLegend.Add(newtile);
        }
        mapnodes = dunxml.GetElementsByTagName("screen");
        LoadScreen(mapnodes[0].Attributes["id"].Value);
    }

    void LoadScreen(string mapid="screen1") {
        
        int k;
        int minx = -200;
        int maxx = 200;
        int minz = -200;
        int maxz = 200;
        //XmlNode mapnode = dunxml.GetElementById("screen0");
        foreach (XmlNode map in mapnodes) {
            if (map.Attributes["id"].Value == mapid) {
                //Debug.Log(map.InnerXml);
                foreach (XmlNode mapdetail in map.ChildNodes) {
                    if (mapdetail.Name=="grid") {
                        k = int.Parse(mapdetail.Attributes["zpos"].Value);
                        string[] grid=((mapdetail.InnerText.Replace(" ","")).Trim()).Split('\n');

                        minz = -grid.Length+1;
                        maxz = 0;
                        minx = 0;
                        maxx = grid[0].Length-1;
                        int linknumber;
                        for (int i = 0; i < grid.Length;i++) {
                            Debug.Log(grid[i]);
                            for (int j = 0; j < grid[i].Length;j++) {
                                if (grid[i].Length-1 > maxx) { maxx = grid[i].Length-1; }

                                if (int.TryParse(grid[i][j].ToString(),out linknumber)) {
                                    Instantiate(linkobj, new Vector3(j, -i, k), Quaternion.identity, transform);
                                }

                                GameObject nextobj = SymbolToObject(grid[i][j].ToString(), SymbolLegend.ToArray());
                                if (nextobj != null) {
                                    Instantiate(nextobj, new Vector3(j,-i,k),Quaternion.identity,transform);
                                }
                            }
                        }
                    }
                }
                cameraScript.SetBoundaries(minx, maxx, minz, maxz);

            }

        }
        //Debug.Log(mapnode.InnerXml);
    }

    GameObject SymbolToObject(string Symbol,ObjLegend[] legends) {
        GameObject toreturn = null;
        foreach (ObjLegend symLegend in legends)
        {
            //Debug.Log(colorprefab.color);
            if (Symbol.Contains(symLegend.ObjectName))
            {
                //Instantiate(symLegend.prefab, new Vector3(i, j, k),
                //            Quaternion.identity, transform);
                toreturn=symLegend.prefab;
            }
        }
        return toreturn;
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
