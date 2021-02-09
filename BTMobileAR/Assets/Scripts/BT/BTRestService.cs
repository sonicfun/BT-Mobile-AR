using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
//using Unity.PackageManager;
using UnityEngine;
using UnityEngine.Networking;


public class BTRestService : MonoBehaviour
{
    [SerializeField]
    private string url = "https://ept-location.eu-gb.mybluemix.net/nodes/";
    [SerializeField]
    private string user = "eptloc";
    [SerializeField]
    private string password = "eptl0c";
    [SerializeField]
    private double radius = 15;
    [SerializeField]
    private double latitude = 51.87610908609458;
    [SerializeField]
    private double longitude = 0.9445491291722874;
    [SerializeField]
    private double MininmumDistance = 10; // if movement more than 10 meters
    [SerializeField]
    private float EndPointRefreshInSeconds = 60f;

    public List<BTCleanPOI> ListPoints;

    private float apiCheckCountdown;
    private GPSPoint PrevPoint;
    

    void Start()
    {
        ListPoints = new List<BTCleanPOI>();
        PrevPoint = null;
        apiCheckCountdown = EndPointRefreshInSeconds;
        StartCoroutine(GetPOI());
    }

    void Update()
    {
        apiCheckCountdown -= Time.deltaTime;
        if (apiCheckCountdown <= 0)
        {
            apiCheckCountdown = EndPointRefreshInSeconds;
            if (PrevPoint != null)
            {
                double distance = PrevPoint.CalcDistance(latitude, longitude);
                if (distance < radius / 2 && distance < MininmumDistance) // if no significant movement, ha, no need to call again the End Point and consume internet data
                {
                    Debug.Log("Skip calling EndPoint! No serious movement from last call!");
                    return;
                }
            }

            StartCoroutine(GetPOI());
        }
    }

   IEnumerator GetPOI()
    {
        // Read GPS Position from singleton, using gps sensor of the device
        if(GPSData.Instance.Ready)
        {
            float lat = GPSData.Instance.Latitude;
            float longt = GPSData.Instance.Longitude;
            if(lat != 0 || longt != 0) // if pc with no gps avoid it
            {
                latitude = lat;
                longitude = longt;
            }
        }
        // now apply radius from global settings
        if (GameManager.Instance.Setting.GPSRadius > 0 && radius != GameManager.Instance.Setting.GPSRadius)
            radius = GameManager.Instance.Setting.GPSRadius;

        string surl = string.Format("{0}query?radius={1}&latitude={2}&longitude={3}", url, radius, GPSPoint.GetDoubleValue(latitude), GPSPoint.GetDoubleValue(longitude));
        string authorization = authenticate(user, password);
        UnityWebRequest www = UnityWebRequest.Get(surl);
        www.SetRequestHeader("AUTHORIZATION", authorization);
        www.SetRequestHeader("Content-Type", "application/json");

        // pass control until request is completed
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            LogManager.Instance.AddLog(string.Format("Error calling BT: {0}", www.error));
        }
        else
        {
            Debug.Log("Form upload complete! Status Code: " + www.responseCode);
            byte[] result = www.downloadHandler.data;
            string POIJSON = System.Text.Encoding.Default.GetString(result);
            // prepare JSON to comply with Unity JsonUtility
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{ \"POI\" : ");
            sb.Append(POIJSON);
            sb.Append("}");


            BTPOIInfo info = JsonUtility.FromJson<BTPOIInfo>(sb.ToString());
            ListPoints.Clear();
            foreach(BTPOI p in info.POI)
                {
                    BTCleanPOI cl = new BTCleanPOI(p);
                    ListPoints.Add(cl);
//                    LogManager.Instance.AddLog(string.Format("Added POI: {0}", cl.ID));
                }
    
           // Debug.LogFormat("Uploaded total {0} POI from BT!", ListPoints.Count);
            LogManager.Instance.AddLog(string.Format("POI from BT - Total: {0}", ListPoints.Count));
            // now update BTManager
            BTPOIManager.Instance.ApendListData(ListPoints);
            PrevPoint = new GPSPoint() { latitude = latitude, longitude = longitude };
        }
    }

    private string authenticate(string username, string password)
    {
        string auth = username + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
    }

 

}
