using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class book : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;
    [SerializeField] List<Transform> pages;
    int index = -1;
    bool rotate = false;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject forwardButton;

    private void Start()
    {
        InitialState();
    }
    
    public void InitialState()
    {
        for (int i=0; i<pages.Count; i++)
        {
            pages[i].transform.rotation = Quaternion.identity;
        }
        pages[0].SetAsLastSibling(); 
    }

    public void RotateForward()
    {
        if (rotate == true) { return; }
        index++;
        float angle = 180; //to rotate page back, set rotation to 180 degrees around y axis

        ForwardButtonActions();
        pages[index].SetAsLastSibling();
        StartCoroutine(Rotate(angle, true));
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
        if (rotate == true) { return; }
        float angle = 0; //to rotate page back, set rotation to 0 degrees around y axis
        pages[index].SetAsLastSibling();
        BackButtonAction();
        StartCoroutine(Rotate(angle, false));
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


    IEnumerator Rotate(float angle, bool forward)
    {
        float value = 0f;
        while (true)
        {
            rotate = true;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            value += Time.deltaTime * pageSpeed;
            pages[index].rotation = Quaternion.Slerp(pages[index].rotation, targetRotation, value); //smoothly turn page
            float angle1 = Quaternion.Angle(pages[index].rotation, targetRotation); //calculate angle between given angle of rotation and current angle of rotation
            if (angle1 < 0.1)
            {
                if (forward == false)
                {
                    index--;
                }
                rotate = false;
                break;

            }
            yield return null;
        }
    }
}
