using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CanvasShowInfo : MonoBehaviour {

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
    }


    // Use this for initialization
    void Start () {
        panel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (panel.activeSelf == false) return;
        CheckCountdown -= Time.deltaTime;
        if (CheckCountdown >= 0) return;
        panel.SetActive(false);
    }

    public void DisplayCustomText(string stext)
    {
        POItext.text = stext;
        CheckCountdown = ShutdownInSeconds;
        panel.SetActive(true);
    }

    public bool IsVisible()
    {
        return panel.activeSelf;
    }
}
