using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class to keep track of GPS location from the Mobile device
/// Also calculates the speed of the device in km / hour
/// </summary>
public class GPSData : Singleton<GPSData>
{
    public bool enableByRequest = true;
    public float RefreshInSeconds = 30f;

    public float Latitude {  get { return _latitude; } }
    public float Longitude { get { return _longitude; } }
    public bool Ready { get { return _ready; } }
    public float Speed {  get { return _speed; } }
    private float _latitude, _longitude;
    private bool _ready = false;
    private int maxWait = 10;
    private float CheckCountdown;
    private float  overallDistance, lastDistance, timer, lastTime, _speed, speed0, acceleration;
    bool firstTime;

    protected GPSData() { } // guarantee this will be always a singleton only - can't use the constructor!
                            // Use this for initialization
    void Start () {
        _speed = 0;
        speed0 = 0;
        timer = 0;
        lastTime = 0;
        CheckCountdown = RefreshInSeconds;
        StartCoroutine(getLocation());
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        CheckCountdown -= Time.deltaTime;
        if (CheckCountdown >= 0) return;
        CheckCountdown = RefreshInSeconds;
        if (_ready == false) return;
        if (_longitude != Input.location.lastData.longitude || _latitude != Input.location.lastData.latitude)
        {
            CalculateDistances(_longitude, _latitude, Input.location.lastData.longitude, Input.location.lastData.latitude);  // last distance and overall distanceS            
            _longitude = Input.location.lastData.longitude;
            _latitude = Input.location.lastData.latitude;

            lastTime = timer;
            timer = 0;

            speed0 = _speed;
            CalculateSpeed();
            CalculateAcceleration();
        }
    }

   IEnumerator getLocation()
    {
        LocationService service = Input.location;
        if (!enableByRequest && !service.isEnabledByUser)
        {
            Debug.Log("Location Services not enabled by user");
            yield break;
        }
        service.Start(1f, 0.1f);
        while (service.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }
        if (service.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
             _latitude = service.lastData.latitude;
            _longitude = service.lastData.longitude;
        }
        //service.Stop();
        _ready = true;
    }

    public void StopGPS()
    {
        if(_ready)
        {
            LocationService service = Input.location;
            service.Stop();
            _ready = false;
            enableByRequest = false;
        }
    }

    public static float Radians(float x)
    {
        return x * Mathf.PI / 180;
    }
    public void CalculateDistances(float firstLon, float firstLat, float secondLon, float secondLat)
    {

        float dlon = Radians(secondLon - firstLon);
        float dlat = Radians(secondLat - firstLat);

        float distance = Mathf.Pow(Mathf.Sin(dlat / 2), 2) + Mathf.Cos(Radians(firstLat)) * Mathf.Cos(Radians(secondLat)) * Mathf.Pow(Mathf.Sin(dlon / 2), 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(distance), Mathf.Sqrt(1 - distance));

        lastDistance = 6371 * c * 1000;

        overallDistance += lastDistance;  // bu 1 anliq 6.000.000-dan boyuk qiymet ala biler

        StartCoroutine(Overall());
    }

    IEnumerator Overall()
    {
        if (firstTime)
        {
            firstTime = false;

            yield return new WaitForSeconds(2);

            if (overallDistance > 6000000)
            {
                overallDistance = 0;
                lastDistance = 0;
            }
        }

        overallDistance += lastDistance;
    }

    void CalculateSpeed()
    {
        _speed = lastDistance / lastTime * 3.6f;
    }

    void CalculateAcceleration()
    {
        acceleration = (_speed - speed0) / lastTime;
    }
}
