using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ChickenBehaviour : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Transform targetPoint;
    private Animator animator;

    [SerializeField] private List<GameObject> chickenPatrolPoints;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
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
