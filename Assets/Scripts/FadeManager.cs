using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }
    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    
    void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public static void FadeTo(float targetAlpha, float duration)
    {
        if (Instance == null)
        {
            Debug.LogWarning("FadeManager instance is not available.");
            return;
        }

        Instance.StartFade(targetAlpha, duration);
    }

    private void StartFade(float targetAlpha, float duration)
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.LogWarning("FadeManager requires a CanvasGroup component.");
                return;
            }
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            return;
        }

        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        fadeCoroutine = null;
    }
}
