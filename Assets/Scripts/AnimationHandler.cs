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
    }

    private void OnDisable()
    {
        PlayerNavMesh.informWalking -= Walk;
        PlayerNavMesh.informStanding -= StandUp;
        IdleTarget.informSit -= Sit;
        PlantHandler.informPlantAnim -= Plant;
        PlantHandler.informDonePlantingAnim -= PlantStandUp;
    }

    private void Sit()
    {
        animator.SetBool("isPlayerWalking", false);
        animator.SetBool("isSitting", true);
        isStanded = false;
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
