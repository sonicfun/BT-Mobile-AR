using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadLever : MonoBehaviour
{
   // public GameObject loadingScreen;
   // public Slider slider;
   // public Text progressText;

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronoulsy(sceneIndex));

    }

    IEnumerator LoadAsynchronoulsy(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        //loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

           // slider.value = progress;
            Debug.Log(operation.progress);
           // progressText.text = progress * 100f + "%";

            yield return null;
        }
    }
}
