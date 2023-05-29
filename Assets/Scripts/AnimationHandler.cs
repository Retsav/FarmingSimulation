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
    [SerializeField] public bool isStanded = true;
    [SerializeField] private float rotationX = 90f;
    [SerializeField] private float rotationY = 20f;

    [SerializeField] private Quaternion currentRotation;
    private Vector3 currentRotationEulerAngles;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentRotation = transform.GetChild(0).rotation;
        Vector3 currentRotationEulerAngles = currentRotation.eulerAngles;
    }

    private void OnEnable()
    {
        PlayerNavMesh.informWalking += Walk;
        PlayerNavMesh.informStanding += StandUp;
        IdleTarget.informSit += Sit;
    }

    private void OnDisable()
    {
        PlayerNavMesh.informWalking -= Walk;
        PlayerNavMesh.informStanding -= StandUp;
        IdleTarget.informSit -= Sit;
    }

    private void Sit()
    {
        animator.SetBool("isPlayerWalking", false);
        animator.SetBool("isSitting", true);
        isStanded = false;
    }
    private void StandUp()
    {
        animator.SetBool("isSitting", false);
    }

    public void AlertObservers(string message)
    {
        Debug.Log("Received message");
        if (message.Equals("SittingAnimationEnded"))
        {
            isStanded = true;
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
