﻿using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class EnergySphereCollision : MonoBehaviour
{
    EnergySphereBehavior energySphereBehavior;

    public ObjectiveManager objectiveManager;
    public CometMovement cometMovement;
    public NodeBehavior nodeManager;
    public SoundManager soundManager;
    public TutorialUI tutorialUI;
    bool nodeHit;

    private void Awake()
    {
        energySphereBehavior = GetComponentInParent<EnergySphereBehavior>();
        //objectiveManager = FindObjectOfType<ObjectiveManager>();
        //cometMovement = FindObjectOfType<CometMovement>();
        //nodeManager = FindObjectOfType<NodeBehavior>();
        //soundManager = FindObjectOfType<SoundManager>();
        //tutorialUI = FindObjectOfType<TutorialUI>();
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
                if (GameManager.inTutorial)
                {
                    tutorialUI.ShowTextFirstCometHit();
                }
                soundManager.CometHitsOrb();
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
                    soundManager.CorrectNodeHit();
                    GoodCollision();
                    if (GameManager.inTutorial)
                    {
                        tutorialUI.ShowTextFirstCorrectHit();
                    }
                }
                else
                {
                    soundManager.IncorrectNodeHit();
                    BadCollision();
                    if (GameManager.inTutorial)
                    {
                        tutorialUI.ShowTextFirstRedNodeHit();
                    }
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
        energySphereBehavior.CollisionParticleEffectGood();
        GetComponentInParent<EnergySphereDeath>().SphereCollision(true);
        objectiveManager.NewCollisionOnTarget(transform.parent.gameObject);
        DisableCollider();
    }

    private void BadCollision()
    {
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