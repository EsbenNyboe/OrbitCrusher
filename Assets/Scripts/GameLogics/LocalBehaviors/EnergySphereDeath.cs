using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySphereDeath : MonoBehaviour
{
    public GameObject particleExplode;
    public GameObject particleParent;
    [Range(0.001f, 5f)]
    public float collisionDeathDelay;
    IEnumerator scheduledDestroy;
    EnergySphereBehavior energySphereBehavior; 
    private void Awake()
    {
        energySphereBehavior = GetComponent<EnergySphereBehavior>();
    }
    public void StopRemainingParticlesThenDestroy()
    {
        energySphereBehavior.isDead = true;
        // necessary to check which has hit a node?
        StartCoroutine(ScheduledStop(0.2f));
        scheduledDestroy = ScheduledDestroy(collisionDeathDelay);
        StartCoroutine(scheduledDestroy);
    }
    IEnumerator ScheduledStop(float t)
    {
        energySphereBehavior.SpherePickedUpNoMore();
        yield return new WaitForSeconds(t);
        energySphereBehavior.isDead = true;
        energySphereBehavior.StopParticles();
    }
    IEnumerator ScheduledDestroy(float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
    }
    public void SphereCollision(bool target)
    {
        if (target)
        {
            StartCoroutine(ScheduledStop(0.2f));
        }
        else
        {
            GameObject newParticle = Instantiate(particleExplode, transform.position, transform.rotation, particleParent.transform);
            newParticle.SetActive(true);
            StartCoroutine(ScheduledStop(0.2f));
        }
    }
}
