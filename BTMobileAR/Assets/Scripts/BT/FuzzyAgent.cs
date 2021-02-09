using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fuzzy;
using System;
using Assets.Scripts.BT;
using System.Text.RegularExpressions;
using UnityEngine.Assertions;

/// <summary>
/// Fuzzy Logic coordination class of fuzzy logic evaluations for each POI
/// </summary>
public class FuzzyAgent : MonoBehaviour {
    [SerializeField]
    float RefreshInSeconds = 30f;
    [SerializeField]
    BTRestService BTService;
    [SerializeField]
    ReversePOI ReversePOI;
    [SerializeField]
    bool IsDebug;

    [SerializeField]
    int MinutesIntervalRefresh = 2;

    public bool ReadFromSQLite;
    private FuzzyRuleManager mngr;
    private float CheckCountdown;

    public delegate void OnAgentShowInformation(BTCleanPOI poi);
    public static event OnAgentShowInformation AgentInformation;


    private void Awake()
    {
        // ensure that both have assigned from Unity Editor
        Assert.IsNotNull(BTService);
        Assert.IsNotNull(ReversePOI);
    }

        // Use this for initialization
    void Start () {
        ReadFromSQLite = false;
        if (GameManager.Instance.Setting.FuzzyLogicFromSQLite == true)
        {
            CreateFuzzyLogicFromSQLiteDB();
            if (mngr == null) // if fail read from SQLite, fall back to code creation
                CreateFuzzyLogic();
        }
        else
            CreateFuzzyLogic(); // create fuzzy logic from code
        CheckCountdown = RefreshInSeconds;
    }


    /// <summary>
    /// This method reads all Fuzzy Logic configuration from SQLite database, and applies 
    /// this logic to all relevant fuzzy classes
    /// </summary>
    private void CreateFuzzyLogicFromSQLiteDB()
    {
       try
        {
            DataService db = new DataService("fuzzy.db");
            var FuzzParts = db.GetFuzzyParts();
            var FuzzValues = db.GetFuzzyValues();
            var FuzzRules = db.GetFuzzyRules();
            // all read so try to create everything from data
            FuzzyRuleManager mgr = new FuzzyRuleManager();
            foreach (FuzzyParts p in FuzzParts)
            {
                FuzzyVariable vv = new FuzzyVariable() { Name = p.Name };
                if (string.Compare(p.FuzzyType, "Antecedent", false) == 0)
                    mgr.Antecedents.Add(vv);
                else
                    mgr.Consequent = vv;
                foreach(FuzzyValues val in FuzzValues)
                {
                    if(string.Compare(val.SetName, p.Name, true) == 0)
                    {
                        FuzzyValue fval = new FuzzyValue() { Name = val.Name, Parent = vv };
                        double[] MFValues = ParseValues(val.MemberValues);
                        if (string.Compare(val.MemberType, "trapmf", true) == 0) // trapezoid MF?
                            fval.MF = new TrapezoidMemberShip(MFValues[0], MFValues[1], MFValues[2], MFValues[3]);
                        else
                            fval.MF = new TriangleMembership(MFValues[0], MFValues[1], MFValues[2]);

                        vv.AddValue(fval);
                    }
                }
            }
            foreach(FuzzyRules r in FuzzRules)
            {
                mgr.AddNewStringRule(r.Rule);
            }
            // apply rule manager to member variable
            mngr = mgr;
            ReadFromSQLite = true;
            LogManager.Instance.AddLog("Success Create Fuzzy Logic from SQLite DB");
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private double[] ParseValues(string memberValues)
    {
        Regex regex = new Regex("[0-9.,]+",
                RegexOptions.IgnoreCase
                | RegexOptions.CultureInvariant
                | RegexOptions.IgnorePatternWhitespace
                );
        List<double> lvals = new List<double>();
        MatchCollection ms = regex.Matches(memberValues);
        foreach(Match v in ms)
        {
            double vv = double.Parse(v.Value);
            lvals.Add(vv);
        }

        return lvals.ToArray();
    }

    // Update is called once per frame
    void Update () {
        CheckCountdown -= Time.deltaTime;
        if (CheckCountdown <= 0)
        {
            CheckCountdown = RefreshInSeconds;
            StartCoroutine(FuzzyProcessPOI());
        }
    }


    /// <summary>
    /// Fuzzy Logic implementation on this Unity Coroutine
    /// Between each POI check, 2 seconds wait time
    /// </summary>
    /// <returns></returns>
    IEnumerator FuzzyProcessPOI()
    {
        // Get POI from BTRestService Singleton
        foreach (BTCleanPOI poi in BTService.ListPoints)
        {
            // if POI required a reverse address call, cool period of 2 seconds
            if (RunFuzzyEvaluations(poi))
                yield return new WaitForSeconds(2);
        }
    }

    // Run fuzzy logic on the poi
    // Stores the result of fuzzy logic in score member of BTCleanPOI class
    private bool RunFuzzyEvaluations(BTCleanPOI poi)
    {
        if (GPSData.Instance.Ready == false) return false;
        if (GameManager.Instance.Setting.DebugMode != IsDebug)
            IsDebug = GameManager.Instance.Setting.DebugMode;
        if(poi.Score != null)
        {
            TimeSpan sp = DateTime.Now - poi.Score.lastupdate;
            if (sp.Minutes < MinutesIntervalRefresh) // skip run score again, if is too soon
                return false;
        }
        // get distance from device (user)
        float distancemeters = CalculateDistance(GPSData.Instance.Latitude, poi.Latitude, GPSData.Instance.Longitude, poi.Longitude);
        // get device speed (user)
        float speed = GPSData.Instance.Speed;
        // apply values to fuzzy logic
        mngr.ApplyVariableValue("Distance", distancemeters);
        mngr.ApplyVariableValue("Speed", speed);
        // run fuzzy logic centroid method, get the crisp result
        double result = mngr.DefuzzifierCentroid();

        // convert result to action
        string sAction = "";
        if (result >= 75)
            sAction = "All";
        else if (result > 35)
            sAction = "Medium";
        else
            sAction = "Few";
 
        if(IsDebug)
        {
            LogManager.Instance.AddLog(string.Format("Fuzzy poi:{0} - Distance = {1} Speed = {2} Result = {3} ScoreType={4}", poi.ID, distancemeters, speed, result, sAction));
        }
        // save fuzzy score on POI
        poi.Score = PrepareScore(result, sAction);
        // if full metadata is required, call ReversePOI to get this information
        // if metadata already exist from previous calls, avoid the reverse call
        if (sAction == "All" && poi.Address == null) 
        {
            // call reverse Poi to get more information for this POI, and then inform the UI to show this info
            ReversePOI.ProcessReverseCleanPoint(poi, callback);
            return true;
        }
        return false;
    }


    // Prepare Score member of POI, to keep fuzzy logic result on POI
    private FuzzyScore PrepareScore(double result, string sAction)
    {
        FuzzyScore score = new FuzzyScore();
        if (string.Compare(sAction, "All", true) == 0)
            score.result = FuzzyResult.All;
        else if (string.Compare(sAction, "Medium", true) == 0)
            score.result = FuzzyResult.Medium;
        else
            score.result = FuzzyResult.Few;
        score.score = (float)result;
        score.lastupdate = DateTime.Now;
        return score;
    }

    /// <summary>
    /// This method raises an event with the POI that has full information, after the decision
    /// of Fuzzy Logic Agent
    /// </summary>
    /// <param name="poi">POI with information</param>
    /// <param name="address">Reverse GPS info for this POI</param>
    private void callback(BTCleanPOI poi, ReverseAddress address)
    {
        // here is a good place to display extra info  to user
        poi.Address = address;
        // update BTPOIManager
        BTPOIManager.Instance.AppendPOIAddress(poi);
        if (AgentInformation != null) // if event is used
        {
            AgentInformation(poi);   // raise an event to inform UI about agent actions
        }
    }

    // calculate the distance between two GPS points(lat, lon)
    private float CalculateDistance(float lat_1, float lat_2, float long_1, float long_2)
    {
        int R = 6371;

        var lat_rad_1 = Mathf.Deg2Rad * lat_1;
        var lat_rad_2 = Mathf.Deg2Rad * lat_2;
        var d_lat_rad = Mathf.Deg2Rad * (lat_2 - lat_1);
        var d_long_rad = Mathf.Deg2Rad * (long_2 - long_1);

        var a = Mathf.Pow(Mathf.Sin(d_lat_rad / 2), 2) + (Mathf.Pow(Mathf.Sin(d_long_rad / 2), 2) * Mathf.Cos(lat_rad_1) * Mathf.Cos(lat_rad_2));
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        var total_dist = R * c * 1000; // convert to meters

        return total_dist;
    }

    /// <summary>
    ///  Creates fuzzy logic from code
    /// </summary>
    private void CreateFuzzyLogic()
    {
        mngr = PrepareRuleManager();
        LogManager.Instance.AddLog("Create Fuzzy Logic from code!");
    }

    // prepare speed antecedent for Fuzzy logic using code
    private FuzzyVariable PrepareSpeed()
    {
        FuzzyVariable speed = new FuzzyVariable() { Name = "Speed" };
        FuzzyValue VerySlow = new FuzzyValue() { Name = "VerySlow" };
        FuzzyValue Slow = new FuzzyValue() { Name = "Slow" };
        FuzzyValue Medium = new FuzzyValue() { Name = "Medium" };
        FuzzyValue Fast = new FuzzyValue() { Name = "Fast" };
        FuzzyValue VeryFast = new FuzzyValue() { Name = "VeryFast" };

        VerySlow.MF = new TrapezoidMemberShip(0, 0, 5, 15);
        Slow.MF = new TriangleMembership(5, 20, 30);
        Medium.MF = new TriangleMembership(20, 30, 50);
        Fast.MF = new TrapezoidMemberShip(30, 60, 80, 100);
        VeryFast.MF = new TrapezoidMemberShip(80, 100, 120, 120);
        speed.AddValue(VerySlow);
        speed.AddValue(Slow);
        speed.AddValue(Medium);
        speed.AddValue(Fast);
        speed.AddValue(VeryFast);
        return speed;
    }

    // prepare Distance antecedent for Fuzzy logic using code
    private FuzzyVariable PrepareDistance()
    {
        FuzzyVariable Distance = new FuzzyVariable() { Name = "Distance" };

        FuzzyValue VeryClose = new FuzzyValue() { Name = "VeryClose" };
        FuzzyValue Close = new FuzzyValue() { Name = "Close" };
        FuzzyValue Medium = new FuzzyValue() { Name = "Medium" };
        FuzzyValue Far = new FuzzyValue() { Name = "Far" };
        FuzzyValue VeryFar = new FuzzyValue() { Name = "VeryFar" };

        VeryClose.MF = new TrapezoidMemberShip(0, 0, 50, 300);
        Close.MF = new TriangleMembership(0, 300, 1000);
        Medium.MF = new TriangleMembership(300, 1000, 2000);
        Far.MF = new TriangleMembership(1000, 2000, 3000);
        VeryFar.MF = new TrapezoidMemberShip(2000, 3000, 4000, 4000);

        Distance.AddValue(VeryClose);
        Distance.AddValue(Close);
        Distance.AddValue(Medium);
        Distance.AddValue(Far);
        Distance.AddValue(VeryFar);
        return Distance;
    }


    // prepare Metadata condequent for Fuzzy Logic with code
    private FuzzyVariable PrepareMetadata()
    {
        FuzzyVariable Metadata = new FuzzyVariable() { Name = "Metadata" };

        FuzzyValue Few = new FuzzyValue() { Name = "Few" };
        FuzzyValue Medium = new FuzzyValue() { Name = "Medium" };
        FuzzyValue All = new FuzzyValue() { Name = "All" };

        Few.MF = new TrapezoidMemberShip(0, 0, 10, 30);
        Medium.MF = new TrapezoidMemberShip(0, 30, 60, 90);
        All.MF = new TrapezoidMemberShip(60, 90, 100, 100);

        Metadata.AddValue(Few);
        Metadata.AddValue(Medium);
        Metadata.AddValue(All);

        return Metadata;
    }


    // prepare fuzzy logic rules using code
    private void PrepareRules(FuzzyRuleManager mgr)
    {
        mgr.AddNewRule("Distance.VeryFar", "none", "Metadata.Few");
        mgr.AddNewRule("Distance.Far", "Speed.Fast", "Metadata.Few");
        mgr.AddNewRule("Distance.Far", "Speed.Medium", "Metadata.Few");
        mgr.AddNewRule("Distance.Far", "Speed.Slow", "Metadata.Medium");
        mgr.AddNewRule("Distance.Far", "Speed.VerySlow", "Metadata.Medium");

        mgr.AddNewRule("none", "Speed.VeryFast", "Metadata.Few");

        mgr.AddNewRule("Distance.Medium", "Speed.Medium", "Metadata.Few");
        mgr.AddNewRule("Distance.Medium", "Speed.Slow", "Metadata.Medium");
        mgr.AddNewRule("Distance.Medium", "Speed.VerySlow", "Metadata.Medium");

        mgr.AddNewRule("Distance.Close", "Speed.Fast", "Metadata.Medium");
        mgr.AddNewRule("Distance.Close", "Speed.Medium", "Metadata.Medium");
        mgr.AddNewRule("Distance.Close", "Speed.Slow", "Metadata.Medium");
        mgr.AddNewRule("Distance.Close", "Speed.VerySlow", "Metadata.All");

        mgr.AddNewRule("Distance.VeryClose", "Speed.Fast", "Metadata.Medium");
        mgr.AddNewRule("Distance.VeryClose", "Speed.Medium", "Metadata.All");
        mgr.AddNewRule("Distance.VeryClose", "Speed.Slow", "Metadata.All");
        mgr.AddNewRule("Distance.VeryClose", "Speed.VerySlow", "Metadata.All");
    }


    // prepare rule manager
    private FuzzyRuleManager PrepareRuleManager()
    {
        FuzzyRuleManager mgr = new FuzzyRuleManager();
        mgr.Antecedents.Add(PrepareSpeed());
        mgr.Antecedents.Add(PrepareDistance());
        mgr.Consequent = PrepareMetadata();
        PrepareRules(mgr);
        return mgr;
    }
  
}
