using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class EnergySphereCollision : MonoBehaviour
{
    EnergySphereBehavior energySphereBehavior;
    ObjectiveManager objectiveManager;
    CometMovement cometMovement;
    NodeBehavior nodeManager;
    SoundManager soundManager;
    bool nodeHit;

    private void Awake()
    {
        energySphereBehavior = GetComponentInParent<EnergySphereBehavior>();
        objectiveManager = FindObjectOfType<ObjectiveManager>();
        cometMovement = FindObjectOfType<CometMovement>();
        nodeManager = FindObjectOfType<NodeBehavior>();
        soundManager = FindObjectOfType<SoundManager>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Comet"))
        {
            if (!nodeHit && !energySphereBehavior.isGhost)
            {
                Debug.Log("<color=blue> cometHit </color>");
                GameObject comet = collision.gameObject;
                MakeSpherePosSnapToCollObjectPos(comet);
                ForceCollisionOnGluedObjectIfThisObjectIsBeingDragged(comet);
                BadCollision();
            }
        }
        if (collision.gameObject.CompareTag("Node"))
        {
            if (!nodeHit)
            {
                GameObject node = collision.gameObject;
                nodeManager.CollisionNodeEnergySphere(collision.gameObject);
                MakeSpherePosSnapToCollObjectPos(node);
                ForceCollisionOnGluedObjectIfThisObjectIsBeingDragged(node);
                if (node == cometMovement.nodes[LevelManager.targetNodes[LevelManager.levelObjectiveCurrent]])
                {
                    GoodCollision();
                }
                else
                {
                    BadCollision();
                }
            }
        }
        //if (collision.gameObject != transform.parent.gameObject && collision.gameObject.CompareTag("EnergySphere"))
        //{
        //    if (energySphereBehavior.isBeingDragged)
        //        collision.gameObject.GetComponent<EnergySphereBehavior>().GlueUnclickedObjectToClickedObject(transform.parent.gameObject);
        //}
        if (collision.gameObject.CompareTag("EnergySphere") && collision.gameObject.name != transform.parent.gameObject.name)
        {
            if (energySphereBehavior.isBeingDragged)
                collision.gameObject.GetComponent<EnergySphereBehavior>().GlueUnclickedObjectToClickedObject(transform.parent.gameObject);
        }
    }

    private void MakeSpherePosSnapToCollObjectPos(GameObject node)
    {
        nodeHit = true;
        energySphereBehavior.collObjectPos = node.transform.position;
        energySphereBehavior.hasHitNode = true;
    }

    private void ForceCollisionOnGluedObjectIfThisObjectIsBeingDragged(GameObject node)
    {
        if (energySphereBehavior.isBeingDragged)
        {
            if (EnergySphereBehavior.gluedObjects != null)
            {
                foreach (var gluedObject in EnergySphereBehavior.gluedObjects)
                {
                    gluedObject.ForceNodeCollisionOnGluedObject(node.transform.position);
                }
                EnergySphereBehavior.gluedObjects = new EnergySphereBehavior[0];
            }
        }
    }

    private void GoodCollision()
    {
        soundManager.CorrectHit();
        energySphereBehavior.CollisionParticleEffectGood();
        GetComponentInParent<EnergySphereDeath>().SphereCollision(true);
        objectiveManager.NewCollisionOnTarget(transform.parent.gameObject);
        DisableCollider();
    }

    private void BadCollision()
    {
        soundManager.IncorrectHit();
        energySphereBehavior.CollisionParticleEffectBad();
        GetComponentInParent<EnergySphereDeath>().SphereCollision(false);
        objectiveManager.NewCollisionOther(transform.parent.gameObject);
        DisableCollider();
    }

    public void DisableCollider()
    {
        GetComponent<SphereCollider>().enabled = false;
    }
}