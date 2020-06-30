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

    public MusicMeter musicMeter;
    public ObjectiveManager objectiveManager;
    public SoundManager soundManager;
    public MeterLookahead meterLookahead;
    public static bool ghostSpawns;
    public static bool hasFinishedSpawning;
    private GameObject[] spawnedSpheres;
    int spawnBeatCounter;
    int spawnProgressCounter;

    bool spawningSubscribed;

    public void StartNewSpawnSequence(bool lookahead)
    {
        if (lookahead)
        {
            soundManager.newSpawnSequence = true;
            //print("newSeq");
            SubscribeSpawningToMeter();
        }
        else
        {
            spawnZones = LevelManager.spawnZones;
            for (int i = 0; i < spawnZones.Length; i++)
            {
                spawnZones[i].LoadSpawnZone(objectiveManager);
            }
            SpawnAllGhostsAtOnce();
        }
    }
    public void ReloadSpawnSequence(bool lookahead)
    {
        if (lookahead)
        {
            soundManager.newSpawnSequence = false;
            SubscribeSpawningToMeter();
        }
        else
        {
            SpawnAllGhostsAtOnce();
        }
    }
    public void InterruptAndStopSpawnSequence()
    {

    }

    private void SpawnAllGhostsAtOnce()
    {
        hasFinishedSpawning = false;
        spawnedSpheres = new GameObject[LevelManager.spawnZones.Length];
        for (int i = 0; i < LevelManager.spawnZones.Length; i++)
        {
            spawnedSpheres[i] = spawnZones[i].SpawnEnergySphere();
            //spawnedSpheres[i].GetComponent<EnergySphereBehavior>().BecomeGhost();
            objectiveManager.AddGoToArrayOfSpawns(spawnedSpheres[i], i);
        }
        if (!spawningSubscribed)
        {
            SubscribeSpawningToMeterDelayed();
        }
        spawningSubscribed = false;
    }
    private void SubscribeSpawningToMeter()
    {
        spawningSubscribed = true;
        spawnProgressCounter = LevelManager.spawnZones.Length;
        spawnBeatCounter = 0;
        musicMeter.SubscribeEvent(SpawnRealSpheresAtTheRelativeTimings, ref musicMeter.subscribeAnytime);
    }
    private void SubscribeSpawningToMeterDelayed() // when there hasn't been time enough to trigger lookahead-subscription
    {
        // this tries to catch up with the lookahead-sound timing... is this necessary?
    }

    public void SpawnRealSpheresAtTheRelativeTimings()
    {
        spawnBeatCounter++;
        for (int i = 0; i < LevelManager.spawnZones.Length; i++)
        {
            if (spawnBeatCounter == LevelManager.spawnTimings[i])
            {
                spawnProgressCounter--;
                
                soundManager.OrbSpawn(i);

                StartCoroutine(LookaheadDelayBecomeAlive(i));
            }
        }
        if (spawnProgressCounter <= 0)
        {
            musicMeter.UnsubscribeEvent(SpawnRealSpheresAtTheRelativeTimings, ref musicMeter.subscribeAnytime); // unsubscribe when levelFail/objCompl too
            hasFinishedSpawning = true; 
        }
    }
    IEnumerator LookaheadDelayBecomeAlive(int i)
    {
        yield return new WaitForSeconds(meterLookahead.lookaheadFactor * musicMeter.secondsPerBeatDiv);
        //yield return new WaitForSeconds(meterLookahead.lookaheadFactor * musicMeter.secondsPerBeatDiv - MusicMeter.threshHitDelay);
        spawnedSpheres[i].GetComponent<EnergySphereBehavior>().BecomeAlive();
        if (GameManager.inTutorial && !TutorialUI.textShown1)
        {
            FindObjectOfType<TutorialUI>().ShowTextFirstSpawn();
        }
    }
}