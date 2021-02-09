using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// This class performs a Physics Raycast, to find the gameobject under the mouse / thumb
/// If a valid game object is found (POI), information is displayed using the CanvasShowInfo class
/// </summary>
public class ARShowInfo : MonoBehaviour {

    [SerializeField]
    private AbstractMap map;

    [SerializeField]
    private GameObject PrefabInfo;

    [SerializeField]
    private bool UseNameToLocatePOI;

    private CanvasShowInfo cinfo;

    private void Awake()
    {
        Assert.IsNotNull(map);
        Assert.IsNotNull(PrefabInfo);
        var instance = Instantiate(PrefabInfo);
        cinfo = instance.GetComponent<CanvasShowInfo>();
    }


    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (cinfo.IsVisible()) return;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Pole" || hit.transform.tag == "Node" || hit.transform.tag == "Cabinet" || hit.transform.tag == "Sign")
                {
                    string sname = hit.transform.name;
                    LogManager.Instance.AddLog(string.Format("Hit AR POI found. Tag = {0} name={1}", hit.transform.tag, sname));
                    if(UseNameToLocatePOI)
                    {
                        LocatePOIByName(hit.transform);
                    }
                    else
                    {
                        LocatePOIByPosition(hit.transform);
                    }
                }
                else if (hit.transform.tag == "Player")
                {
                    cinfo.DisplayCustomText("This is Your Position!");
                }
            }
       }
    }

 
    private void LocatePOIByName(Transform transform)
    {
        string sID = transform.name;
        int ID = 0;
        // Node is a composite model, so take parent name
        sID = transform.name;
        if (int.TryParse(sID, out ID) == false)
            return;
        List<BTCleanPOI> lpoi = BTPOIManager.Instance.ListPoints;
        BTCleanPOI pfound = lpoi.Where(x => x.ID == ID).FirstOrDefault();
        if (pfound != null)
            cinfo.DisplayCustomText(pfound.GetPoiDescriptionByScore());
    }

    private void LocatePOIByPosition(Transform transform)
    {
        Transform pos = transform;
        // Node is a composite model, so take parent
        string stype = transform.tag;
        Vector3 vv = pos.localPosition;
        //convert to latlong
        var latlong = map.WorldToGeoPosition(vv);
        List<BTCleanPOI> lpoi = BTPOIManager.Instance.ListPoints;

        BTCleanPOI pfound = null;
        double mindistance = 0;
        foreach (var pp in lpoi)
        {
             if (string.Compare(pp.POIType, stype, true) != 0)
                continue;
            Vector2d vpp = new Vector2d(pp.Latitude, pp.Longitude);
            if (pfound == null)
            {
                pfound = pp;
                mindistance = Vector2d.Distance(latlong, vpp);
            }
            else
            {
                double distance = Vector2d.Distance(latlong, vpp);
                if (distance < mindistance)
                {
                    pfound = pp;
                    mindistance = distance;
                }
            }
        }

        if(pfound != null)
        {
            LogManager.Instance.AddLog(string.Format("Found POI to show: {0}",pfound.ID));
            cinfo.DisplayCustomText(pfound.GetPoiDescriptionByScore());
        }
        else
        {
            LogManager.Instance.AddLog(string.Format("Didn't Found POI to show for tag: {0}", stype));
        }
    }


}
