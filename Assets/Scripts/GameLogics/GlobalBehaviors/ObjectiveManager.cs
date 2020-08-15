using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public GameObject energySpherePrefab;
    [HideInInspector]
    public GameObject[] energySpheresSpawned;
    [HideInInspector]
    public GameObject[] collidedSpheresOnTarget;
    [HideInInspector]
    public GameObject[] collidedSpheresOnTraps;
    public static int amountOnTarget;
    public static int amountOnTraps;
    public static int amountSpawned;

    public SpawnManager spawnManager;
    public GameManager gameManager;
    public HealthBar healthBar;
    public NodeBehavior nodeBehavior;
    public TutorialUI tutorialUI;

    private void Update()
    {
        if (gameManager.levelLoadDeveloperMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                EnergySphereBehavior.draggedObject = energySpheresSpawned[energySpheresSpawned.Length - 1].GetComponent<EnergySphereBehavior>();
                for (int i = 0; i < energySpheresSpawned.Length; i++)
                {
                    GameObject node = LevelManager.nodes[LevelManager.targetNodes[LevelManager.levelObjectiveCurrent]];
                    energySpheresSpawned[i].GetComponentInChildren<EnergySphereCollision>().GoodCollision4Reals(node);
                }
            }
        }
    }
    public void ResetSphereArrays()
    {
        NumberDisplayEnergyChange.amountOnTargetMemory = amountOnTarget;
        //print("reset spheres" +Time.time);
        collidedSpheresOnTarget = new GameObject[0];
        collidedSpheresOnTraps = new GameObject[0];
        amountOnTarget = 0;
        amountOnTraps = 0;
        energySpheresSpawned = new GameObject[LevelManager.spawnZones.Length];
        amountSpawned = LevelManager.spawnZones.Length;
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
    public void AddGoToArrayOfSpawns(GameObject go, int index)
    {
        energySpheresSpawned[index] = go;
    }
    public void NewCollisionOnTarget(GameObject collidedSphere)
    {
        amountOnTarget++;
        ExpandArrayWithOneNewGoAddition(collidedSphere, amountOnTarget, ref collidedSpheresOnTarget);
        
        if (collidedSpheresOnTarget.Length == energySpheresSpawned.Length)
        {
            nodeBehavior.HighlightCompletedTarget(LevelManager.targetNodes[LevelManager.levelObjectiveCurrent]);
            LevelManager.nodes[LevelManager.targetNodes[LevelManager.levelObjectiveCurrent]].GetComponentInChildren<NumberDisplayTargetNode>().AllOrbsHaveHitTarget();
            if (LevelManager.levelMusic != null)
                LevelManager.levelMusic.AllOrbsHaveHitTarget();

            gameManager.levelManager.CheckIfBackgroundShouldChange(false);
        }
        else
        {
            LevelManager.nodes[LevelManager.targetNodes[LevelManager.levelObjectiveCurrent]].GetComponentInChildren<NumberDisplayTargetNode>().OrbHitsTarget();
            tutorialUI.ShowTextCorrectHitButStillMoreOrbsToGo();
        }

        GameManager.energyPool++;
        healthBar.UpdateHealthbarOnCollision(true);
    }
    public void NewCollisionOnTrap(GameObject collidedSphere)
    {
        amountOnTraps++;
        ExpandArrayWithOneNewGoAddition(collidedSphere, amountOnTraps, ref collidedSpheresOnTraps);
        gameManager.UpdateEnergyHealth(-1, false);
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
}