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
    private bool isWalkInformed;
    private bool isSitInformed;

    public delegate void InformAnimation();

    public static InformAnimation informWalking;
    public static InformAnimation informStanding;

    [SerializeField] private Animator playerAnim;
    private AnimationHandler animHandler;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animHandler = transform.GetChild(1).GetComponent<AnimationHandler>();
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
        if (!targetPoints.Contains(plant)) return;
        targetPoints.Remove(plant);
    }

    private void RemoveDeadPlant(Transform plant)
    {
        if (!targetPoints.Contains(plant)) return;
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
        CheckWalkingAnimation();
    }

    private void CheckWalkingAnimation()
    {
        if (navMeshAgent.remainingDistance > 0.1f)
        {
            if (!isWalkInformed)
            {
                playerAnim.SetBool("isSitting", false);
                informWalking?.Invoke();
                isWalkInformed = true;
            }
        }
        else
        {
            isWalkInformed = false;
        }
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
        }
        else
        {
            if (!isCritical)
            {
                for (int j = 1; j < targetPoints.Count(); j++)
                {
                    if (animHandler.isStanded)
                    {
                        navMeshAgent.destination = targetPoints[j].position;
                        isSitInformed = false;
                    }
                    else
                    {
                        if (!isSitInformed)
                        {
                            informStanding?.Invoke();
                            isSitInformed = true;
                        }
                    }
                }
            }
        }
    }
}
