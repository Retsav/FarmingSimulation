using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMesh : MonoBehaviour
{
    [SerializeField] private List<Transform> targetPoints;
    
    
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
    }

    private void OnDisable()
    {
        PlantWater.cryForHelp -= AddDyingPlant;
        PlantWater.informGoodStatus -= RemovePlant;
    }

    private void AddDyingPlant(Transform plant)
    {
        targetPoints.Add(plant);
    }

    private void RemovePlant(Transform plant)
    {
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
