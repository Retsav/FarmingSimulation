using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ExhaustionHandler : MonoBehaviour
{
    public float exhaustionMax = 100f;
    [SerializeField] private float exhaustionDepletionMultiplier = 1f;
    public float restMultiplier = 1f;
    
    public float exhaustionRemaining;

    private bool isTimerRunning;
    private IdleTarget idleTarget;

    private void Awake()
    {
        idleTarget = GameObject.Find("IdlePoint").GetComponent<IdleTarget>();
    }

    private void Start()
    {
        exhaustionRemaining = exhaustionMax;
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
                if (idleTarget.isResting) return;
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
        return exhaustionRemaining / exhaustionMax;
    }
}
