using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleTarget : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private ExhaustionHandler exhaustionHandler;
    public bool isResting;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        exhaustionHandler = player.GetComponent<ExhaustionHandler>();
    }

    private void Update()
    {
        CheckIfResting();
    }

    private void CheckIfResting()
    {
        if (player.transform.position.x == transform.position.x)
        {
            isResting = true;
            if (exhaustionHandler.exhaustionRemaining >= exhaustionHandler.exhaustionMax) return;
            exhaustionHandler.exhaustionRemaining += Time.deltaTime * exhaustionHandler.restMultiplier;
        }
        else
        {
            isResting = false;
        }
    }
}
