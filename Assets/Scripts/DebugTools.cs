using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    [SerializeField] private float timeScaleToSpeed = 2f;
    [SerializeField] private float permaScaleToSpeed = 10f;

    private bool permaMode;
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            permaMode = true;
        } 
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            permaMode = false;
        }

        if (permaMode)
        {
            Time.timeScale = permaScaleToSpeed;
        }
    }
}
