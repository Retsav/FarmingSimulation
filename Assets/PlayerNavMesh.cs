using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMesh : MonoBehaviour
{
    [SerializeField] private List<Transform> targetPoints;
    [SerializeField] private List<Transform> plantSpaces;
    
    
    private NavMeshAgent navMeshAgent;
    private int i;
    private bool hasAdded = false;
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        PlantWater.cryForHelp += AddDyingPlant;
        PlantWater.informGoodStatus += RemovePlant;
        PlantWater.informDeath += RemovePlant;
    }

    private void OnDisable()
    {
        PlantWater.cryForHelp -= AddDyingPlant;
        PlantWater.informGoodStatus -= RemovePlant;
        PlantWater.informDeath -= RemovePlant;
    }

    private void AddDyingPlant(Transform plant)
    {
        targetPoints.Add(plant);
    }

    private void RemovePlant(Transform plant)
    {
        if(!targetPoints.Contains(plant)) return;
        targetPoints.Remove(plant);
    }
    
    void Update()
    {
        GoToDestination();
    }

    private void GoToDestination()
    {
        if (targetPoints.Count() == 1)
        {
            navMeshAgent.destination = targetPoints[0].position;
        }
        else
        {
            for (int j = 1; j < targetPoints.Count(); j++)
            {
                navMeshAgent.destination = targetPoints[j].position;
            }
        }
    }
}
