using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenRespawnManager : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 15f;
    
    private void OnEnable()
    {
        ChickenBehaviour.death += HandleRespawn;
    }


    private void HandleRespawn(Transform chicken)
    {
        StartCoroutine(Respawn(chicken));
    }

    IEnumerator Respawn(Transform chickenToSpawn)
    {
        yield return new WaitForSeconds(respawnDelay);
        chickenToSpawn.gameObject.SetActive(true);    
    }
}
