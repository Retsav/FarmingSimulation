using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlantWater : MonoBehaviour
{
    [SerializeField] public float timeFull = 60f;
    [SerializeField] public float timeBeforeDehydration;
    [SerializeField] public float dehydrationMultiplier = 1f;
    [SerializeField] public float hydrationMultiplier = 1f;
    [SerializeField] public float timeBeforeCryingForHelp = 30f;
    [SerializeField] private float leaveTime = 60f;
    [SerializeField] public bool isTimerRunning;
    [SerializeField] private bool isBeingHydrated;
    [SerializeField] private bool hasCried;
    
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject player;

    public delegate void InformPlayer(Transform plant);
    public static InformPlayer cryForHelp;
    public static InformPlayer informGoodStatus;

    private void Awake()
    {
        barImage = transform.Find("Bar").GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void Start()
    {
        isTimerRunning = true;
        timeBeforeDehydration = timeFull;
    }

    private void Update()
    {
        CheckRefillHydration();
        CalculateTimer();
        CryForHelp();
        CheckHydrationStatus();
        barImage.fillAmount = GetTimeNormalized();
        Debug.Log(timeBeforeDehydration);
    }

    public void CheckRefillHydration()
    {
        if (timeBeforeDehydration == timeFull) return;
        if (player.transform.position.x == transform.position.x)
        {
            isBeingHydrated = true;
            timeBeforeDehydration += Time.deltaTime * hydrationMultiplier;
        }
        else
        {
            isBeingHydrated = false;
        }
    }

    private void CryForHelp()
    {
        if (timeBeforeCryingForHelp <= timeBeforeDehydration) return;
        if (!hasCried)
        { 
            cryForHelp?.Invoke(this.GameObject().transform);
            hasCried = true;
        }
    }

    private void CheckHydrationStatus()
    {
        if (hasCried && (timeBeforeDehydration >= leaveTime))
        {
            informGoodStatus?.Invoke(this.GameObject().transform);
            hasCried = false;
        }
    }

    public void CalculateTimer()
    {
        if (isTimerRunning)
        {
            if (timeBeforeDehydration >= 0)
            {
                if (isBeingHydrated) return;
                timeBeforeDehydration -= Time.deltaTime * dehydrationMultiplier;
            }
            else
            {
                timeBeforeDehydration = 0f;
                isTimerRunning = false;
                Destroy(gameObject);
            }
        }
    }
    
    public float GetTimeNormalized()
    {
        return timeBeforeDehydration / timeFull;
    }
}
