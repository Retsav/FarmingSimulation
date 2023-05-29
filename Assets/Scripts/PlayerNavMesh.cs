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

    private NavMeshAgent navMeshAgent;
    private int i;
    private bool isCritical;
    [SerializeField] private float exhaustionStunTime = 20f;
    public bool isDetoxicating;
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        PlantWater.cryForHelp += AddDyingPlant;
        PlantWater.informGoodStatus += RemovePlant;
        PlantWater.informDeath += RemoveDeadPlant;
        PlantWater.informHarvesting += AddHarvestablePlant;
        PlantHandler.informSeed += AddSeedablePlant;
        PlantHandler.informDoneSeeding += RemoveSeedablePlant;
        PlantWater.informRemoveSeed += RemoveSeedable;
        PlantWater.informCleanRemoval += RemovePlant;
        ExhaustionHandler.criticalCondition += CriticalCondition;
    }

    private void OnDisable()
    {
        PlantWater.cryForHelp -= AddDyingPlant;
        PlantWater.informGoodStatus -= RemovePlant;
        PlantWater.informDeath -= RemoveDeadPlant;
        PlantWater.informHarvesting -= AddHarvestablePlant;
        PlantHandler.informSeed -= AddSeedablePlant;
        PlantHandler.informDoneSeeding -= RemoveSeedablePlant;
        PlantWater.informRemoveSeed -= RemoveSeedable;
        PlantWater.informCleanRemoval -= RemovePlant;
        ExhaustionHandler.criticalCondition -= CriticalCondition;
    }

    private void AddDyingPlant(Transform plant)
    {
        targetPoints.Add(plant);
    }

    private void AddSeedablePlant(Transform seedPlace)
    {
        if (targetPoints.Contains(seedPlace)) return;
        if (isDetoxicating) return;
        targetPoints.Add(seedPlace);
    }

    private void RemoveSeedablePlant(Transform seedPlace)
    {
        targetPoints.Remove(seedPlace);
    }
    
    private void AddHarvestablePlant(Transform plant)
    {
        targetPoints.Add(plant);
    }

    private void RemovePlant(Transform plant)
    {
        if(!targetPoints.Contains(plant)) return;
        targetPoints.Remove(plant);
    }

    private void RemoveDeadPlant(Transform plant)
    {
        if(!targetPoints.Contains(plant)) return;
        targetPoints.Remove(plant);
        targetPoints.Add(plant);
    }

    private void RemoveSeedable(Transform plant)
    {
        if (!targetPoints.Contains(plant)) return;
        targetPoints.Remove(plant);
    }

    private void CriticalCondition()
    {
        isCritical = true;
        navMeshAgent.destination = targetPoints[0].position;
        StartCoroutine(ResetCritical());
    }

    void Update()
    {
        GoToDestination();
    }

    IEnumerator ResetCritical()
    {
        yield return new WaitForSeconds(exhaustionStunTime);
        isCritical = false;
    }

    private void GoToDestination()
    {
        if (targetPoints.Count() == 1)
        {
            navMeshAgent.destination = targetPoints[0].position;
            Debug.Log("Siedze na dupie");
        }
        else
        {
            if (!isCritical)
            {
                for (int j = 1; j < targetPoints.Count(); j++)
                {
                    if(targetPoints[j].CompareTag("tree")) { Debug.Log("Wykryto drzewo"); }
                    navMeshAgent.destination = targetPoints[j].position;
                }
            }
        }
    }
}
