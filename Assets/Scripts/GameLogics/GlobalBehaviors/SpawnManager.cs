using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnManager : MonoBehaviour
{
    public MusicMeter.MeterCondition spawnBeatInterval;
    [HideInInspector]
    public SpawnZone[] spawnZones;

    public float spawnFrequency;
    public float spawnFreqRandomness;
    public bool manualSpawn;

    MusicMeter musicMeter;
    ObjectiveManager objectiveManager;
    SoundManager soundManager;
    public static bool ghostSpawns;
    public static bool hasFinishedSpawning;
    private GameObject[] spawnedSpheres;
    int spawnBeatCounter;
    int spawnProgressCounter;

    private void Awake()
    {
        musicMeter = FindObjectOfType<MusicMeter>();
        objectiveManager = FindObjectOfType<ObjectiveManager>();
        soundManager = FindObjectOfType<SoundManager>();
    }
    public void StartNewSpawnSequence()
    {
        soundManager.newSpawnSequence = true;
        spawnZones = LevelManager.spawnZones;
        
        for (int i = 0; i < spawnZones.Length; i++)
        {
            spawnZones[i].LoadSpawnZone();
        }
        SpawnAllGhostsAtOnce();
    }
    public void ReloadSpawnSequence()
    {
        soundManager.newSpawnSequence = false;
        SpawnAllGhostsAtOnce();
    }
    private void SpawnAllGhostsAtOnce()
    {
        hasFinishedSpawning = false;
        spawnedSpheres = new GameObject[LevelManager.spawnZones.Length];
        for (int i = 0; i < LevelManager.spawnZones.Length; i++)
        {
            spawnedSpheres[i] = spawnZones[i].SpawnEnergySphere();
            //spawnedSpheres[i].GetComponent<EnergySphereBehavior>().BecomeGhost();
            objectiveManager.AddToSpawned(spawnedSpheres[i], i);
        }
        SubscribeSpawningToMeter();
    }
    private void SubscribeSpawningToMeter()
    {
        spawnProgressCounter = LevelManager.spawnZones.Length;
        spawnBeatCounter = 0;
        musicMeter.SubscribeEvent(SpawnRealSpheresAtTheRelativeTimings, ref musicMeter.subscribeAnytime);
    }

    private void SpawnRealSpheresAtTheRelativeTimings()
    {
        spawnBeatCounter++;
        for (int i = 0; i < LevelManager.spawnZones.Length; i++)
        {
            if (spawnBeatCounter == LevelManager.spawnTimings[i])
            {
                spawnProgressCounter--;
                spawnedSpheres[i].GetComponent<EnergySphereBehavior>().BecomeAlive();
                soundManager.SphereSpawn(i);
            }
        }
        if (spawnProgressCounter <= 0)
        {
            musicMeter.UnsubscribeEvent(SpawnRealSpheresAtTheRelativeTimings, ref musicMeter.subscribeAnytime);
            hasFinishedSpawning = true;
        }
    }
}