using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public class BTPOI
{
    public int id;
    public string type;
    public float latitude;
    public float longitude;
    public float x;
    public float y;
    public float z;
    public string dateTimeUpdated;
}


[Serializable]
public class BTPOIInfo
{
    public List<BTPOI> POI;
}


/// <summary>
/// Class that stores all relevant information from a POI (Point if Interest)
/// The address field, stores all information from Reverse GPS service, if that call is done
/// or else is null
/// </summary>
public class BTCleanPOI
{
    public int ID { get; set; }
    public string POIType { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public DateTime LastUpdate { get; set; }
    public ReverseAddress Address { get; set; }
    public FuzzyScore Score { get; set; }

    public BTCleanPOI()
    {

    }

    public BTCleanPOI(BTPOI input)
    {
        ID = input.id;
        POIType = input.type;
        Latitude = input.latitude;
        Longitude = input.longitude;
        LastUpdate = Convert.ToDateTime(input.dateTimeUpdated);
    }


    public string GetPoiDescriptionByScore()
    {
        if (Score == null)
            return FewInfo();
        switch(Score.result)
        {
            case FuzzyResult.Few:
                return FewInfo();
            case FuzzyResult.Medium:
                return MediumInfo();
            case FuzzyResult.All:
                return PrepareFullPOIDescription();
        }
        return "";
    }

    public string FewInfo()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
        sb.AppendFormat("POI ID - {0}\n",ID);
        sb.AppendFormat("Type: {0}\n",POIType);
        if(Score != null)
         sb.AppendFormat("Score: {0} Rank: {1}\n", Score.score, Score.result);
        return sb.ToString();
    }

    public string MediumInfo()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
        sb.AppendFormat("POI ID - {0}\n", ID);
        sb.AppendFormat("Type: {0}\n", POIType);
        sb.AppendFormat("Latitude: {0} Longitude: {1}\n", Latitude, Longitude);
        if (Score != null)
            sb.AppendFormat("Score: {0} Rank: {1}\n", Score.score, Score.result);
        return sb.ToString();
    }
    public string PrepareFullPOIDescription()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
        sb.AppendFormat("ID: {0} Type: {1}\n", ID, POIType);
        sb.AppendFormat("Latitude: {0} Longitude: {1}\n", Latitude, Longitude);
        if (Score != null)
            sb.AppendFormat("Score: {0} Rank: {1}\n", Score.score, Score.result);
        if (Address != null)
        {
            if(string.IsNullOrEmpty(Address.house_number) == false)
            {
                sb.Append(Address.house_number);
                sb.Append(" ");
            }
            if (Address.road != null)
            {
                sb.Append(Address.road);
                sb.Append(" ");
            }
            sb.Append("\n");
            if (string.IsNullOrEmpty(Address.suburb) == false)
            {
                sb.Append(Address.suburb);
                sb.Append(" ");
            }
            if (string.IsNullOrEmpty(Address.town) == false)
            {
                sb.Append(Address.town);
                sb.Append(" ");
            }
            if (string.IsNullOrEmpty(Address.village) == false)
            {
                sb.Append(Address.village);
                sb.Append(" ");
            }
            if (string.IsNullOrEmpty(Address.city) == false)
            {
                sb.Append(Address.city);
                sb.Append(" ");
            }
            sb.Append("\n");
            if (string.IsNullOrEmpty(Address.state_district) == false)
            {
                sb.Append(Address.state_district);
                sb.Append(" ");
            }
            if (string.IsNullOrEmpty(Address.postcode) == false)
            {
                sb.Append(Address.postcode);
                sb.Append(" ");
            }
            sb.Append("\n");
            if (string.IsNullOrEmpty(Address.state) == false)
            {
                sb.Append(Address.state);
                sb.Append(" ");
            }
            if (string.IsNullOrEmpty(Address.country_code) == false)
            {
                sb.Append(Address.country_code);
                sb.Append(" ");
            }
            if (string.IsNullOrEmpty(Address.country) == false)
            {
                sb.Append(Address.country);
                sb.Append(" ");
            }
        }
        return sb.ToString();
    }
}


/// <summary>
/// class that has GPS data and does some helper calculations or conversions
/// </summary>
public class GPSPoint
{
    public double latitude;
    public double longitude;

    public double CalcDistance(double latnew, double longnew)
    {
        int R = 6371;

        var lat_rad_1 = Mathf.Deg2Rad * latitude;
        var lat_rad_2 = Mathf.Deg2Rad * latnew;
        var d_lat_rad = Mathf.Deg2Rad * (latnew - latitude);
        var d_long_rad = Mathf.Deg2Rad * (longnew - longitude);

        var a = Mathf.Pow(Mathf.Sin((float)d_lat_rad / 2), 2) + (Mathf.Pow(Mathf.Sin((float)d_long_rad / 2), 2) * Mathf.Cos((float)lat_rad_1) * Mathf.Cos((float)lat_rad_2));
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        var total_dist = R * c * 1000; // convert to meters

        return total_dist;
    }

    public static string GetDoubleValue(double Amount)
    {
        var CustomFormatInfo = new NumberFormatInfo();
        var culture = new CultureInfo("en-US");
        CustomFormatInfo = culture.NumberFormat;
        CustomFormatInfo.NumberGroupSeparator = ",";
        CustomFormatInfo.NumberDecimalSeparator = ".";
        return Amount.ToString("g", CustomFormatInfo);
    }
}


/// <summary>
/// class that has all the necessery fields to store JSON info from Reverse GPS Service
/// </summary>
[Serializable]
public class ReverseGPS
{
    public string place_id;
    public string licence;
    public string osm_type;
    public string osm_id;
    public string lat;
    public string lon;
    public string place_rank;
    public string category;
    public string type;
    public string importance;
    public string addresstype;
    public string name;
    public string display_name;
    public ReverseAddress address;
    public List<string> boundingbox;
}

/// <summary>
/// Class that has all necessary information to read Address from Reverse GPS service
/// </summary>
[Serializable]
public class ReverseAddress
{
    public string house_number;
    public string road;
    public string village;
    public string suburb;
    public string town;
    public string city;
    public string county;
    public string state_district;
    public string state;
    public string postcode;
    public string country;
    public string country_code;
}

public enum FuzzyResult
{
    All = 1,
    Medium = 2,
    Few = 3
}

[Serializable]
public class FuzzyScore
{
    public float score;
    public FuzzyResult result;
    public DateTime lastupdate;
}


[Serializable]
public class POIPrefabsByScore
{
    public GameObject Normal;
    public GameObject All;
    public GameObject Medium;
    public GameObject Few;

    public GameObject GetPrefabByScore(FuzzyScore score)
    {
        if (score == null) return Normal;
        GameObject retobj = null;
        switch(score.result)
        {
            case FuzzyResult.All:
                retobj = All;
                break;
            case FuzzyResult.Medium:
                retobj = Medium;
                break;
            case FuzzyResult.Few:
                retobj = Few;
                break;
        }
        if (retobj == null)
            retobj = Normal;
        return retobj;
    }
}
