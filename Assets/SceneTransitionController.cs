using UnityEngine;

public class SceneTransitionController : MonoBehaviour
{
    private void Start()
    {
        if (ScreenFader.Instance != null)
        {
            ScreenFader.Instance.FadeToClear();
        }
    }
}