using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FoxBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject penLocation;
    [SerializeField] private List<GameObject> chickensInRange;

    private float chickenDistance;

    
    private NavMeshAgent foxNavMesh;
    private Transform closestTarget;

    public delegate void InformChicken(Transform chicken);

    public static InformChicken attack;


    private void Awake()
    {
        foxNavMesh = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        ChooseTarget();
        if (foxNavMesh.remainingDistance < 0.1f)
        {
            attack(closestTarget);
        }
    }

    public void ChooseTarget()
    {
        closestTarget = null;
        float closestTargetDistance = float.MaxValue;
        NavMeshPath path = new NavMeshPath();
        for (int i = 0; i < chickensInRange.Count; i++)
        {
            if(chickensInRange[i] is null) { continue;}

            if (NavMesh.CalculatePath(transform.position, chickensInRange[i].transform.position, foxNavMesh.areaMask, path))
            {
                float distance = Vector3.Distance(transform.position, path.corners[0]);
                for (int j = 1; j < path.corners.Length; j++)
                {
                    distance += Vector3.Distance(path.corners[j - 1], path.corners[j]);
                }
                if (distance < closestTargetDistance)
                {
                    closestTargetDistance = distance;
                    closestTarget = chickensInRange[i].transform;
                }
            }
        }
        if (closestTarget != null)
        {
            foxNavMesh.SetDestination(closestTarget.position);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Detected trigger");
        if (other.GameObject().layer != 6) return;
        if (chickensInRange.Contains(other.GameObject())) return;
        chickensInRange.Add(other.GameObject());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GameObject().layer != 6) return;
        if (!chickensInRange.Contains(other.GameObject())) return;
        chickensInRange.Remove(other.GameObject());
    }

    private void GoToPen()
    {
        foxNavMesh.destination = penLocation.transform.position;
    }
}
