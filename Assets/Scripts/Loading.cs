using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Image loadingProgress;

    public void LoadScene(int sceneId)
    {
        loadingScreen.SetActive(true);
        mainCanvas.SetActive(false);
        StartCoroutine(nameof(LoadSceneAsync), sceneId);
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {


        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            loadingProgress.fillAmount = progressValue;

            yield return null;
        }
    }
}
