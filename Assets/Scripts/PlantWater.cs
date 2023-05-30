using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlantWater : MonoBehaviour
{
    [Header("Dehydration Settings")]
    [SerializeField] public float timeFull = 60f;
    [SerializeField] public float timeBeforeDehydration;
    [SerializeField] public float dehydrationMultiplier = 1f;
    [SerializeField] public float hydrationMultiplier = 1f;
    [SerializeField] public float timeBeforeCryingForHelp = 30f;
    [SerializeField] private float leaveTime = 60f;
    [SerializeField] public bool isTimerRunning;
    [SerializeField] private bool isBeingHydrated;
    [SerializeField] private bool hasCried;
    [Header("Harvest Settings")]
    [SerializeField] private bool isHarvestInformed;
    private const float timeHarvesting = 4f;
    [SerializeField] private bool hasApples;
    [SerializeField] private float timeBeforeHarvest;
    [Header("Toxicity Settings")]
    [SerializeField] private float toxicCuringMultiplier = 1f;
    [SerializeField] private float toxicIncreaseMultiplier = 1f;
    [SerializeField] private float timeBeforeFullToxic;
    [SerializeField] private float toxicFullMeter = 15f;
    [SerializeField] private bool isToxic;
    [SerializeField] private float toxicRollInterval = 10f;
    [SerializeField] private bool hasToxicCried = false;
    

    private Image hydrationBar;
    private Image harvestBar;
    private Image toxicBar;

    private AnimationHandler animHandler;
    private Animator animator;


    private GameObject player;

    private int GrowLevel = 1;

    public delegate void InformPlayer(Transform plant);
    public static InformPlayer cryForHelp;
    public static InformPlayer informGoodStatus;
    public static InformPlayer informDeath;
    public static InformPlayer informHarvesting;
    public static InformPlayer informRemoveSeed;
    public static InformPlayer informCleanRemoval;

    private void Awake()
    {
        hydrationBar = transform.GetChild(2).GetComponent<Image>();
        harvestBar = transform.parent.GetChild(1).GetChild(2).GetComponent<Image>();
        toxicBar = transform.parent.GetChild(2).GetChild(2).GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player");
        animHandler = player.transform.GetChild(1).GetComponent<AnimationHandler>();
        animator = player.transform.GetChild(1).GetComponent<Animator>();
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
        CleanUpMissingTransform();
        Intoxicate();
        hydrationBar.fillAmount = GetPlantTimeNormalized();
        //CheckStatuses();
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

    private void CleanUpMissingTransform()
    {
        if (hasApples && isHarvestInformed)
        {
            if (hasCried && (timeBeforeDehydration >= timeFull) || isToxic && (timeBeforeFullToxic >= toxicFullMeter))
            {
                informCleanRemoval?.Invoke(this.GameObject().transform.parent.parent.parent.transform);
            }
        }
    }

    private void Intoxicate()
    {
        var navMesh = player.GetComponent<PlayerNavMesh>();
        if (!isToxic) return;
        if (hasToxicCried == false)
        {
            cryForHelp?.Invoke(this.GameObject().transform.parent.parent.parent.transform);
            hasToxicCried = true;
        }
        transform.parent.GetChild(2).gameObject.SetActive(true);
        if(timeBeforeFullToxic >= toxicFullMeter)
        {
            if (hasCried)
            {
                informCleanRemoval?.Invoke(this.GameObject().transform.parent.parent.parent.transform);
            }
            informDeath?.Invoke(this.GameObject().transform.parent.parent.parent.transform);
            Destroy(transform.parent.parent.GameObject());
        } else if (player.transform.position.x == transform.position.x)
        {
            timeBeforeFullToxic -= Time.deltaTime * toxicCuringMultiplier;
            navMesh.isDetoxicating = true;
            if(timeBeforeFullToxic <= 0)
            {
                isToxic = false;
                hasToxicCried = false;
                timeBeforeFullToxic = 0f;
                navMesh.isDetoxicating = false;
                informGoodStatus?.Invoke(this.GameObject().transform.parent.parent.parent.transform);
                transform.parent.GetChild(2).gameObject.SetActive(false);
            }
        } else
        {
            timeBeforeFullToxic += Time.deltaTime * toxicIncreaseMultiplier;
            navMesh.isDetoxicating = false;
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
        if (!hasCried && !hasToxicCried && animator.GetBool("isPlayerWalking"))
        { 
            cryForHelp?.Invoke(this.GameObject().transform.parent.parent.parent.transform);
            hasCried = true;
        }
    }

    private void CheckApples()
    {
        if(hasApples && !isHarvestInformed && animator.GetBool("isPlayerWalking"))
        {
            isHarvestInformed = true;
            informHarvesting?.Invoke(this.GameObject().transform.parent.parent.parent.transform);
            
        }
    }

    private void CheckPlantStatus()
    {
        if (hasCried && (timeBeforeDehydration >= leaveTime))
        {
            informGoodStatus?.Invoke(this.GameObject().transform.parent.parent.parent.transform);
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
            informGoodStatus?.Invoke(this.GameObject().transform.parent.parent.parent.transform);
            informRemoveSeed?.Invoke(transform.parent.parent.parent.GameObject().transform);
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
                if (hasToxicCried)
                {
                    informCleanRemoval?.Invoke(this.GameObject().transform.parent.parent.parent.transform);
                }
                informDeath?.Invoke(this.GameObject().transform.parent.parent.parent.transform);
                Destroy(transform.parent.parent.GameObject());
            }
        }
    }
    
    public float GetPlantTimeNormalized()
    {
        return timeBeforeDehydration / timeFull;
    }
}
