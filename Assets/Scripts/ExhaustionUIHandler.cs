using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExhaustionUIHandler : MonoBehaviour
{
    private ExhaustionHandler exhaustionHandler;
    private int applesHarvested;
    [SerializeField] private Image bar;
    [SerializeField] private TMP_Text appleText;

    private void OnEnable()
    {
        PlantWater.informHarvestUI += AddApple;
    }

    private void OnDisable()
    {
        PlantWater.informHarvestUI -= AddApple;
    }

    private void AddApple()
    {
        applesHarvested++;
    }

    private void Awake()
    {
        exhaustionHandler = GameObject.FindWithTag("Player").GetComponent<ExhaustionHandler>();
    }

    private void Update()
    {
        bar.fillAmount = exhaustionHandler.GetExhaustionTimeNormalized();
        appleText.text = applesHarvested.ToString();
    }
}
