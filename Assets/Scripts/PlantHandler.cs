using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlantHandler : MonoBehaviour
{
    [SerializeField] private List<Transform> plantSpaces;
    private GameObject player;
    [SerializeField] private Image actionfillBar;
    [SerializeField] private GameObject actionBar;

    [SerializeField] private GameObject plantPrefab;

    [SerializeField] private float timeBeforeSeeded;
    [SerializeField] private float maxTimeSeed = 5f;
    
    public delegate void InformSeed(Transform plantSpace);
    public static InformSeed informSeed;
    public static InformSeed informDoneSeeding;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        CheckIfEmpty();
        foreach (var plant in plantSpaces)
        {
            if (plant.childCount == 0)
            {
                if (player.transform.position.x == plant.transform.position.x)
                {
                    actionBar.SetActive(true);
                    if (maxTimeSeed >= timeBeforeSeeded)
                    {
                        timeBeforeSeeded += Time.deltaTime;
                    } else  {
                        informDoneSeeding?.Invoke(plant.GameObject().transform);
                        Instantiate(plantPrefab, plant.position, Quaternion.identity, plant.transform); 
                        timeBeforeSeeded = 0f;
                        actionBar.SetActive(false);
                    }
                    actionfillBar.fillAmount = GetSeedTimeNormalized();
                }
                else
                {
                    actionBar.SetActive(false);
                }
            }
        }
    }

    private float GetSeedTimeNormalized()
    {
        return timeBeforeSeeded / maxTimeSeed;
    }

    private void CheckIfEmpty()
    {
        foreach (var plant in plantSpaces)
        {
            if (plant.childCount > 0) return;
            Debug.Log("Wykryto seedplace bez rosliny");
            informSeed?.Invoke(plant.GameObject().transform);
        }
    }
}
