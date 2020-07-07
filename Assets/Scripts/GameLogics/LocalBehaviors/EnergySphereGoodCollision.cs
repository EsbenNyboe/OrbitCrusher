using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySphereGoodCollision : MonoBehaviour
{
    EnergySphereCollision energySphereCollision;
    CometBehavior cometBehavior;
    SphereCollider sphereCollider;
    EnergySphereBehavior energySphereBehavior;

    private void Awake()
    {
        energySphereCollision = transform.parent.GetComponentInChildren<EnergySphereCollision>();
        cometBehavior = energySphereCollision.cometBehavior;
        sphereCollider = GetComponent<SphereCollider>();
        energySphereBehavior = GetComponentInParent<EnergySphereBehavior>();
    }
    private void Update()
    {
        if (!EnergySphereBehavior.playerIsDraggingAnEnergySphere)
        {
            sphereCollider.enabled = true;
        }
        else
        {
            sphereCollider.enabled = false;
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (energySphereBehavior.hasBeenMoved && collision.gameObject.CompareTag("Node"))
        {
            if (!energySphereCollision.nodeHit)
            {
                GameObject node = collision.transform.parent.gameObject;
                if (node == cometBehavior.nodes[LevelManager.targetNodes[LevelManager.levelObjectiveCurrent]])
                {
                   
                    if (!EnergySphereBehavior.playerIsDraggingAnEnergySphere)
                    {
                        energySphereCollision.GoodCollision4Reals(node);
                    }
                }
            }
        }
    }
}
