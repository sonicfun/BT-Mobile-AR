using SQLite4Unity3d;



/// <summary>
/// Class that corresponds to a table in SQLite database, storing info about fuzzy variables
/// </summary>
public class FuzzyParts
{
    [PrimaryKey]
    public string Name { get; set; }
    public string FuzzyType { get; set; }
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
}

/// <summary>
/// Class that is a table in SQLite database, storing Fuzzy Rules
/// </summary>
public class FuzzyRules
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public string Rule { get; set; }
    public bool SuccesParsing { get; set; }
}


public class FuzzyValues
{
    public string Name { get; set; }
    public string SetName { get; set; }
    public string MemberType { get; set; }
    public string MemberValues { get; set; }
}

/// <summary>
/// This class stores POI addresses, for performance optimization, avoiding extra calls
/// to Reverse GPS Service. This table stores all data related to an address 
/// of a GPS location
/// </summary>
public class POIAddress
{
    [PrimaryKey]
    public int ID { get; set; }

    public string road { get; set; }
    public string suburb { get; set; }
    public string town { get; set; }
    public string county { get; set; }
    public string state_district { get; set; }
    public string state { get; set; }
    public string postcode { get; set; }
    public string country { get; set; }
    public string country_code { get; set; }
    public string village { get; set; }
    public string house_number { get; set; }
    public string city { get; set; }

    public static POIAddress GetPOIFromAddress(ReverseAddress ad, BTCleanPOI poi)
    {
        POIAddress po = new POIAddress();
        po.ID = poi.ID;
        po.city = ad.city;
        po.country = ad.country;
        po.country_code = ad.country_code;
        po.county = ad.county;
        po.house_number = ad.house_number;
        po.postcode = ad.postcode;
        po.road = ad.road;
        po.state = ad.state;
        po.state_district = ad.state_district;
        po.suburb = ad.suburb;
        po.town = ad.town;
        po.village = ad.village;
        return po;
    }

    public static ReverseAddress GetAddressFromPOI(POIAddress po)
    {
        ReverseAddress ad = new ReverseAddress();
        ad.city = po.city;
        ad.country = po.country;
        ad.country_code = po.country_code;
        ad.county = po.county;
        ad.house_number = po.house_number;
        ad.postcode = po.postcode;
        ad.road = po.road;
        ad.state = po.state;
        ad.state_district = po.state_district;
        ad.suburb = po.suburb;
        ad.town = po.town;
        ad.village = po.village;
        return ad;
    }
}