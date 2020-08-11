using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySphereGoodCollision : MonoBehaviour
{
    public EnergySphereCollision energySphereCollision;
    CometBehavior cometBehavior;
    public SphereCollider sphereCollider;
    public EnergySphereBehavior energySphereBehavior;

    private void Awake()
    {
        cometBehavior = energySphereCollision.cometBehavior;
        //energySphereCollision = transform.parent.GetComponentInChildren<EnergySphereCollision>();
        //sphereCollider = GetComponent<SphereCollider>();
        //energySphereBehavior = GetComponentInParent<EnergySphereBehavior>();
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
