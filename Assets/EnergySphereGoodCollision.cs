using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySphereGoodCollision : MonoBehaviour
{
    EnergySphereCollision energySphereCollision;
    CometBehavior cometBehavior;

    private void Awake()
    {
        energySphereCollision = transform.parent.GetComponentInChildren<EnergySphereCollision>();
        cometBehavior = energySphereCollision.cometBehavior;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Node"))
        {
            if (!energySphereCollision.nodeHit)
            {
                GameObject node = collision.transform.parent.gameObject;
                if (node == cometBehavior.nodes[LevelManager.targetNodes[LevelManager.levelObjectiveCurrent]])
                {
                    energySphereCollision.GoodCollision4Reals(node);
                }
            }
        }
    }
}
