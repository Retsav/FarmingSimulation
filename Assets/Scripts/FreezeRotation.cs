using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        Vector3 currentRotation = rectTransform.rotation.eulerAngles;
        currentRotation.y = 0f;
        rectTransform.rotation = Quaternion.Euler(currentRotation);
    }
}
