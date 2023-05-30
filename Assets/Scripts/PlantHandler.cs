using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlantHandler : MonoBehaviour
{
    [SerializeField] private List<Transform> plantSpaces;
    private GameObject player;
    private PlayerNavMesh playerNavMesh;
    [SerializeField] private Image actionfillBar;
    [SerializeField] private GameObject actionBar;

    [SerializeField] private float raycastDistance = .5f;
    [SerializeField] private LayerMask pointsLayer;
    private Ray ray;
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private GameObject treeplantPrefab;

    [SerializeField] private float timeBeforeSeeded;
    [SerializeField] private float maxTimeSeed = 5f;
    public bool wasPlantAnimInvoked;
    
    public delegate void InformSeed(Transform plantSpace);
    public static InformSeed informSeed;
    public static InformSeed informDoneSeeding;

    public delegate void InformPlantAnimations();

    private RaycastHit hit;

    public static InformPlantAnimations informPlantAnim;
    public static InformPlantAnimations informDonePlantingAnim;

    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private Animator playerAnimator;
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerNavMesh = player.GetComponent<PlayerNavMesh>();
    }

    private void Update()
    {
        CheckIfEmpty();
        PlantSeeds();
        KneelFix();
        ray = new Ray(transform.position, Vector3.down);
        //Debug.DrawRay(transform.position, Vector3.down * raycastDistance, Color.green );
    }

    private void OnDestroy()
    {
        
        wasPlantAnimInvoked = false;
        if (playerAnimator.GetBool("isKneeling"))
        {
            playerAnimator.Play("Kneeling To Standing", 0, 0f);
        }
    }

    private void PlantSeeds()
    {
        foreach (var plant in plantSpaces)
        {
            if (plant.childCount == 0)
            {
                if (player.transform.position.x == plant.transform.position.x)
                {
                    if (!wasPlantAnimInvoked)
                    {
                        if (plant.childCount > 0) return;
                        informPlantAnim?.Invoke();
                        wasPlantAnimInvoked = true;
                    } 
                    if (playerAnimator.GetBool("isPlanting"))
                    {
                        actionBar.SetActive(true);
                        if (maxTimeSeed >= timeBeforeSeeded)
                        {
                            timeBeforeSeeded += Time.deltaTime;
                        }
                        else 
                        { 
                            informDoneSeeding?.Invoke(plant.GameObject().transform);
                            if (plant.CompareTag("tree"))
                            { 
                                Instantiate(treeplantPrefab, plant.position, Quaternion.identity, plant.transform);
                            }
                            else
                            {
                                Instantiate(plantPrefab, plant.position, Quaternion.identity, plant.transform);
                            }
                            wasPlantAnimInvoked = false;
                            playerAnimator.SetBool("isPlanting", false);
                            informDonePlantingAnim?.Invoke();
                            timeBeforeSeeded = 0f;
                            actionBar.SetActive(false);
                        }
                    }
                    actionfillBar.fillAmount = GetSeedTimeNormalized();
                }
            }
        }
        if (!playerAnimator.GetBool("isPlanting"))
        {
            actionBar.SetActive(false);
        }
        if (wasPlantAnimInvoked && !playerAnimator.GetBool("isKneeling"))
        {
            Debug.Log("Uratowałem przed shitem");
            wasPlantAnimInvoked = false;
        }
    }

    private void KneelFix()
    {
        if (wasPlantAnimInvoked && !playerAnimator.GetBool("isKneeling"))
        {
            StartCoroutine(FixKneeling());
        }
    }

    IEnumerator FixKneeling()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("Tried fixing Kneeling");
        wasPlantAnimInvoked = false;
    }

    private float GetSeedTimeNormalized()
    {
        return timeBeforeSeeded / maxTimeSeed;
    }

    private void CheckIfEmpty()
    {
        foreach (var plant in plantSpaces)
        {
            if (plant.childCount > 0) return;
            informSeed?.Invoke(plant.GameObject().transform);
        }
    }
}
