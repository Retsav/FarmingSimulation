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
    [SerializeField] private bool isHarvestInformed;
    private bool isSpawningApples = false;
    [SerializeField] private bool hasCried;
    [SerializeField] private bool hasApples;
    [SerializeField] private float timeBeforeHarvest;
    private const float timeHarvesting = 4f;
    
    [SerializeField] private Image hydrationBar;
    [SerializeField] private Image harvestBar;

    [SerializeField] private GameObject player;

    [SerializeField] private int GrowLevel = 1;

    public delegate void InformPlayer(Transform plant);
    public static InformPlayer cryForHelp;
    public static InformPlayer informGoodStatus;
    public static InformPlayer informDeath;
    public static InformPlayer informHarvesting;

    private void Awake()
    {
        hydrationBar = transform.GetChild(2).GetComponent<Image>();
        harvestBar = transform.parent.GetChild(1).GetChild(2).GetComponent<Image>();
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
        CheckPlantStatus();
        CheckApples();
        hydrationBar.fillAmount = GetPlantTimeNormalized();
    }

    public void CheckRefillHydration()
    {
        if (timeBeforeDehydration == timeFull) return;
        if (player.transform.position.x == transform.position.x && !hasApples)
        {
            Hydrate();
        }
        else if (player.transform.position.x == transform.position.x && hasApples)
        {
            Harvest();  
        } else 
        {
            isBeingHydrated = false;
        }
    }

    private void Hydrate()
    {
        isBeingHydrated = true;
        timeBeforeDehydration += Time.deltaTime * hydrationMultiplier;
    }

    private void CryForHelp()
    {
        if (hasApples) return;
        if (timeBeforeCryingForHelp <= timeBeforeDehydration) return;
        if (!hasCried)
        { 
            cryForHelp?.Invoke(this.GameObject().transform);
            hasCried = true;
        }
    }

    private void CheckApples()
    {
        if(hasApples && !isHarvestInformed)
        {
            isHarvestInformed = true;
            informHarvesting?.Invoke(this.GameObject().transform);
        }
    }

    private void CheckPlantStatus()
    {
        if (hasCried && (timeBeforeDehydration >= leaveTime))
        {
            informGoodStatus?.Invoke(this.GameObject().transform);
            hasCried = false;
            if(GrowLevel < 4)
            {
                Grow();
            } else
            {
                if (hasApples) return;
                StartCoroutine(GrowApples());
            }
        }
    }

    private void Harvest()
    {
        transform.parent.GetChild(1).gameObject.SetActive(true);
        if(timeBeforeHarvest >= timeHarvesting)
        {
            transform.parent.parent.GetChild(GrowLevel).GetChild(1).gameObject.SetActive(false);
            hasApples = false;
            transform.parent.GetChild(1).gameObject.SetActive(false);
            isHarvestInformed = false;
            Degrow();
            informGoodStatus?.Invoke(this.GameObject().transform);
            timeBeforeHarvest = 0f;
        } else
        {
            if(player.transform.position.x == transform.position.x)
            {
                timeBeforeHarvest += Time.deltaTime;
            }
            harvestBar.fillAmount = GetHarvestNormalized();
        }
    }

    private float GetHarvestNormalized()
    {
        return timeBeforeHarvest / timeHarvesting;
    }


    IEnumerator GrowApples()
    {
        yield return new WaitForSeconds(5f);
        transform.parent.parent.GetChild(GrowLevel).GetChild(1).gameObject.SetActive(true);
        hasApples = true;
    }
    private void Degrow()
    {
        transform.parent.parent.GetChild(GrowLevel).gameObject.SetActive(false);
        GrowLevel--;
        transform.parent.parent.GetChild(GrowLevel).gameObject.SetActive(true);
    }


    private void Grow()
    {
           transform.parent.parent.GetChild(GrowLevel).gameObject.SetActive(false);
           GrowLevel++;
           transform.parent.parent.GetChild(GrowLevel).gameObject.SetActive(true);
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
                informDeath?.Invoke(this.GameObject().transform);
                Destroy(transform.parent.parent.GameObject());
            }
        }
    }
    
    public float GetPlantTimeNormalized()
    {
        return timeBeforeDehydration / timeFull;
    }

    IEnumerator KillWithDelay()
    {
        yield return new WaitForSeconds(2f);
        transform.parent.parent.GetChild(GrowLevel).GetChild(1).gameObject.SetActive(false);
    }
}
