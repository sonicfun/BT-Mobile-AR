using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DebugPOICreate : MonoBehaviour {


    [SerializeField]
    private Button btncreatePOI;


    [HideInInspector]
    public BTRestService BTService;

    private void Awake()
    {
        Assert.IsNotNull(btncreatePOI);
        btncreatePOI.onClick.AddListener(delegate { buttonclicked(); });
    }

    private void buttonclicked()
    {
        if (GPSData.Instance.Ready == false) return;
        BTCleanPOI pp = CreateDebugPOI();
        BTService.ListPoints.Add(pp); // and debug poi to Service List
        LogManager.Instance.AddLog(string.Format("Added New POI: {0} at current location", pp.ID));
    }

   
    BTCleanPOI CreateDebugPOI()
    {
        BTCleanPOI pp = new BTCleanPOI();
        pp.ID = GameManager.Instance.GetNextCounterValue();
        int type = pp.ID % 3;
        if(type == 0)
            pp.POIType = "Pole";
        else if(type == 1)
            pp.POIType = "Cabinet";
        else if(type == 2)
            pp.POIType = "Node";
        pp.Longitude = GPSData.Instance.Longitude;
        pp.Latitude = GPSData.Instance.Latitude;
        pp.LastUpdate = DateTime.Now;
        return pp;
    }
}
