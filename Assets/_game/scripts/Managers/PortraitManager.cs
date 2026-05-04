using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PortraitManager : MonoBehaviour
{
    [System.Serializable]
    public struct PortraitEntry
    {
        public string key;
        public Sprite portraitSprite;
    }

    [SerializeField] private Image portraitImage;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private List<PortraitEntry> portraitLibrary;

    private Dictionary<string, Sprite> libraryDictionary;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        // Build dictionary from list for fast lookups
        libraryDictionary = new Dictionary<string, Sprite>();
        foreach (var entry in portraitLibrary)
        {
            if (!string.IsNullOrEmpty(entry.key) && entry.portraitSprite != null)
            {
                // Prevent duplicate keys
                if (!libraryDictionary.ContainsKey(entry.key))
                {
                    libraryDictionary.Add(entry.key, entry.portraitSprite);
                }
            }
        }
    }

    private void Start()
    {
        // Ensure that portraits start completely hidden when the scene loads
        if (portraitImage != null)
        {
            Color c = portraitImage.color;
            c.a = 0f;
            portraitImage.color = c;
            portraitImage.enabled = false;
        }
    }

    public void ShowPortrait(string key)
    {
        if (libraryDictionary.TryGetValue(key, out Sprite targetSprite))
        {
            ShowPortrait(targetSprite);
        }
        else
        {
            Debug.LogWarning($"Portrait key '{key}' not found in the library.");
        }
    }

    public void ShowPortrait(Sprite sprite)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        if (sprite != null)
        {
            portraitImage.sprite = sprite;
        }
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