using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class BTMapBoxManager : MonoBehaviour {

    [SerializeField]
    private AbstractMap map;

    [SerializeField]
    private GameObject PolePrefab;

    [SerializeField]
    private GameObject CabinetPrefab;

    [SerializeField]
    private GameObject BillBoardPrefab;

    [SerializeField]
    private GameObject NodesPrefab;

    [SerializeField]
    private GameObject SignPrefab;

    [SerializeField]
    private BTRestService BTService;

    [SerializeField]
    private float RefreshInSeconds = 20f;

    private List<BTCleanPOI> POIOnMap;
    private float CheckCountdown;
    private short NewPointsAdded;

    public List<BTCleanPOI> POI
    {
        get { return POIOnMap; }
    }
    private void Awake()
    {
        Assert.IsNotNull(map);
        Assert.IsNotNull(BTService);
        Assert.IsNotNull(PolePrefab);
        Assert.IsNotNull(CabinetPrefab);
        Assert.IsNotNull(BillBoardPrefab);
        Assert.IsNotNull(SignPrefab);
        Assert.IsNotNull(NodesPrefab);
        POIOnMap = new List<BTCleanPOI>();
        CheckCountdown = RefreshInSeconds;
        NewPointsAdded = 0;
    }

    private void Start()
    {
       // InsertManagerPOI();
    }

    private void InsertManagerPOI()
    {
        if (BTPOIManager.Instance.ListPoints.Count == 0) return;
        // if manager has already points, insert them to the map
        if (BTService.ListPoints.Count > 0) return;

        foreach (BTCleanPOI pp in BTPOIManager.Instance.ListPoints)
        {
            BTCleanPOI bnew = new BTCleanPOI() { ID = pp.ID, Longitude = pp.Longitude, Latitude = pp.Latitude, LastUpdate = pp.LastUpdate, POIType = pp.POIType };
            if (pp.Address != null)
                bnew.Address = bnew.Address;
            AddPointToMap(bnew);
            POIOnMap.Add(bnew);
        }
        // activate 
        //if (map.gameObject.activeSelf == false)
        //{
        //    LogManager.Instance.AddLog("Map is now Activated! Should display POIs!");
        //    map.gameObject.SetActive(true);
        //}
        //else
        //{
        //    LogManager.Instance.AddLog("Map is Reset! Should display POIs!");
        //    Vector2d vv = new Vector2d(GPSData.Instance.Latitude, GPSData.Instance.Longitude);
        //    //map.Initialize(vv, map.AbsoluteZoom);
        //    map.ResetMap();
        //   // map.UpdateMap(vv, map.Zoom);
        //}
    }


    // Update is called once per frame
    void Update ()
    {
        CheckCountdown -= Time.deltaTime;
        if (CheckCountdown <= 0)
        {
            CheckCountdown = RefreshInSeconds;
            InsertManagerPOI(); // insert once only manager points if exist
            InputPOIToMap();
            if (map.gameObject.activeSelf == false)
            {
                LogManager.Instance.AddLog("Map is Activated! Should display POIs!");
                map.gameObject.SetActive(true);
                NewPointsAdded = 0;
            }
            else
            {
                if (NewPointsAdded >=2)
                {
                    Vector2d vv = new Vector2d(GPSData.Instance.Latitude, GPSData.Instance.Longitude);
                    // map.UpdateMap(vv, map.Zoom);
                    LogManager.Instance.AddLog("New Points Added. Called Map Initialize! Should display POIs!");
                    map.Initialize(vv,map.AbsoluteZoom);
                    NewPointsAdded = 0;
                }
            }
        }
	}

    void InputPOIToMap()
    {
        List<BTCleanPOI> btlist = BTService.ListPoints;
        foreach(BTCleanPOI pp in btlist)
        {
            // check if pp is already on map
            BTCleanPOI found = POIOnMap.Where(x => x.ID == pp.ID).FirstOrDefault();
            if (found != null)
            {
                continue;
            }
            BTCleanPOI bnew = new BTCleanPOI() { ID = pp.ID, Longitude = pp.Longitude, Latitude = pp.Latitude, LastUpdate = pp.LastUpdate, POIType = pp.POIType };
            if (pp.Address != null)
                bnew.Address = bnew.Address;
            LogManager.Instance.AddLog(string.Format("Try to put POI on MAP - {0}", bnew.ID));
            AddPointToMap(bnew);
            POIOnMap.Add(bnew);
            NewPointsAdded += 1;
        }
    }


    // SpawnPrefabAtGeoLocation(GameObject, Vector2d, Action<List<GameObject>>, Boolean, String)
    //  A Vector2d(Latitude Longitude) object
    // https://www.mapbox.com/mapbox-unity-sdk/api/unity/Mapbox.Unity.Map.AbstractMap.html
    private void AddPointToMap(BTCleanPOI bnew)
    {
        GameObject prefab = GetPrefab(bnew.POIType);

        Mapbox.Utils.Vector2d vec = new Mapbox.Utils.Vector2d(bnew.Latitude, bnew.Longitude);
        string sname = string.Format("{0} - {1}", bnew.ID, bnew.POIType);
        map.SpawnPrefabAtGeoLocation(prefab, vec, null, true, sname);

        //if (GameManager.Instance.Setting.CreateWithoutPredefined)
        //{
        //    string sname = string.Format("{0} - {1}", bnew.ID, bnew.POIType);
        //    map.SpawnPrefabAtGeoLocation(prefab, vec, null, true, sname);
        //}
        //else
        //{
        //    if (AddPrefabPOI(vec, bnew) == true)
        //    {
        //        LogManager.Instance.AddLog(string.Format("Success Added in map POI (prefabed) using coordinates - {0}", bnew.ID));
        //    }
        //    else
        //    {
        //        string sname = string.Format("{0} - {1}", bnew.ID, bnew.POIType);
        //        map.SpawnPrefabAtGeoLocation(prefab, vec, null, true, sname);
        //    }
        //}
 

    }


    private GameObject GetPrefab(string POIType)
    {
        GameObject prefab;
        if (string.Compare(POIType, "Pole", true) == 0)
            prefab = PolePrefab;
        else if (string.Compare(POIType, "Cabinet", true) == 0)
            prefab = CabinetPrefab;
        else if (string.Compare(POIType, "BillBoard", true) == 0)
            prefab = BillBoardPrefab;
        else if (string.Compare(POIType, "Sign", true) == 0)
            prefab = SignPrefab;
        else if (string.Compare(POIType, "Node", true) == 0)
            prefab = NodesPrefab;
        else
            prefab = PolePrefab;
        return prefab;
    }

    private void cclback(List<GameObject> obj)
    {
        int count = obj.Count();
    }


    /// <summary>
    /// This is a hack on mapbox points of interest
    /// Scans all entries in PointsOfInterestSublayerList for the map.VectorData
    /// If Finds a List with the Prefab Type, like eg. Pole
    /// Then gets the array of POI (string) array and ads a new entry to the end
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="Name"></param>
    /// <returns></returns>
    private bool AddPrefabPOI(Mapbox.Utils.Vector2d vec,  BTCleanPOI bnew)
    {
        if (map.VectorData == null)
            return false;
        string Name = null;
        if (string.Compare(bnew.POIType, "Pole", true) == 0)
            Name = "Pole";
        else if (string.Compare(bnew.POIType, "Cabinet", true) == 0)
            Name = "Street Cabinet";
        else if (string.Compare(bnew.POIType, "BillBoard", true) == 0)
            Name = "Billboard";
        else if (string.Compare(bnew.POIType, "Sign", true) == 0)
            Name = "Sign";
        else if (string.Compare(bnew.POIType, "Node", true) == 0)
            Name = "Nodes";
        else
            Name = "Pole";
        foreach (var ll in map.VectorData.PointsOfInterestSublayerList)
        {
            
            if(string.Compare(ll.prefabItemName,Name, true) == 0)
            {
                List<string> sdata =  ll.coordinates.ToList();
               string snew =vec.x + ", " + vec.y;
                sdata.Add(snew);
                ll.coordinates = sdata.ToArray();

                return true;
            }
        }
        return false;
    }

    void MultiUpdatePOI()
    {
        string[] POITypes = { "Pole", "Cabinet", "Node", "BillBoard", "Sign" };
        for (int i = 0; i < POITypes.Length; i++)
        {
            List<BTCleanPOI> lcat = BTPOIManager.Instance.GetListByCategory(POITypes[i]);
            if (lcat.Count == 0) continue;

            GameObject prefab = GetPrefab(POITypes[i]);
            Vector2d[] LatLon = new Vector2d[lcat.Count];
            for(int j=0; j < lcat.Count; j++)
            {
                LatLon[j] = new Vector2d(lcat[j].Latitude, lcat[j].Longitude);
                BTCleanPOI bnew = new BTCleanPOI() { ID = lcat[j].ID, Longitude = lcat[j].Longitude, Latitude = lcat[j].Latitude, LastUpdate = lcat[j].LastUpdate, POIType = lcat[j].POIType };
                if(lcat[j].Address != null)
                    bnew.Address = lcat[j].Address;
                POIOnMap.Add(bnew);
            }
            string sname = string.Format("{0} count {1}", lcat.Count, POITypes[i]);
            map.SpawnPrefabAtGeoLocation(prefab, LatLon, cclback, true, sname);
        }
    }
}
