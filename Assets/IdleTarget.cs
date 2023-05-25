using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleTarget : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (player.transform.position.x == transform.position.x)
        {
            Debug.Log("Dodaj Exhaustion");
        }
    }
}
