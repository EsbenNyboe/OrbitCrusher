using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    // split in two: EnergySphereBehavior & ObjectiveManager

    public GameObject energySpherePrefab;
    [HideInInspector]
    public GameObject[] energySpheresSpawned;
    [HideInInspector]
    public GameObject[] collidedSpheresOnTarget;
    [HideInInspector]
    public GameObject[] collidedSpheresOnTraps;
    int amountOnTarget;
    int amountOnOthers;

    public SpawnManager spawnManager;
    public GameManager gameManager;
    public HealthBar healthBar;
    public NodeBehavior nodeBehavior;


    private void Start()
    {
        //spawnManager = FindObjectOfType<SpawnManager>();
        //gameManager = FindObjectOfType<GameManager>();
        //healthBar = FindObjectOfType<HealthBar>();
        //nodeBehavior = FindObjectOfType<NodeBehavior>();
    }
    public void ResetSphereArrays()
    {
        collidedSpheresOnTarget = new GameObject[0];
        collidedSpheresOnTraps = new GameObject[0];
        amountOnTarget = 0;
        amountOnOthers = 0;
        energySpheresSpawned = new GameObject[LevelManager.spawnZones.Length];
    }

    public void AddToSpawned(GameObject go, int index)
    {
        energySpheresSpawned[index] = go;
    }
    public void NewCollisionOnTarget(GameObject collidedSphere)
    {
        amountOnTarget++;
        ExpandArrayWithOneNewGoAddition(collidedSphere, amountOnTarget, ref collidedSpheresOnTarget);
        if (collidedSpheresOnTarget.Length == energySpheresSpawned.Length)
            nodeBehavior.HighlightCompletedTarget(LevelManager.targetNodes[LevelManager.levelObjectiveCurrent]);
        gameManager.UpdateEnergyHealth(1);
        healthBar.UpdateHealthbarOnCollision(true);
    }
    public void NewCollisionOther(GameObject collidedSphere)
    {
        amountOnOthers++;
        ExpandArrayWithOneNewGoAddition(collidedSphere, amountOnOthers, ref collidedSpheresOnTraps);
        gameManager.UpdateEnergyHealth(-1);
        healthBar.UpdateHealthbarOnCollision(false);
    }
    private void ExpandArrayWithOneNewGoAddition(GameObject go, int amount, ref GameObject[] array) // fix
    {
        GameObject[] oldArray = array;
        array = new GameObject[amount];
        for (int i = 0; i < amount; i++)
        {
            if (oldArray.Length > i)
                array[i] = oldArray[i];
        }
        array[amount - 1] = go;
    }
    public bool HasAllEnergySpheresHitTheTargetNode()
    {
        if (collidedSpheresOnTarget.Length >= spawnManager.spawnZones.Length)
        {
            return true;
        }
        return false;
    }
    public void RemoveEnergySpheres()
    {
        for (int i = 0; i < energySpheresSpawned.Length; i++)
        {
            if (energySpheresSpawned[i] != null)
            {
                energySpheresSpawned[i].GetComponent<EnergySphereBehavior>().CollisionParticleEffectBad();
                energySpheresSpawned[i].GetComponent<EnergySphereBehavior>().KillTheLights();
                energySpheresSpawned[i].GetComponent<EnergySphereDeath>().StopRemainingParticlesThenDestroy();
                energySpheresSpawned[i].GetComponentInChildren<EnergySphereCollision>().DisableCollider();
            }
        }
    }
}