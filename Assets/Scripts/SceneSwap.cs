using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwap : MonoBehaviour
{
    [SerializeField] private string sceneName;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SwapScene);
    }

    public void SwapScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
