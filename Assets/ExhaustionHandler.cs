using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustionHandler : MonoBehaviour
{
    private const float EXHAUSTION_MAX = 100f;
    [SerializeField] private float exhaustionDepletionMultiplier = 1f;
    [SerializeField] private float restMultiplier = 1f;
    
    [SerializeField] private float exhaustionRemaining;

    private bool isTimerRunning;

    private void Start()
    {
        exhaustionRemaining = EXHAUSTION_MAX;
        isTimerRunning = true;
    }

    private void Update()
    {
        CalculateTimer();
    }

    private void CalculateTimer()
    {
        if (isTimerRunning)
        {
            if (exhaustionRemaining >= 0)
            {
                exhaustionRemaining -= Time.deltaTime * exhaustionDepletionMultiplier;
            }
            else
            {
                exhaustionRemaining = 0f;
                isTimerRunning = false;
            }
        }
    }

    public float GetExhaustionTimeNormalized()
    {
        return exhaustionRemaining / EXHAUSTION_MAX;
    }
}
