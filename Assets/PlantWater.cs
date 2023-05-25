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

    [SerializeField] private int GrowLevel = 1;

    public delegate void InformPlayer(Transform plant);
    public static InformPlayer cryForHelp;
    public static InformPlayer informGoodStatus;
    public static InformPlayer informDeath;
    public static InformPlayer informHarvesting;

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
        barImage.fillAmount = GetPlantTimeNormalized();
    }

    public void CheckRefillHydration()
    {
        if (timeBeforeDehydration == timeFull) return;
        if (player.transform.position.x == transform.position.x)
        {
            isBeingHydrated = true;
            timeBeforeDehydration += Time.deltaTime * hydrationMultiplier;
        }
        else if (player.transform.position.x == transform.position.x && GrowLevel >= 4)
        {
            CheckIfHarvestable();
        }
        else
        {
            isBeingHydrated = false;
        }
    }

    private void Harvest()
    {
        StartCoroutine(KillWithDelay());
        informDeath?.Invoke(this.GameObject().transform);
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
            Grow();
            CheckIfHarvestable();
        }
    }

    private void Grow()
    {
        if (GrowLevel >= 4)
        {
            CheckIfHarvestable();
        }
        else
        {
            transform.parent.parent.GetChild(GrowLevel).gameObject.SetActive(false);
            GrowLevel++;
            transform.parent.parent.GetChild(GrowLevel).gameObject.SetActive(true);
            CheckGrowApples();
        }
    }

    private void CheckGrowApples()
    {
        if(GrowLevel >= 4)
        {
            StartCoroutine(AppleSpawnDelay());
        }
    }


    private void CheckIfHarvestable()
    {
        if (GrowLevel >= 4)
        {
            if (!transform.parent.parent.GetChild(GrowLevel).GetChild(1).gameObject.active)
            {
                CheckGrowApples();
            }
            else
            {
                Harvest();
            }             //Sprawdz czy masz japca, jezeli tak to je zbierz, je¿eli nie, to zadeklaruj rozpoczêcie nowych jab³ek;
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
                informDeath?.Invoke(this.GameObject().transform);
                Destroy(gameObject);
            }
        }
    }
    
    public float GetPlantTimeNormalized()
    {
        return timeBeforeDehydration / timeFull;
    }

    IEnumerator AppleSpawnDelay()
    {
        yield return new WaitForSeconds(10f);
        transform.parent.parent.GetChild(GrowLevel).GetChild(1).gameObject.SetActive(true);
        informHarvesting?.Invoke(this.GameObject().transform);
    }

    IEnumerator KillWithDelay()
    {
        yield return new WaitForSeconds(2f);
        transform.parent.parent.GetChild(GrowLevel).GetChild(1).gameObject.SetActive(false);
    }
}
