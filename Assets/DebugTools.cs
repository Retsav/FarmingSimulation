using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    [SerializeField] private float timeScaleToSpeed = 2f;
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Time.timeScale = timeScaleToSpeed;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
