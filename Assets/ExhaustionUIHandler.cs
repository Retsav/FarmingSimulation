using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExhaustionUIHandler : MonoBehaviour
{
    private ExhaustionHandler exhaustionHandler;
    [SerializeField] private Image bar;

    private void Awake()
    {
        exhaustionHandler = GameObject.FindWithTag("Player").GetComponent<ExhaustionHandler>();
    }

    private void Update()
    {
        bar.fillAmount = exhaustionHandler.GetExhaustionTimeNormalized();
    }
}
