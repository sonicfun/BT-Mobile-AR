using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpawnAR : MonoBehaviour {

    [SerializeField]
    AbstractMap _map;

    [SerializeField]
    GameObject[] POIPrefabs;

    [SerializeField]
    string[] PoiTypes;

    [SerializeField]
    float _spawnScale = 100f;

    [SerializeField]
    float RefreshInSeconds = 60f;

    private float CheckCountdown;

    private List<GameObject> _spawnedObjects;
    private List<Vector2d> _locations;
    private List<BTCleanPOI> ListPoiInScene;


    private void Awake()
    {
        Assert.IsNotNull(_map);
        Assert.IsNotNull(POIPrefabs);
        Assert.IsNotNull(PoiTypes);
        Assert.AreEqual(PoiTypes.Length, POIPrefabs.Length, "Prefabs Array must have the same number of elements as POIType array!");
        _locations = new List<Vector2d>();
        _spawnedObjects = new List<GameObject>();
        ListPoiInScene = new List<BTCleanPOI>();
    }



    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < PoiTypes.Length; i++)
        {
            List<BTCleanPOI> lcat = BTPOIManager.Instance.GetListByCategory(PoiTypes[i]);
            ListPoiInScene.AddRange(lcat);
           // CreateInitialPoi(lcat, POIPrefabs[i], PoiTypes[i]);
            SpawnInitialPOI(lcat, POIPrefabs[i]);
        }
       
    }

    private void SpawnInitialPOI(List<BTCleanPOI> lcat, GameObject pref)
    {
        foreach (var pp in lcat)
        {
            SpawnPrefab(pp, pref);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        CheckForNewPOI();
        RefreshSpawnPOI();
    }

    private void CheckForNewPOI()
    {
        CheckCountdown -= Time.deltaTime;
        if (CheckCountdown >= 0) return;
        CheckCountdown = RefreshInSeconds;
        foreach(var pp in BTPOIManager.Instance.ListPoints)
        {
           var found =  ListPoiInScene.Where(x => x.ID == pp.ID).FirstOrDefault();
           if (found != null)
                continue;
           InsertNewSpawnPOI(found);
        }
    }

    private void InsertNewSpawnPOI(BTCleanPOI found)
    {
       int index = -1;
       if (FindPOITypeIndex(found, out index) == false)
            return;
        SpawnPrefab(found, POIPrefabs[index]);
    }

    private void SpawnPrefab(BTCleanPOI pp, GameObject prefab)
    {
        Vector2d vv = new Vector2d(pp.Latitude, pp.Longitude);
        _locations.Add(vv);
        var instance = Instantiate(prefab);
        instance.transform.localPosition = _map.GeoToWorldPosition(vv, true);
        instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
        instance.tag = pp.POIType;
        instance.name = pp.ID.ToString();
        _spawnedObjects.Add(instance);
    }
    private bool FindPOITypeIndex(BTCleanPOI found, out int index)
    {
        index = -1;
        for (int i = 0; i < PoiTypes.Length; i++)
        {
            if (string.Compare(PoiTypes[i], found.POIType, true) == 0)
            {
                index = i;
                break;
            }
        }
        if (index >= 0)
            return true;
        else
            return false;
    }
    private void RefreshSpawnPOI()
    {
        for (int i = 0; i < _spawnedObjects.Count; i++)
        {
            var spawnedObject = _spawnedObjects[i];
            var location = _locations[i];
            spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
            spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
        }
    }
}
