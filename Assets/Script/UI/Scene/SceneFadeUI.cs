//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SceneFadeUI : FadeUI
//{
//    [SerializeField] private float fadeTime;
//    private CanvasGroup sceneFadeCanvas;

//    void Start()
//    {
//        sceneFadeCanvas = GetComponent<CanvasGroup>();
//        FadeUIOut(sceneFadeCanvas, fadeTime);
//    }
//    public IEnumerator FadeIn()
//    {
//        FadeUIIn(sceneFadeCanvas,fadeTime);
//        yield return new WaitForSeconds(fadeTime);
//    }
//}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeUI : FadeUI
{
    [Header("Scene Transition")]
    [SerializeField] private string nextScene;
    [SerializeField] private float delayBeforeLoad = 0.5f;

    private void Start()
    {
        // Khi bắt đầu scene mới → fade từ đen ra (lộ gameplay)
        StartCoroutine(FadeOut());
    }

    public void FadeAndLoadNextScene()
    {
        StartCoroutine(FadeOutAndLoad());
    }

    private IEnumerator FadeOutAndLoad()
    {
        yield return StartCoroutine(FadeIn()); // Fade đen lại
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(nextScene);
    }
}
