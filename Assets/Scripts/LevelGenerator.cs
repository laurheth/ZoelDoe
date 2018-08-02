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
    int centerx;
	// Use this for initialization
	void Awake () {
        centerx = 10;
        //defaultbg = new Color(0,0,0,0);
        SymbolLegend = new List<ObjLegend>();
        cameraScript = cam.GetComponent<CameraScript>();
        cameraScript.SaveOffset();
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
        //cameraScript.jumpToPos();
    }

    void LoadScreen(string mapid="screen1") {
        //mapid = "screen1";
        int k;
        int minx = -200;
        int maxx = 200;
        int minz = -200;
        int maxz = 200;
        //XmlNode mapnode = dunxml.GetElementById("screen0");
        foreach (XmlNode map in mapnodes) {
            if (map.Attributes["id"].Value == mapid) {
                //Debug.Log(map.InnerXml);
                List<string> linktargs = new List<string>();
                List<int> linkxid = new List<int>();
                List<LinkObject> linklist = new List<LinkObject>();
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
                            //Debug.Log(grid[i]);
                            for (int j = 0; j < grid[i].Length;j++) {
                                if (grid[i].Length-1 > maxx) { maxx = grid[i].Length-1; }
                                GameObject nextobj;
                                if (int.TryParse(grid[i][j].ToString(), out linknumber))
                                {
                                    nextobj = Instantiate(linkobj, new Vector3(j, -i, k), Quaternion.identity, transform);
                                    LinkObject newlinkobj = nextobj.GetComponent<LinkObject>();
                                    newlinkobj.currentScreen = mapid;
                                    newlinkobj.levelGenerator = this;
                                    newlinkobj.targetScreen = grid[i][j].ToString();
                                    //newlinkobj.setdirection()
                                    linklist.Add(newlinkobj);
                                    if (linktargs.Contains(grid[i][j].ToString())) {
                                        newlinkobj.xidnum = linkxid[linktargs.IndexOf(grid[i][j].ToString())]+1;
                                        linkxid[linktargs.IndexOf(grid[i][j].ToString())]++;
                                    }
                                    else {
                                        linktargs.Add(grid[i][j].ToString());
                                        linkxid.Add(0);
                                        newlinkobj.xidnum = 0;
                                    }
                                }
                                else
                                {
                                    nextobj = SymbolToObject(grid[i][j].ToString(), SymbolLegend.ToArray());
                                    if (nextobj != null)
                                    {
                                        Instantiate(nextobj, new Vector3(j, -i, k), Quaternion.identity, transform);
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (XmlNode mapdetail in map.ChildNodes)
                {
                    string exitid;
                    string exittarget;
                    if (mapdetail.Name == "exit")
                    {
                        exitid = mapdetail.Attributes["id"].Value;
                        exittarget = mapdetail.Attributes["target"].Value;
                        foreach (LinkObject newlinkobj in linklist) {
                            if (newlinkobj.targetScreen == exitid) {
                                newlinkobj.targetScreen = exittarget;
                                newlinkobj.SetDirection(mapdetail.Attributes["direction"].Value);
                            }
                        }
                    }
                }

                centerx = (maxx + minx) / 2;
                cameraScript.SetBoundaries(minx, maxx, minz, maxz);

            }

        }
        //Debug.Log(mapnode.InnerXml);
    }

    public void SwitchScreen(string targetscreen) {
        foreach (Transform childtrans in transform) {
            Destroy(childtrans.gameObject);
        }
        cameraScript.ResetOffset();
        LoadScreen(targetscreen);
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

    public int DirectionToCentre(Vector3 location) {
        if (location[0] < centerx) { return 1; }
        else { return -1; }
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
