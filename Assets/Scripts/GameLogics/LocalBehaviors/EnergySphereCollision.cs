using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class EnergySphereCollision : MonoBehaviour
{
    EnergySphereBehavior energySphereBehavior;

    public ObjectiveManager objectiveManager;
    public CometBehavior cometBehavior;
    public NodeBehavior nodeManager;
    public SoundManager soundManager;
    public TutorialUI tutorialUI;
    public ScreenShake screenShake;
    [HideInInspector]
    public bool nodeHit;

    private void Start()
    {
        // this used to be in awake
        energySphereBehavior = GetComponentInParent<EnergySphereBehavior>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Comet"))
        {
            if (!nodeHit && !energySphereBehavior.isGhost)
            {
                GameObject comet = collision.gameObject;
                MakeSpherePosSnapToCollObjectPos(comet);
                ForceCollisionOnGluedObjectIfThisObjectIsBeingDragged(comet);
                if (GameManager.inTutorial)
                {
                    tutorialUI.ShowTextFirstCometHit();
                }
                soundManager.CometHitsOrb();
                BadCollision();
                screenShake.ScreenShakeCollBadComet();
            }
        }
        if (collision.gameObject.CompareTag("Node"))
        {
            if (!nodeHit)
            {
                GameObject node = collision.transform.parent.gameObject;

                if (node != cometBehavior.nodes[LevelManager.targetNodes[LevelManager.levelObjectiveCurrent]])
                {
                    MakeSpherePosSnapToCollObjectPos(node);
                    ForceCollisionOnGluedObjectIfThisObjectIsBeingDragged(node);
                    soundManager.IncorrectNodeHit();
                    nodeManager.CollisionNodeEnergySphereColor(node, false);
                    BadCollision();
                    screenShake.ScreenShakeCollBadNode();
                    if (GameManager.inTutorial)
                    {
                        tutorialUI.ShowTextFirstRedNodeHit();
                    }
                }
                else if (EnergySphereBehavior.playerIsDraggingAnEnergySphere)
                {
                    GoodCollision4Reals(node);
                }
            }
        }
        if (collision.gameObject.CompareTag("EnergySphere") && collision.gameObject.name != transform.parent.gameObject.name)
        {
            if (energySphereBehavior.isBeingDragged)
                collision.gameObject.GetComponentInParent<EnergySphereBehavior>().GlueUnclickedObjectToClickedObject(transform.parent.gameObject);
        }
    }

    public void GoodCollision4Reals(GameObject node)
    {
        energySphereBehavior.SetColorGoodCollision();
        MakeSpherePosSnapToCollObjectPos(node);
        ForceCollisionOnGluedObjectsAndDraggedObject(node);
        //ForceCollisionOnGluedObjectIfThisObjectIsBeingDragged(node);
        //if (!energySphereBehavior.isBeingDragged) // a bit messy
        //{
        //    energySphereBehavior.ForceNodeCollisionOnGluedObject(node.transform.position);
        //}
        soundManager.CorrectNodeHit();
        nodeManager.CollisionNodeEnergySphereColor(node, true);
        GoodCollision();
        if (GameManager.inTutorial)
        {
            tutorialUI.ShowTextFirstCorrectHit();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnergySphere") && collision.gameObject.name != transform.parent.gameObject.name)
        {
            if (energySphereBehavior.isBeingDragged)
                collision.gameObject.GetComponentInParent<EnergySphereBehavior>().ClickedObjectLeftTheCollider(transform.parent.gameObject);
        }
    }

    private void MakeSpherePosSnapToCollObjectPos(GameObject node)
    {
        nodeHit = true;
        energySphereBehavior.collObjectPos = node.transform.position;
        energySphereBehavior.hasHitNode = true;
    }

    private void ForceCollisionOnGluedObjectsAndDraggedObject(GameObject node)
    {
        if (EnergySphereBehavior.gluedObjects != null)
        {
            foreach (var gluedObject in EnergySphereBehavior.gluedObjects)
            {
                gluedObject.ForceNodeCollisionOnGluedObject(node.transform.position);
            }
            EnergySphereBehavior.gluedObjects = new EnergySphereBehavior[0];
        }
        EnergySphereBehavior.draggedObject.ForceNodeCollisionOnGluedObject(node.transform.position); // not optimal, but it works
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
        screenShake.ScreenShakeCollGoodNode();
    }

    private void BadCollision()
    {
        energySphereBehavior.CollisionParticleEffectBad();
        GetComponentInParent<EnergySphereDeath>().SphereCollision(false);
        objectiveManager.NewCollisionOnTrap(transform.parent.gameObject);
        DisableCollider();
    }

    public void DisableCollider()
    {
        GetComponent<SphereCollider>().enabled = false;
    }
}