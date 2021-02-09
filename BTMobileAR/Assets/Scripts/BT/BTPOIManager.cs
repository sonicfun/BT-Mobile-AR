using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BTPOIManager : Singleton<BTPOIManager>
{
    private List<BTCleanPOI> mcleanlist;
    // read only property
    public List<BTCleanPOI> ListPoints
    {
        get { return mcleanlist; }
    }

    // guarantee this will be always a singleton only - can't use the constructor!
    // Use this for initialization
    protected BTPOIManager()
    {
        mcleanlist = new List<BTCleanPOI>();
    }

    // append input list to the initial list
    public void ApendListData(List<BTCleanPOI> lin)
    {
        foreach(var i in lin)
        {
            if (mcleanlist.Where(x => x.ID == i.ID).Count() > 0)
                continue;
            mcleanlist.Add(i);
        }
    }

    public void AppendPOIAddress(BTCleanPOI poi)
    {
        ReverseAddress address = poi.Address;
        BTCleanPOI found = mcleanlist.Where(x => x.ID == poi.ID).FirstOrDefault();
        if (found == null)
        {
            mcleanlist.Add(poi);
            found = poi;
        }
        else
        {
            found.Address = address;
            found.Score = poi.Score;
        }
    }


    public List<BTCleanPOI> GetListByCategory(string stype)
    {
        return mcleanlist.Where(x => x.POIType == stype).ToList();
    }


    //   // Use this for initialization
    //   void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
}
