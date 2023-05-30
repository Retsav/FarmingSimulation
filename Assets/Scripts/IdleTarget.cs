using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleTarget : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private ExhaustionHandler exhaustionHandler;
    public bool isResting;

    private bool hasSitInformed;
    
    public delegate void InformSitAnimation();
    public static InformSitAnimation informSit;

    private bool debugSit;

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private AnimationHandler animHandler;

    
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
            if (!hasSitInformed)
            {
                Debug.Log("Invoking Sitting");
                informSit?.Invoke();
                hasSitInformed = true;
            }

            if (!playerAnimator.GetBool("isSitting") && !debugSit)
            {
                playerAnimator.Play("Sit", 0, 0);
                debugSit = true;
            }
            isResting = true;
            if (exhaustionHandler.exhaustionRemaining >= exhaustionHandler.exhaustionMax) return;
            exhaustionHandler.exhaustionRemaining += Time.deltaTime * exhaustionHandler.restMultiplier;
        }
        else
        {
            debugSit = false;
            isResting = false;
            hasSitInformed = false;
        }
    }
}
