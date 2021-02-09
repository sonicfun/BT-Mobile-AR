using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShowGPSInfo : MonoBehaviour {

    [SerializeField]
    float RefreshInSeconds = 10f;

    private float CheckCountdown;

    // Use this for initialization
    void Start () {
        CheckCountdown = RefreshInSeconds;

    }
	
	// Update is called once per frame
	void Update () {
        CheckCountdown -= Time.deltaTime;
        if (CheckCountdown >= 0) return;
        CheckCountdown = RefreshInSeconds;
        ShowGPS();
    }

    private void ShowGPS()
    {
        string ss = string.Format("Current GPS Lat={0} Lon={1} speed={2}\n", GPSData.Instance.Latitude, GPSData.Instance.Longitude, GPSData.Instance.Speed);
        LogManager.Instance.AddLog(ss);
    }
}
