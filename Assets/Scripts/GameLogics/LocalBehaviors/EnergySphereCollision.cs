using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class EnergySphereCollision : MonoBehaviour
{
    public EnergySphereBehavior energySphereBehavior;

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
        //energySphereBehavior = GetComponentInParent<EnergySphereBehavior>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!energySphereBehavior.isDead)
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
    }

    public void GoodCollision4Reals(GameObject node)
    {
        if (!energySphereBehavior.isDead)
        {
            energySphereBehavior.SetColorGoodCollision();
            MakeSpherePosSnapToCollObjectPos(node);
            ForceCollisionOnGluedObjectsAndDraggedObject(node);
            soundManager.CorrectNodeHit();
            nodeManager.CollisionNodeEnergySphereColor(node, true);
            GoodCollision();
            if (GameManager.inTutorial)
            {
                tutorialUI.ShowTextFirstCorrectHit();
                tutorialUI.ShowTextFinalPuzzle();
            }
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
                if (gluedObject.isGlued)
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

    [HideInInspector]
    public EnergySphereDeath energySphereDeath;
    private void GoodCollision()
    {
        energySphereBehavior.CollisionParticleEffectGood();
        energySphereDeath.SphereCollision(true);
        //GetComponentInParent<EnergySphereDeath>().SphereCollision(true);
        objectiveManager.NewCollisionOnTarget(transform.parent.gameObject);
        DisableCollider();
        screenShake.ScreenShakeCollGoodNode();
    }

    private void BadCollision()
    {
        energySphereBehavior.CollisionParticleEffectBad();
        energySphereDeath.SphereCollision(false);
        //GetComponentInParent<EnergySphereDeath>().SphereCollision(false);
        objectiveManager.NewCollisionOnTrap(transform.parent.gameObject);
        DisableCollider();
    }

    //[HideInInspector]
    public SphereCollider sphereCollider;
    public void DisableCollider()
    {
        sphereCollider.enabled = false;
        //GetComponent<SphereCollider>().enabled = false;
    }
}