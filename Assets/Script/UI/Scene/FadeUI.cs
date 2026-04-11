//using System.Collections;

//using System.Collections.Generic;

//using UnityEngine;

//public class FadeUI : MonoBehaviour

//{

//    public enum FadeDirection

//    {

//        In,

//        Out

//    }

//    public virtual void FadeUIIn(CanvasGroup canvasGroup, float _seconds)

//    {

//        StartCoroutine(Fade(canvasGroup, FadeDirection.In, _seconds));

//    }

//    public virtual void FadeUIOut(CanvasGroup canvasGroup, float _seconds)

//    {

//        StartCoroutine(Fade(canvasGroup, FadeDirection.Out, _seconds));

//    }

//    private IEnumerator Fade(CanvasGroup canvasGroup, FadeDirection fadeDirection, float fadeTime)

//    {

//        float startAlpha = fadeDirection == FadeDirection.In ? 0f : 1f;

//        float endAlpha = fadeDirection == FadeDirection.In ? 1f : 0f;

//        float fadeStep = (Time.unscaledDeltaTime / fadeTime) * (fadeDirection == FadeDirection.In ? 1 : -1);

//        if (fadeDirection == FadeDirection.In)

//        {

//            canvasGroup.interactable = true;

//            canvasGroup.blocksRaycasts = true;

//        }

//        while ((fadeDirection == FadeDirection.Out && startAlpha > endAlpha) ||

//          (fadeDirection == FadeDirection.In && startAlpha < endAlpha))

//        {

//            canvasGroup.alpha = startAlpha;

//            startAlpha += fadeStep;

//            yield return null;

//        }

//        canvasGroup.alpha = endAlpha;

//        if (fadeDirection == FadeDirection.Out)

//        {

//            canvasGroup.interactable = false;

//            canvasGroup.blocksRaycasts = false;

//        }

//    }

//}
using System.Collections;
using UnityEngine;

public class FadeUI : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] protected float fadeDuration = 1f;

    protected CanvasGroup canvasGroup;
    protected bool isFading = false;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) 
            canvasGroup = gameObject.AddComponent<CanvasGroup>(); 
    }

    // Fade In: hiện dần lên
    public virtual IEnumerator FadeIn(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(Fade(0f, 1f));

        // 🔹 Kích hoạt lại tương tác khi fade in hoàn tất
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    // Fade Out: mờ dần đi
    public virtual IEnumerator FadeOut(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(Fade(1f, 0f));
        // 🔹 Tắt tương tác khi fade out xong
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    // Core fade logic
    protected IEnumerator Fade(float from, float to)
    {
        if (isFading) yield break; // tránh gọi nhiều lần
        isFading = true;

        float elapsed = 0f;
        canvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // dùng unscaledDeltaTime để không bị ảnh hưởng bởi Time.timeScale
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = to;
        isFading = false;
    }

    // 👇 Dùng cho OnClick trong Unity
    public void FadeInButton() => StartCoroutine(FadeIn());
    public void FadeOutButton() => StartCoroutine(FadeOut());
}

