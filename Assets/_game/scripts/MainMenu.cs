using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstLevel;

    [Header("Audio")]
    // Changed from AudioClip to string so it matches the AudioManager library
    [SerializeField] private string characterCreationAmbienceKey = "LoopAmbience";

    [Header("Panels")]
    [SerializeField] private GameObject CreditsPanel;
    [SerializeField] private GameObject OptionsPanel;

    public void StartGame()
    {
        // Pass the string key instead of the direct AudioClip reference
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TransitionAmbience(characterCreationAmbienceKey, 1.5f);
        }

        // Trigger the screen fade
        if (ScreenFader.Instance != null)
        {
            ScreenFader.Instance.FadeToBlack(() =>
            {
                SceneManager.LoadScene(firstLevel);
                ScreenFader.Instance.FadeToClear();
            });
        }
        else
        {
            SceneManager.LoadScene(firstLevel);
        }
    }

    public void OpenOptions() { }
    public void CloseOptions() { }

    public void OpenCredits()
    {
        if (CreditsPanel != null) CreditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (CreditsPanel != null) CreditsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}