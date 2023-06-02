using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ChickenBehaviour : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Transform targetPoint;
    private Animator animator;

    private Image healthBar;

    private bool isOnDamageDelay;

    [SerializeField] private float damageDelay = 1f;
    
    [SerializeField] private float chickenHealth = 3f;
    [SerializeField] private float chickenHealthMax = 3f;
    [SerializeField] private List<GameObject> chickenPatrolPoints;

    private void OnEnable()
    {
        FoxBehaviour.attack += GetDamage;
    }

    private void GetDamage(Transform chicken)
    {
        if (!chicken == transform) return;
        if (!isOnDamageDelay)
        {
            chickenHealth--;
            isOnDamageDelay = true;
            StartCoroutine(EndDelay());
        }
    }

    IEnumerator EndDelay()
    {
        yield return new WaitForSeconds(damageDelay);
        if (isOnDamageDelay) 
            isOnDamageDelay = false;
    }

    private void Awake()
    {
        chickenHealth = chickenHealthMax;
        navMeshAgent = GetComponent<NavMeshAgent>();
        healthBar = transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Image>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        SetAsTargetDestination(GetRandomDestination());
    }

    private void Update()
    {
        WalkAnimation();
        if (navMeshAgent.transform.position.x == targetPoint.position.x)
        {
            SetAsTargetDestination(GetRandomDestination());
        }
        healthBar.fillAmount = GetHealthNormalized();
    }

    private void WalkAnimation()
    {
        if (navMeshAgent.remainingDistance > 0.1f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private float GetHealthNormalized()
    {
        return chickenHealth / chickenHealthMax;
    }

    private void SetAsTargetDestination(GameObject destination)
    {
        targetPoint = destination.transform;
        navMeshAgent.destination = targetPoint.position;
    }

    private GameObject GetRandomDestination()
    {
        var randomDestination = chickenPatrolPoints.OrderBy(_ => Guid.NewGuid()).First();
        return randomDestination;
    }
}