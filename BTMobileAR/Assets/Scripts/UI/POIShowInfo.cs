using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class POIShowInfo : MonoBehaviour
{
    [SerializeField]
    private Button btnclose;

    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private Text POItext;

    [SerializeField]
    float ShutdownInSeconds = 40f;

    private float CheckCountdown;
    private void Awake()
    {
        Assert.IsNotNull(btnclose);
        Assert.IsNotNull(panel);
        Assert.IsNotNull(POItext);
        btnclose.onClick.AddListener(delegate { buttonclicked(); });
        CheckCountdown = ShutdownInSeconds;
    }

    private void buttonclicked()
    {
        panel.SetActive(false);
        Kill();
    }

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        CheckCountdown -= Time.deltaTime;
        if (CheckCountdown >= 0) return;
        panel.SetActive(false);
        Kill();
    }

    public void ShowPOIInfo(BTCleanPOI poi)
    {
        CheckCountdown = ShutdownInSeconds;
        POItext.text = PrepareText(poi);
        panel.SetActive(true);
    }

    private void Kill()
    {
        panel.SetActive(false);
        Destroy(GetComponent<Canvas>().gameObject);
    }
    private string PrepareText(BTCleanPOI poi)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
        sb.Append("POI\r\n");
        sb.Append(poi.PrepareFullPOIDescription());

        return sb.ToString();
    }


    public void DisplayCustomText(string stext)
    {
        POItext.text = stext;
        panel.SetActive(true);
    }
}
