using Assets.Scripts.BT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Class that calls open street map service, to get the Reverse GPS data,
/// meaning the real address of a GPS point on earth (address, city, zip code. etc)
/// </summary>
public class ReversePOI: MonoBehaviour
{
    [SerializeField]
    private string url = "https://nominatim.openstreetmap.org/reverse";

    [SerializeField]
    private short zoomlevel = 18;

    [SerializeField]
    private string email;

    // if this property is true, then the Reverse GPS address is stored in SQLite database
    [SerializeField]
    private bool SaveReveseAddressToDatabase;

    public bool ready = false;
    public bool Succes;

    // delegate to call after success reverse gps point
    public delegate void OnSuccessAddressCleanPOI(BTCleanPOI poi, ReverseAddress resultdata);
    private DataService db;

    void Start()
    {
         db = new DataService("fuzzy.db");
    }

    public void ProcessReverseCleanPoint(BTCleanPOI poi, OnSuccessAddressCleanPOI callback)
    {
        // if  has address already no need to call endpoint
        if(poi.Address != null)
        {
            callback(poi, poi.Address);
            return;
        }
        if (SaveReveseAddressToDatabase)
        {
            // check if exists in local database
            POIAddress ad = db.GetPoiAddressByID(poi.ID);
            if (ad != null) // if record is found, avoid to call open map service
            {
                ReverseAddress rev = POIAddress.GetAddressFromPOI(ad);
                callback(poi, rev);
                return;
            }
        }

        StartCoroutine(GetReverseCleanPOI(poi, callback));
    }

    IEnumerator GetReverseCleanPOI(BTCleanPOI poi, OnSuccessAddressCleanPOI callback)
    {
        if (poi == null)
        {
            ready = false;
            yield return null;
        }
        Succes = false;
        string surl = string.Format("{0}?format=jsonv2&lat={1}&lon={2}&zoom={3}&addressdetails=1", url, GPSPoint.GetDoubleValue(poi.Latitude), GPSPoint.GetDoubleValue(poi.Longitude), zoomlevel);
        // email is necessary, for this service to work as stated in it's web site
        if (string.IsNullOrEmpty(email) == false)
            surl = string.Format("{0}&email={1}", surl, email);

        UnityWebRequest www = UnityWebRequest.Get(surl);

        yield return www.SendWebRequest();
        while (!www.isDone)
            yield return null;
        if (www.isNetworkError || www.isHttpError)
        {
            Succes = false;
            Debug.Log(www.error);
            LogManager.Instance.AddLog(string.Format("Error: Calling Reverse GPS Service: {0}", www.error));
            ready = false;
            yield return null;
        }
        byte[] resultdata = www.downloadHandler.data;
        string POIJSON = System.Text.Encoding.Default.GetString(resultdata);
        ReverseGPS revgps = JsonUtility.FromJson<ReverseGPS>(POIJSON);
        Succes = true;

        ready = true;
        // update flag from global setting
        if(SaveReveseAddressToDatabase != GameManager.Instance.Setting.SaveReversePOIToSQLite)
            SaveReveseAddressToDatabase = GameManager.Instance.Setting.SaveReversePOIToSQLite;
        // now save this to database
        if (SaveReveseAddressToDatabase)
        {
            POIAddress poiad = POIAddress.GetPOIFromAddress(revgps.address, poi);
            try
            {
                if (db != null)
                {
                    // check if POI already exists in SQLite DB
                    POIAddress ad = db.GetPoiAddressByID(poi.ID);
                    if (ad == null)
                    {
                        db.InsertPoiAddress(poiad);
                        LogManager.Instance.AddLog(string.Format("Success insert to SQLite Poi: {0}", poi.ID));
                    }
                }
            }
            catch(Exception ex)
            {
                LogManager.Instance.AddLog(string.Format("Failed insert to SQLite Poi: {0} - {1}", poi.ID, ex.Message));
            }
        }
        // update the caller thru callback
        if (callback != null)
            callback(poi, revgps.address);
    }

}

