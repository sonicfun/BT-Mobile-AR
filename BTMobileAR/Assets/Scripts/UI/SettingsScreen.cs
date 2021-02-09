using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{

    [SerializeField]
    private Button btnapply;

    [SerializeField]
    private Button btnback;

    [SerializeField]
    private Button btnOpenUrl;

    [SerializeField]
    private Toggle DebugOnOff;

    [SerializeField]
    private Toggle FuzzyOnOff;

    [SerializeField]
    private Toggle SavePOIOnOff;


    [SerializeField]
    private InputField txtRadius;

    [SerializeField]
    private string SurveyUrl;

    private void Awake()
    {
        Assert.IsNotNull(btnapply);
        Assert.IsNotNull(btnback);
        Assert.IsNotNull(btnOpenUrl);
        Assert.IsNotNull(DebugOnOff);
        Assert.IsNotNull(FuzzyOnOff);
        Assert.IsNotNull(SavePOIOnOff);
        Assert.IsNotNull(txtRadius);
        Assert.IsFalse(string.IsNullOrEmpty(SurveyUrl));
        btnapply.onClick.AddListener(delegate { btnapplyclicked(); });
        btnback.onClick.AddListener(delegate { btnbackclicked(); });
        btnOpenUrl.onClick.AddListener(delegate { btnOpenUrlclicked(); });
        ApplySetttingsToScreen(GameManager.Instance.Setting);
    }

    private void ApplySetttingsToScreen(Settings setting)
    {
        DebugOnOff.isOn = setting.DebugMode;
        FuzzyOnOff.isOn = setting.FuzzyLogicFromSQLite;
        SavePOIOnOff.isOn = setting.SaveReversePOIToSQLite;
        txtRadius.text = setting.GPSRadius.ToString();
    }

    private void btnOpenUrlclicked()
    {
        Application.OpenURL(SurveyUrl);
    }

    private void btnbackclicked()
    {
        Kill();
    }

    private void btnapplyclicked()
    {
        if (ApplyScreenToSettings() == true)
        {
            GameManager.Instance.SaveSettings();
            Kill();
        }
    }

    private bool ApplyScreenToSettings()
    {
        string sradius = txtRadius.text;
        int val = 10;
        if (int.TryParse(sradius, out val) == false)
        {
            txtRadius.text = val.ToString();
            return false;
        }
        if (val < 1 || val > 1000) // must have a logical value
        {
            val = 10;
            txtRadius.text = val.ToString();
            return false;
        }
        GameManager.Instance.Setting.GPSRadius = val;
        GameManager.Instance.Setting.DebugMode = DebugOnOff.isOn;
        GameManager.Instance.Setting.FuzzyLogicFromSQLite = FuzzyOnOff.isOn;
        GameManager.Instance.Setting.SaveReversePOIToSQLite = SavePOIOnOff.isOn;

        return true;
    }

    private void Kill()
    {
        Destroy(GetComponent<Canvas>().gameObject);
        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
}

