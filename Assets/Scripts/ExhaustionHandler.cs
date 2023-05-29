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

    private float timeBeforeCriticalCondition = 10f;
    public float exhaustionRemaining;

    private bool isTimerRunning;
    private bool isExhaustCritical;
    private IdleTarget idleTarget;
    
    public delegate void CriticalCondition();
    public static CriticalCondition criticalCondition;

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
                if (exhaustionRemaining <= timeBeforeCriticalCondition)
                {
                    criticalCondition?.Invoke();
                }
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
