using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        GoToFirstDestination();
    }
    
    void Update()
    {
        GoToNextDestination();
    }

    private void GoToFirstDestination()
    {
        navMeshAgent.destination = targetPoints[i].position;
    }

    private void GoToNextDestination()
    {
        Debug.Log(i);
        Debug.Log(hasAdded);
        if (i >= targetPoints.Count || hasAdded) { return; }
        if (navMeshAgent.transform.position.x == targetPoints[i].position.x)
        {
            i++;
            navMeshAgent.destination = targetPoints[i].position;
            hasAdded = true;
        }
    }
}
