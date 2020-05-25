using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    GameObject spawn;
    GameObject newSpawn;
    float xMin;
    float xMax;
    float yMin;
    float yMax;
    public static int counter;
    ObjectiveManager energySphereManager;

    private void Awake()
    {
        energySphereManager = FindObjectOfType<ObjectiveManager>();
        transform.parent = energySphereManager.gameObject.transform;
    }
    public void LoadSpawnZone()
    {
        spawn = energySphereManager.energySpherePrefab;
        Vector3 zonePos = transform.position;
        Vector3 zoneScale = transform.localScale;
        Vector3 spawnScale = spawn.transform.localScale;
        xMin = zonePos.x - zoneScale.x * 0.5f + spawnScale.x * 0.5f;
        xMax = zonePos.x + zoneScale.x * 0.5f - spawnScale.x * 0.5f;
        yMin = zonePos.y - zoneScale.y * 0.5f + spawnScale.y * 0.5f;
        yMax = zonePos.y + zoneScale.y * 0.5f - spawnScale.y * 0.5f;
        if (Mathf.Abs(xMax - xMin) < spawnScale.x)
            xMin = xMax = zonePos.x;
        if (Mathf.Abs(yMax - yMin) < spawnScale.y)
            yMin = yMax = zonePos.y;
    }
    public GameObject SpawnEnergySphere()
    {
//        Destroy(newSpawn);
        Vector3 spawnPos = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);
        newSpawn = Instantiate(spawn, spawnPos, transform.rotation);
        newSpawn.name = "EnergySphere " + counter;
        newSpawn.SetActive(true);
        counter++;
        return newSpawn;
    }
}
