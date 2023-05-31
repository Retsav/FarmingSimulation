using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private List<AudioClip> clipList;


    private bool isPlayingSound;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        PlayerNavMesh.informStopSounds += AllSoundEnd;
        PlantHandler.informPlantSoundStart += PlantSoundStart;
        PlantHandler.informPlantSoundEnd += PlantSoundEnd;
        PlantWater.informHydrateSoundStart += HydrateSound;
        PlantWater.informHydrateSoundEnd += HydrateSoundEnd;
        PlantWater.informToxicSoundStart += DetoxSoundStart;
        PlantWater.informToxicSoundEnd += DetoxSoundEnd;
        PlantWater.informHarvestSoundStart += HarvestSoundStart;
        PlantWater.informHarvestSoundEnd += HarvestSoundEnd;
    }

    private void OnDisable()
    {
        PlayerNavMesh.informStopSounds -= AllSoundEnd;
        PlantHandler.informPlantSoundStart -= PlantSoundStart;
        PlantHandler.informPlantSoundEnd -= PlantSoundEnd;
        PlantWater.informHydrateSoundStart -= HydrateSound;
        PlantWater.informHydrateSoundEnd -= HydrateSoundEnd;
        PlantWater.informToxicSoundStart -= DetoxSoundStart;
        PlantWater.informToxicSoundEnd -= DetoxSoundEnd;
        PlantWater.informHarvestSoundStart -= HarvestSoundStart;
        PlantWater.informHarvestSoundEnd -= HarvestSoundEnd;
    }


    private void AllSoundEnd()
    {
        audioSource.Stop();
        audioSource.loop = false;
        isPlayingSound = false;
    }

    private void HydrateSound()
    {
        if(!isPlayingSound)
        {
            audioSource.clip = clipList[1];
            audioSource.Play();
            audioSource.loop = true;
            isPlayingSound = true;
        }
    }

    private void HydrateSoundEnd()
    {
        audioSource.Stop();
        audioSource.loop = false;
        isPlayingSound = false;
    }


    private void PlantSoundStart()
    {
        if(!isPlayingSound)
        {
            audioSource.clip = clipList[0];
            audioSource.Play();
            audioSource.loop = true;
            isPlayingSound = true;
        }
    }

    private void HarvestSoundStart()
    {
        if (!isPlayingSound)
        {
            audioSource.clip = clipList[2];
            audioSource.Play();
            audioSource.loop = true;
            isPlayingSound = true;
        }
    }

    private void DetoxSoundStart()
    {
        if (!isPlayingSound)
        {
            audioSource.clip = clipList[0];
            audioSource.Play();
            audioSource.loop = true;
            isPlayingSound = true;
        }
    }

    private void HarvestSoundEnd()
    {
        audioSource.Stop();
        audioSource.loop = false;
        isPlayingSound = false;
    }

    private void DetoxSoundEnd()
    {
        audioSource.Stop();
        audioSource.loop = false;
        isPlayingSound = false;
    }

    private void PlantSoundEnd()
    {
        audioSource.Stop();
        audioSource.loop = false;
        isPlayingSound = false;
    }

}
