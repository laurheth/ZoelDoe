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
    Dictionary<string, List<bool>> ObjLog;
    XmlDocument dunxml;
    XmlNodeList mapnodes;
    int centerx;
    string currentmapid;
	// Use this for initialization
	void Awake () {
        ObjLog = new Dictionary<string, List<bool>>();
        centerx = 10;
        //defaultbg = new Color(0,0,0,0);
        SymbolLegend = new List<ObjLegend>();
        cameraScript = cam.GetComponent<CameraScript>();
        cameraScript.SaveOffset();
        LoadDungeon(LevelToLoad);
	}

    void LoadDungeon(TextAsset dungeon) {
        dunxml = new XmlDocument();
        bool trackable;
        dunxml.LoadXml(dungeon.text);
        XmlNodeList legendnodes = dunxml.GetElementsByTagName("tile");
        //SymbolLegend = new ObjLegend[legendnodes.Count];
        SymbolLegend.Clear();
        foreach (XmlNode tiletype in legendnodes)
        {
            ObjLegend newtile = new ObjLegend();
            foreach (XmlNode tileelement in tiletype.ChildNodes)
            {
                trackable = false;
                if (tileelement.Name == "symbol")
                {
                    newtile.ObjectName = tileelement.InnerText;
                    //Debug.Log(tileelement.InnerText);
                }
                else if (tileelement.Name == "isobject")
                {
                    newtile.prefab = SymbolToObject(tileelement.InnerText, ObjectLegend, out trackable);
                    newtile.TrackThis = trackable;
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
        currentmapid = mapid;
        bool trackable;
        bool firstload;
        if (ObjLog.ContainsKey(mapid)) {
            firstload = false;
        }
        else {
            firstload = true;
            ObjLog.Add(mapid, new List<bool>());
        }

        List<bool> thislog = ObjLog[mapid];
        int trackobjid = 0;
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
                                    bool addobj = true;
                                    trackable = false;
                                    nextobj = SymbolToObject(grid[i][j].ToString(), SymbolLegend.ToArray(),out trackable);
                                    if (nextobj != null)
                                    {
                                        
                                        if (trackable) {
                                            if (firstload) {
                                                thislog.Add(true);
                                                //trackobjid++;
                                            }
                                            else {
                                                addobj = thislog[trackobjid];
                                            }
                                            trackobjid++;
                                        }
                                        if (addobj) {
                                            nextobj = Instantiate(nextobj, new Vector3(j, -i, k), Quaternion.identity, transform);
                                            if (trackable) {
                                                if (nextobj.tag=="Monster") {
                                                    nextobj.GetComponent<MurderBirb>().SetID(trackobjid - 1);
                                                }
                                                else if (nextobj.tag=="Item") {
                                                    nextobj.GetComponent<Item>().SetID(trackobjid - 1);
                                                }
                                            }
                                        }
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

    GameObject SymbolToObject(string Symbol,ObjLegend[] legends, out bool trackable) {
        GameObject toreturn = null;
        trackable = false;
        foreach (ObjLegend symLegend in legends)
        {
            //Debug.Log(colorprefab.color);
            if (Symbol.Contains(symLegend.ObjectName))
            {
                //Instantiate(symLegend.prefab, new Vector3(i, j, k),
                //            Quaternion.identity, transform);
                toreturn=symLegend.prefab;
                trackable = symLegend.TrackThis;
            }
        }
        return toreturn;
    }

    public void UnTrack(int tid) {
        ObjLog[currentmapid][tid] = false;
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
        public bool TrackThis;
    }
	
    /*public class ObjectLog
    {
        public string MapID;
        public List<bool> ObjectExists;
        public ObjectLog() {
            ObjectExists = new List<bool>();
        }
    }*/
}
