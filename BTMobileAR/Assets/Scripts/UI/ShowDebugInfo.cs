using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ShowDebugInfo : MonoBehaviour
{
    [SerializeField]
    private Button btnclose;

    [SerializeField]
    private Button btnclear;

    [SerializeField]
    private Button btnsave;

    [SerializeField]
    private Button btnUpdateMap;

    [SerializeField]
    private Text debugtext;

    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private Toggle DebugOnOff;

    private AbstractMap map;
    private System.Text.StringBuilder sbdata;

    private void Awake()
    {
        Assert.IsNotNull(btnclose);
        Assert.IsNotNull(btnclear);
        Assert.IsNotNull(btnsave);
        Assert.IsNotNull(debugtext);
        Assert.IsNotNull(panel);
        Assert.IsNotNull(DebugOnOff);
        Assert.IsNotNull(btnUpdateMap);

        btnclose.onClick.AddListener(delegate { buttonclicked(); });
        btnclear.onClick.AddListener(delegate { clearclicked(); });
        btnsave.onClick.AddListener(delegate { saveclicked(); });
        btnUpdateMap.onClick.AddListener(delegate { UpdateMapclicked(); });
        DebugOnOff.onValueChanged.AddListener(delegate { ShowHidePanel(); });

    }


    public void ApplyMap(AbstractMap mmap)
    {
        map = mmap;
    }

    private void saveclicked()
    {
        // this code saves all the text to system clipboard 
        // https://answers.unity.com/questions/1144378/copy-to-clipboard-with-a-button-unity-53-solution.html
        TextEditor te = new TextEditor();
        te.text = debugtext.text;
        te.SelectAll();
        te.Copy();
        LogManager.Instance.SaveLog();
        LogManager.Instance.AddLog("Success Saved to clipboard!");
    }

    private void OnEnable()
    {
        LogManager.ShowDebugInfo += LogManager_ShowDebugInfo;
    }

    private void OnDisable()
    {
        LogManager.ShowDebugInfo -= LogManager_ShowDebugInfo;
    }

    private void LogManager_ShowDebugInfo(System.Text.StringBuilder sb)
    {
        debugtext.text = sb.ToString();
    }

    private void clearclicked()
    {
        LogManager.Instance.ClearLog();
    }

    private void ShowHidePanel()
    {
        if (DebugOnOff.isOn)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }
    }

    private void buttonclicked()
    {
        panel.SetActive(false);
        DebugOnOff.isOn = false;
    }

    private void UpdateMapclicked()
    {
        if (map == null || map.gameObject.activeSelf == false) return;
        if (GPSData.Instance.Ready)
        {
            if (GPSData.Instance.Latitude == 0 && GPSData.Instance.Longitude == 0)
                return;
            Vector2d vv = new Vector2d(GPSData.Instance.Latitude, GPSData.Instance.Longitude);
            map.UpdateMap(vv, map.Zoom);
        }
    }
}
