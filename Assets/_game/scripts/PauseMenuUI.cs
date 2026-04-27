using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject pauseRoot;

    [Header("Buttons")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button resumeButton;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMPro.TMP_Text titleText;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed = 3f;

    private bool isPaused = false;
    private bool isEndScreen = false; // <--- track end screen mode

    private void Start()
    {
        if (pauseRoot != null)
            pauseRoot.SetActive(false);

        if (toggleButton != null)
            toggleButton.onClick.AddListener(TogglePause);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);
    }

    private void Update()
    {
        if (!isEndScreen && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isEndScreen) return; // can't pause if end screen is showing

        isPaused = !isPaused;

        pauseRoot.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        // Normal pause appearance
        if (isPaused && backgroundImage != null)
        {
            Color c = backgroundImage.color;
            c.a = 0.6f;
            backgroundImage.color = c;
        }

        if (titleText != null && isPaused)
            titleText.text = "PAUSE";

        EventSystem.current?.SetSelectedGameObject(null);
    }

    public void Resume()
    {
        if (isEndScreen) return;

        isPaused = false;
        pauseRoot.SetActive(false);
        Time.timeScale = 1f;
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ShowEndScreen(string message = "END")
    {
        isEndScreen = true; // <--- mark end screen mode
        isPaused = true;

        pauseRoot.SetActive(true);
        Time.timeScale = 0f;

        // Fully opaque background
        if (backgroundImage != null)
        {
            Color c = backgroundImage.color;
            c.a = 1f;
            backgroundImage.color = c;
        }

        // End screen title
        if (titleText != null)
        {
            titleText.text = message;
        }

        // Disable buttons you don't want for end screen
        if (resumeButton != null)
            resumeButton.gameObject.SetActive(false);

        if (toggleButton != null)
            toggleButton.gameObject.SetActive(false);

        EventSystem.current?.SetSelectedGameObject(null);
    }
}