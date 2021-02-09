using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public class SpawnAllCategories : MonoBehaviour {

    [SerializeField]
    AbstractMap _map;

    [SerializeField]
    POIPrefabsByScore[] ScorePrefabs;

    [SerializeField]
    string[] PoiTypes;

    [SerializeField]
    float _spawnScale = 100f;


    private List<GameObject> _spawnedObjects;
    private List<Vector2d> _locations;

    private void Awake()
    {
        Assert.IsNotNull(_map);
        Assert.IsNotNull(ScorePrefabs);
        Assert.IsNotNull(PoiTypes);
        Assert.AreEqual(PoiTypes.Length, ScorePrefabs.Length, "Prefabs Array must have the same number of elements as POIType array!");
        _locations = new List<Vector2d>();
        _spawnedObjects = new List<GameObject>();
    }

        // Use this for initialization
    void Start ()
    {
		for(int i=0; i < PoiTypes.Length; i++)
        {
            List<BTCleanPOI> lcat = BTPOIManager.Instance.GetListByCategory(PoiTypes[i]);
            foreach (var pp in lcat)
            {
                Vector2d vv = new Vector2d(pp.Latitude, pp.Longitude);
                _locations.Add(vv);
                GameObject prefab = ScorePrefabs[i].GetPrefabByScore(pp.Score);
                var instance = Instantiate(prefab);
                instance.transform.localPosition = _map.GeoToWorldPosition(vv, true);
                instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                instance.tag = pp.POIType;
                instance.name = pp.ID.ToString();
                _spawnedObjects.Add(instance);
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < _spawnedObjects.Count; i++)
        {
            var spawnedObject = _spawnedObjects[i];
            var location = _locations[i];
            spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
            spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
        }
// add one poi for debug reasons with right mouse click
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(1))
        {
            var mousePosScreen = Input.mousePosition;
            //assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
            //http://answers.unity3d.com/answers/599100/view.html
            mousePosScreen.z = Camera.main.transform.localPosition.y;
            var pos = Camera.main.ScreenToWorldPoint(mousePosScreen);

            var latlongDelta = _map.WorldToGeoPosition(pos);
            Debug.Log("Latitude: " + latlongDelta.x + " Longitude: " + latlongDelta.y);

            _locations.Add(latlongDelta);
            GameObject prefab = ScorePrefabs[0].GetPrefabByScore(null);
            var instance = Instantiate(prefab);
            instance.transform.localPosition = _map.GeoToWorldPosition(latlongDelta, true);
            instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            instance.tag = "Pole";
            instance.name = "1600";
            _spawnedObjects.Add(instance);
        }
#endif
    }
}
