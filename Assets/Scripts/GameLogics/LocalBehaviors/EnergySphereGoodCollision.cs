using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySphereGoodCollision : MonoBehaviour
{
    EnergySphereCollision energySphereCollision;
    CometBehavior cometBehavior;
    SphereCollider sphereCollider;

    private void Awake()
    {
        energySphereCollision = transform.parent.GetComponentInChildren<EnergySphereCollision>();
        cometBehavior = energySphereCollision.cometBehavior;
        sphereCollider = GetComponent<SphereCollider>();
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
        if (collision.gameObject.CompareTag("Node"))
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
