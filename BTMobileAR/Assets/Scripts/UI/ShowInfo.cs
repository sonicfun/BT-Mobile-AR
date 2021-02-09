using Mapbox.Unity.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI; //Need this for calling UI scripts

public class ShowInfo : MonoBehaviour
{
    [SerializeField]
    FuzzyAgent agent;

    [SerializeField]
    GameObject prefabShowInfo;

    [SerializeField]
    private BTRestService BTService;

    [SerializeField]
    private AbstractMap map;

    private GameObject mdebuginstance;
    private void Awake()
    {
        Assert.IsNotNull(agent);
        Assert.IsNotNull(prefabShowInfo);
        Assert.IsNotNull(BTService);
        Assert.IsNotNull(map);
        mdebuginstance = null;
    }
    void Start()
    {
        if (GameManager.Instance.Setting.DebugMode)
        {
            InitializeDebugInformation();
        }

    }

    private void InitializeDebugInformation()
    {
        GameObject obj = Instantiate(prefabShowInfo);
        mdebuginstance = obj;
        DebugPOICreate dbobj = obj.GetComponent<DebugPOICreate>();
        dbobj.BTService = BTService;

        ShowDebugInfo shobj = obj.GetComponent<ShowDebugInfo>();
        shobj.ApplyMap(map);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.Setting.DebugMode)
        {
            if (mdebuginstance == null)
                InitializeDebugInformation();
        }
        else
        {
            if (mdebuginstance != null)
            {
                Destroy(mdebuginstance);
                mdebuginstance = null;
            }
        }
    }

    //private void OnEnable()
    //{
    //    FuzzyAgent.AgentInformation += ShowInfoFromAgent;
    //}

    //private void OnDisable()
    //{
    //    FuzzyAgent.AgentInformation -= ShowInfoFromAgent;
    //}

    //void ShowInfoFromAgent(BTCleanPOI poi)
    //{
    //    // now create a prefab
    //    GameObject obj = Instantiate(prefabShowPOIInfo);
    //    if (obj != null)
    //    {
    //        POIShowInfo pp = obj.GetComponent<POIShowInfo>();
    //        pp.ShowPOIInfo(poi);
    //    }
    //}


}