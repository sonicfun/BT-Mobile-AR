using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using UnityEngine.Assertions;

/// <summary>
/// The main functionality of this class, is to perform a login validation
/// If valid user name & password are used, the next scene is loaded
/// </summary>
public class LoginScript : MonoBehaviour {

    [SerializeField]
    private GameObject email;
    [SerializeField]
    private GameObject password;

    [SerializeField]
    private Button btnLogin;

    private string Email;
    private string Password;

    [SerializeField]
    private GameObject ProgressPanel;
    [SerializeField]
    private Transform LoadingBar;

    [SerializeField]
    private Text errortxt;
    [SerializeField]
    private float currentAmount = 1;
    [SerializeField]
    private float speed = 0.5f;



    [Serializable]
    public class UserDetail
    {
       public string message;
       public  int status;
       public  Data data;
    }

    [Serializable]
    public class Data
    {
       public string first_name;
    }

    private void Awake()
    {
        Assert.IsNotNull(btnLogin);
        Assert.IsNotNull(errortxt);
        Assert.IsNotNull(email);
        Assert.IsNotNull(password);
        Assert.IsNotNull(ProgressPanel);
        Assert.IsNotNull(LoadingBar);


        errortxt.gameObject.SetActive(false);
        ProgressPanel.SetActive(false);
        btnLogin.onClick.AddListener(validateLogin);
    }

    // Use this for initialization
    void Start () {
        Screen.orientation = ScreenOrientation.Portrait;
    }
	
	// Update is called once per frame
	void Update () {
        if (ProgressPanel.activeSelf)
        {

            if(currentAmount< 100)
            {
                currentAmount += currentAmount * speed;
                LoadingBar.GetComponent<Image>().fillAmount = currentAmount/100;
            }
            else
            {
                currentAmount = 1;
                LoadingBar.GetComponent<Image>().fillAmount = currentAmount/100;
            }
        }

	}
    private void validateLogin()
    {
        errortxt.gameObject.SetActive(false);
        string sEmail = email.GetComponent<InputField>().text;
        string sPassword = password.GetComponent<InputField>().text;
        bool bok = ValidateEmailPassword(sEmail, sPassword);
        if (bok)
        {
            Email = sEmail;
            Password = sPassword;
            ProgressPanel.SetActive(true);
            SceneManager.LoadScene(1);
        }
        else
        {
            errortxt.gameObject.SetActive(true);
            password.GetComponent<InputField>().text = ""; // clear password
        }
    }

    private bool ValidateEmailPassword(string email, string password)
    {
        if(string.Compare(email, "BT", true) == 0 && password == "mobilear")
            return true;

        return false;
    }
}
