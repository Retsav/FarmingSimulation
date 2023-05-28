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
    [SerializeField] private bool hasToxicCried = false;

    [SerializeField] private Image hydrationBar;
    [SerializeField] private Image harvestBar;
    [SerializeField] private Image toxicBar;

    [SerializeField] private float timeBeforeFullToxic;
    [SerializeField] private float toxicFullMeter = 15f;
    [SerializeField] private bool isToxic;
    [SerializeField] private float toxicRollInterval = 10f;

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
        toxicBar = transform.parent.GetChild(2).GetChild(2).GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnEnable()
    {
        timeBeforeDehydration = timeFull;
        isTimerRunning = true;
    }

    public void Start()
    {
        StartCoroutine(ToxicRoll());
    }

    private void Update()
    {
        CheckRefillHydration();
        CalculateTimer();
        CryForHelp();
        CheckPlantStatus();
        CheckApples();
        Intoxicate();
        hydrationBar.fillAmount = GetPlantTimeNormalized();
    }

    public void CheckRefillHydration()
    {
        if (timeBeforeDehydration == timeFull) return;
        if (player.transform.position.x == transform.position.x && !hasApples && !isToxic)
        {
            Hydrate();
        }
        else if (player.transform.position.x == transform.position.x && hasApples && !isToxic)
        {
            Harvest();  
        } else 
        {
            isBeingHydrated = false;
        }
    }

    private void Intoxicate()
    {
        if (!isToxic) return;
        if (hasToxicCried == false)
        {
            cryForHelp?.Invoke(this.GameObject().transform);
            hasToxicCried = true;
        }
        transform.parent.GetChild(2).gameObject.SetActive(true);
        if(timeBeforeFullToxic >= toxicFullMeter)
        {
            informDeath?.Invoke(this.GameObject().transform);
            transform.parent.parent.GameObject().SetActive(false);
        } else if (player.transform.position.x == transform.position.x)
        {
            timeBeforeFullToxic -= Time.deltaTime;
            if(timeBeforeFullToxic <= 0)
            {
                isToxic = false;
                hasToxicCried = false;
                timeBeforeFullToxic = 0f;
                informGoodStatus?.Invoke(this.GameObject().transform);
                transform.parent.GetChild(2).gameObject.SetActive(false);
            }
        } else
        {
            timeBeforeFullToxic += Time.deltaTime;
        }
        toxicBar.fillAmount = GetToxicNormalized();
    }

    IEnumerator ToxicRoll()
    {
        while(true)
        {
            yield return new WaitUntil(() => !isToxic);
            yield return new WaitForSeconds(toxicRollInterval);
            var roll = MathF.Floor(UnityEngine.Random.Range(0f, 101f));
            Debug.Log("Rolled toxicity for: " + this.transform.parent.parent.GameObject().name + ". Roll was: " + roll);
            if(roll >= 95f)
            {
                isToxic = true;
            }
        }
    }

    private void Hydrate()
    {
        isBeingHydrated = true;
        if(timeBeforeDehydration <= timeFull)
        {
            timeBeforeDehydration += Time.deltaTime * hydrationMultiplier;   
        }
    }

    private void CryForHelp()
    {
        if (hasApples) return;
        if (timeBeforeCryingForHelp <= timeBeforeDehydration) return;
        if (!hasCried && !hasToxicCried)
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

    private float GetToxicNormalized()
    {
        return timeBeforeFullToxic / toxicFullMeter;
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
                transform.parent.parent.GameObject().SetActive(false);
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
