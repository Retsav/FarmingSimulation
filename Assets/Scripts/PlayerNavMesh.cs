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

    [SerializeField] private PlantHandler planthandler;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animHandler = transform.GetChild(1).GetComponent<AnimationHandler>();
        planthandler = GetComponent<PlantHandler>();
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
        CheckIfFreezed();
        ReturnDistance();
    }

    private void CheckWalkingAnimation()
    {
        if (navMeshAgent.remainingDistance > 0.1f)
        {
            ResetBools();
            if (!isWalkInformed && !animHandler.isAnimFreezed)
            {
                playerAnim.SetBool("isSitting", false);
                //informWalking?.Invoke();
                playerAnim.Play("Walking", 0, 0);
                playerAnim.SetBool("isPlayerWalking", true);
                isWalkInformed = true;
            }
        }
        else
        {
            playerAnim.SetBool("isPlayerWalking", false);
            animHandler.isAnimFreezed = false;
            isWalkInformed = false;
        }
    }

    public float ReturnDistance()
    {
        return navMeshAgent.remainingDistance;
    }

    private void ResetBools()
    {
        playerAnim.SetBool("isKneeling", false);
        playerAnim.SetBool("isPlanting", false);
        playerAnim.SetBool("isWatering", false);
        planthandler.wasPlantAnimInvoked = false;
    }

    IEnumerator ResetCritical()
    {
        yield return new WaitForSeconds(exhaustionStunTime);
        isCritical = false;
    }

    private void CheckIfFreezed()
    {
        if (animHandler.isAnimFreezed)
        {
            navMeshAgent.isStopped = true;
        }
        else
        {
            navMeshAgent.isStopped = false;
        }
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
                    if(playerAnim.GetBool("isSitting")) {playerAnim.SetBool("isSitting", false);}
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
