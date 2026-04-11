//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PrologueFadeUI : FadeUI
//{
//    private CanvasGroup canvasPanel;

//    private void Start()
//    {
//        canvasPanel = GetComponent<CanvasGroup>();
//    }

//    public IEnumerator FadeIn(float delay)
//    {
//        FadeUIIn(canvasPanel,delay);
//        yield return new WaitForSeconds(delay);
//    }
//    public IEnumerator FadeOut(float delay)
//    {
//        FadeUIOut(canvasPanel,delay);
//        yield return new WaitForSeconds(delay);
//    }
//}
using System.Collections;
using UnityEngine;

public class PrologueFadeUI : FadeUI
{
    [Header("Prologue Fade")]
    [SerializeField] private float startDelay = 0.5f;

    private void Start()
    {
        // Bắt đầu bằng fade in (hiện phần mở đầu)
        StartCoroutine(FadeIn(startDelay));
    }

    public void EndPrologue()
    {
        // Khi hết prologue → fade out
        StartCoroutine(FadeOut());
    }
}
