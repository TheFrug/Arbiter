using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class book : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;
    [SerializeField] List<Transform> pages;
    int index = -1;

    public void RotateNext()
    {
        index++;
        float angle = 180;
    }

    public void RotateBack()
    {
        float angle = 0;
    }
}
