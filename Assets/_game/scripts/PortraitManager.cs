using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PortraitManager : MonoBehaviour
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine fadeRoutine;

    public void ShowPortrait(Sprite sprite)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        portraitImage.sprite = sprite;
        fadeRoutine = StartCoroutine(FadeIn());
    }

    public void HidePortrait()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        portraitImage.enabled = true;

        Color c = portraitImage.color;
        c.a = 0f;
        portraitImage.color = c;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            c.a = Mathf.Lerp(0f, 1f, t);
            portraitImage.color = c;
            yield return null;
        }

        c.a = 1f;
        portraitImage.color = c;
    }

    private IEnumerator FadeOut()
    {
        Color c = portraitImage.color;

        float startAlpha = c.a;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            c.a = Mathf.Lerp(startAlpha, 0f, t);
            portraitImage.color = c;
            yield return null;
        }

        c.a = 0f;
        portraitImage.color = c;
        portraitImage.enabled = false;
    }
}
