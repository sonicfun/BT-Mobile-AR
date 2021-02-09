using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ShowSettings : MonoBehaviour
{

    [SerializeField]
    GameObject prefabShowSettings;

    [SerializeField]
    private Button btnShowSettings;

    private void Awake()
    {
        Assert.IsNotNull(prefabShowSettings);
        Assert.IsNotNull(btnShowSettings);
        btnShowSettings.onClick.AddListener(delegate { CreateSettingsScreen(); });
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateSettingsScreen()
    {
        Instantiate(prefabShowSettings);
    }
}