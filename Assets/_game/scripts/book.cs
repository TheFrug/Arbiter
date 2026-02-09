using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class book : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;
    [SerializeField] List<Transform> pages;
    int index = -1;
    bool rotate = false;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject forwardButton;

    // --- Book Toggle + Slide Variables ---
    [Header("Book Toggle")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private float slideSpeed = 6f;
    [SerializeField] private Vector2 hiddenPosition;
    [SerializeField] private Vector2 visiblePosition;

    [SerializeField] private RectTransform rectTransform;
    private bool isVisible = false;
    private bool isSliding = false;
    // -------------------------------------------

    private void Start()
    {
        InitialState();

        // --- Initialize sliding state ---
        rectTransform.anchoredPosition = hiddenPosition;
        isVisible = false;

        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleBook);
        }
        // --------------------------------------
    }

    public void InitialState()
    {
        index = 0;

        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].localRotation = Quaternion.identity;
            pages[i].SetSiblingIndex(i); // enforce correct stacking order
        }

        backButton.SetActive(false);
        forwardButton.SetActive(pages.Count > 1);
    }


    // --- Toggle Book Visibility ---
    public void ToggleBook()
    {
        if (isSliding) return;
        StartCoroutine(SlideBook(!isVisible));

        EventSystem.current.SetSelectedGameObject(null);
    }

    IEnumerator SlideBook(bool show)
    {
        isSliding = true;

        Vector2 start = rectTransform.anchoredPosition;
        Vector2 target = show ? visiblePosition : hiddenPosition;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;
            rectTransform.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        rectTransform.anchoredPosition = target;
        isVisible = show;
        isSliding = false;
    }
    // ------------------------------------


    public void RotateForward()
    {
        if (rotate) return;
        if (index >= pages.Count - 1) return;

        rotate = true;

        pages[index].SetAsLastSibling();
        StartCoroutine(RotatePage(index, 180f, true));
    }


    public void ForwardButtonActions()
    {
        if (backButton.activeInHierarchy == false)
        {
            backButton.SetActive(true); //every time turn page forward, back button should be active
        }
        if (index == pages.Count - 1)
        {
            forwardButton.SetActive(false); //if page is last then we turn off forward button
        }
    }

    public void RotateBack()
    {
        if (rotate) return;
        if (index <= 0) return;

        rotate = true;

        index--;
        pages[index].SetAsLastSibling();
        StartCoroutine(RotatePage(index, 0f, false));
    }


    public void BackButtonAction()
    {
        if (forwardButton.activeInHierarchy == false)
        {
            forwardButton.SetActive(true); //every time turn page back, forward button should be activated
        }
        if (index - 1 == -1)
        {
            backButton.SetActive(false);
        }
    }


    IEnumerator RotatePage(int pageIndex, float targetAngle, bool forward)
    {
        Quaternion startRotation = pages[pageIndex].rotation;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * pageSpeed;
            pages[pageIndex].rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        pages[pageIndex].rotation = targetRotation;

        if (forward)
        {
            index++;
        }

        rotate = false;

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        backButton.SetActive(index > 0);
        forwardButton.SetActive(index < pages.Count - 1);
    }
}
