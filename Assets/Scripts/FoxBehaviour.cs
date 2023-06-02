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
    [SerializeField] private GameObject foxholeLocation;
    
    [SerializeField] private List<GameObject> chickensInRange;

    private float chickenDistance;
    [SerializeField] private bool hasChicken;
    [SerializeField] private float foxCooldown = 10f;
    
    private NavMeshAgent foxNavMesh;
    private Transform closestTarget;

    public delegate void InformChicken(Transform chicken);

    public static InformChicken attack;


    private void Awake()
    {
        foxNavMesh = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        foxNavMesh.destination = penLocation.transform.position;
        ChickenBehaviour.death += RemoveDeadChickenFromList;
        ChickenBehaviour.death += OnChickenKilled;
    }

    private void OnDisable()
    {
        ChickenBehaviour.death -= RemoveDeadChickenFromList;
        ChickenBehaviour.death -= OnChickenKilled;
    }

    private void Update()
    {
        ChooseTarget();
        Debug.Log(foxNavMesh.remainingDistance);
        FoxAttack();
        if (hasChicken)
        {
            ReturnToFoxHole();
        }
    }

    private void ReturnToFoxHole()
    {
        foxNavMesh.SetDestination(foxholeLocation.transform.position);
        if (foxNavMesh.remainingDistance < 0.1f)
        {
            StartCoroutine(FoxCooldown());
        }
    }

    private void FoxAttack()
    {
        if (foxNavMesh.remainingDistance <= 1f && !hasChicken)
        {
            Debug.Log("Atakuje");
            attack(closestTarget);
        }
    }

    private void RemoveDeadChickenFromList(Transform chicken)
    {
        if (chickensInRange.Contains(chicken.GameObject()))
        {
            chickensInRange.Remove(chicken.GameObject());
        }
    }

    private void OnChickenKilled(Transform chicken)
    {
        hasChicken = true;
        transform.GetChild(1).GameObject().SetActive(true);
    }

    public void ChooseTarget()
    {
        if (hasChicken) return;
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

    IEnumerator FoxCooldown()
    {
        yield return new WaitForSeconds(foxCooldown);
        transform.GetChild(1).GameObject().SetActive(false);
        hasChicken = false;
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
