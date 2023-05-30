using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimationHandler : MonoBehaviour
{
    private Animator animator;
    private PlantHandler plantHandler;
    [SerializeField] public bool isStanded = true;
    [SerializeField] public bool isAnimFreezed;
    [SerializeField] public bool isCriticalKneel;

    private bool wasPlantInvoked;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        plantHandler = transform.parent.GetComponent<PlantHandler>();
    }
    private void OnEnable()
    {
        PlayerNavMesh.informWalking += Walk;
        PlayerNavMesh.informStanding += StandUp;
        IdleTarget.informSit += Sit;
        PlantHandler.informPlantAnim += Plant;
        PlantHandler.informDonePlantingAnim += PlantStandUp;
        PlantWater.informHydrateAnim += HydrateAnim;
        PlantWater.informResetAnim += ResetAnims;
        PlantWater.informHarvestAnim += HarvestAnim;
        PlantWater.informDetoxing += ToxicAnim;
    }

    private void OnDisable()
    {
        PlayerNavMesh.informWalking -= Walk;
        PlayerNavMesh.informStanding -= StandUp;
        IdleTarget.informSit -= Sit;
        PlantHandler.informPlantAnim -= Plant;
        PlantHandler.informDonePlantingAnim -= PlantStandUp;
        PlantWater.informHydrateAnim -= HydrateAnim;
        PlantWater.informResetAnim -= ResetAnims;
        PlantWater.informHarvestAnim -= HarvestAnim;
        PlantWater.informDetoxing -= ToxicAnim;
    }

    private void Sit()
    {
        animator.SetBool("isPlayerWalking", false);
        animator.SetBool("isSitting", true);
        isStanded = false;
    }

    private void ToxicAnim()
    {
        animator.SetBool("isDetoxing", true);
    }

    private void HarvestAnim()
    {
        animator.SetBool("isPickingFruit", true);
    }
    
    private void ResetAnims()
    { 
        animator.SetBool("isWatering", false);
    }

    private void PlantStandUp()
    {
        animator.SetBool("isKneeling", false);
        animator.SetBool("isPlanting", false);
        plantHandler.wasPlantAnimInvoked = false;
    }
    private void StandUp()
    {
        animator.SetBool("isSitting", false);
    }

    private void HydrateAnim()
    {
        animator.SetBool("isWatering", true);
    }

    private void Plant()
    {
        if (!wasPlantInvoked)
        {
            Debug.Log("Planting Invoke");
            animator.SetBool("isKneeling", true);
            animator.SetBool("isPlayerWalking", false);
            wasPlantInvoked = true;
        }
        wasPlantInvoked = false;
    }

    public void AlertObservers(string message)
    {
        if (message.Equals("SittingAnimationEnded"))
        {
            isStanded = true;
        }

        if (message.Equals("KneeledDown"))
        {
            Debug.Log("Knelled Down Message");
            animator.SetBool("isPlanting", true);
            animator.SetBool("isKneeling", false);
        }

        if (message.Equals("StopMotionStart"))
        {
            isAnimFreezed = true;
        }

        if (message.Equals("StopMotionEnd"))
        {
            isAnimFreezed = false;
            animator.SetBool("isPlayerWalking", true);
            isCriticalKneel = false;
        }

        if (message.Equals("DebugKneeling"))
        {
            if (animator.GetBool("isKneeling"))
            {
                animator.SetBool("isKneeling", false);
            }
        }

        if (message.Equals("AfterKneel"))
        {
            isCriticalKneel = true;
        }
    }
    private void Walk()
    {
        if (isStanded)
        {
            animator.SetBool("isPlayerWalking", true);
        }
    }
}
