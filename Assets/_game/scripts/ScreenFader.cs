using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize: Ensure the image is transparent when starting the game
        if (fadeImage != null)
        {
            Color initialColor = fadeImage.color;
            initialColor.a = 0f;
            fadeImage.color = initialColor;
        }

        // Deactivate the object so it doesn't block clicks when not fading
        gameObject.SetActive(false);
    }

    public void FadeToBlack(System.Action onComplete = null)
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeRoutine(0f, 1f, onComplete));
    }

    public void FadeToClear(System.Action onComplete = null)
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeRoutine(1f, 0f, () =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }));
    }

    private IEnumerator FadeRoutine(float startAlpha, float endAlpha, System.Action onComplete)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        color.a = startAlpha;
        fadeImage.color = color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
        onComplete?.Invoke();
    }
}