
using UnityEngine;

public class EnergySphereBehavior : MonoBehaviour
{
    [HideInInspector]
    public Color cGhost, cAlive, cPickup, cGlued;


    public GameObject particleTrailPrefabA;
    public GameObject particleTrailLight;
    public GameObject particleTrailPrefabB;
    public GameObject particleCollisionBad;
    public GameObject particleCollisionGood;
    public GameObject particleSpawnA;
    public GameObject particleSpawnB;

    ParticleSystem.MainModule psLight;
    ParticleSystem.MainModule psmainA;
    ParticleSystem.MainModule psmainB;

    public GameObject particleParent;
    public bool snapToMousePos;
    public bool onlySnapIfMoving;
    [Range(0.9f, 0.999f)]
    public float snapSoftness;
    public GameObject nodeCollider;
    [Range(0.001f, 0.5f)]
    public float nodeSnapSpeed;
    public float glueSpeed;


    private Vector3 mOffset;
    private float mZCoord;
    Vector3 mPos;
    Vector3 mPosLast = new Vector3(0, 0, 0);
    private float particleDelay;

    [HideInInspector]
    public bool hasHitNode;
    [HideInInspector]
    public Vector3 collObjectPos;
    [HideInInspector]
    public bool isBeingDragged;
    public static bool playerIsDraggingAnEnergySphere;
    public static EnergySphereBehavior[] gluedObjects;
    public static EnergySphereBehavior draggedObject;

    bool isGlued;
    GameObject clickedObjectExternal;
    int initialSortingOrderA;
    int initialSortingOrderB;
    [HideInInspector]
    public bool isGhost;
    [HideInInspector]
    public bool isDead;
    bool movedToBack;


    float currentSpeed;

    private bool ghostEagerToBeGlued;
    private GameObject sphereBeingDraggedForEagerGhosts;
    private GameObject sphereBeingDragged;

    public SoundManager soundManager;
    public TutorialUI tutorialUI;


    public bool killParticleOnGoodColl;

    void Start()
    {
        // this used to be called in awake
        hasHitNode = false;
        initialSortingOrderA = particleTrailPrefabA.GetComponent<ParticleSystemRenderer>().sortingOrder;
        initialSortingOrderB = particleTrailPrefabB.GetComponent<ParticleSystemRenderer>().sortingOrder;
        psmainA = particleTrailPrefabA.GetComponent<ParticleSystem>().main;
        psmainB = particleTrailPrefabB.GetComponent<ParticleSystem>().main;
        psLight = particleTrailLight.GetComponent<ParticleSystem>().main;
        BecomeGhost();
    }
    void FixedUpdate()
    {
        if (isDead)
        {
            if (!movedToBack)
            {
                GetComponent<SphereCollider>().enabled = false;
                transform.localPosition += new Vector3(0, 0, 0); // this is stupid! disable collider instead
                movedToBack = true;
            }
            if (isBeingDragged)
            {
                SpherePickedUpNoMore();
                isBeingDragged = false;
                playerIsDraggingAnEnergySphere = false;
            }
        }
        if (!isDead)
        {
            if (hasHitNode)
            {
                isGlued = false;
                if (isBeingDragged)
                {
                    isBeingDragged = false;
                    playerIsDraggingAnEnergySphere = false;
                }
                mPos = transform.position; // this is dumb
                transform.position = Vector3.Lerp(mPos, collObjectPos, nodeSnapSpeed);
            }
            if (isGlued && playerIsDraggingAnEnergySphere)
            {
                transform.position = Vector3.Lerp(transform.position, clickedObjectExternal.transform.position, glueSpeed);
            }
            else if (isGlued && !playerIsDraggingAnEnergySphere) //
            {
                if (!hasHitNode)
                {
                    SetColorAlive(psmainA);
                    SetColorAlive(psmainB);
                }
                transform.position = sphereBeingDragged.transform.position;
                isGlued = false;
            }
            if (ghostEagerToBeGlued)
            {
                if (isGhost && !playerIsDraggingAnEnergySphere)
                {
                    ghostEagerToBeGlued = false;
                    sphereBeingDraggedForEagerGhosts = null;
                }
                else if (!isGhost && playerIsDraggingAnEnergySphere)
                {
                    ghostEagerToBeGlued = false;
                    if (sphereBeingDraggedForEagerGhosts != null)
                    {
                        GlueUnclickedObjectToClickedObject(sphereBeingDraggedForEagerGhosts);
                    }
                }
            }
            SetParticleDelay();
            SnapPositionOffsetToMousePosition();
            mPosLast = mPos;
        }
    }
    private void OnMouseDown()
    {
        if (isGhost)
        {
            soundManager.OrbPickupDenied();
        }
        if (Time.timeScale == 0)
        {
            tutorialUI.ClickOrbAfterFirstSpawn();
        }
        if (!hasHitNode && !isGhost && !isDead && Time.timeScale == 1)
        {
            soundManager.OrbPickup();
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            mOffset = gameObject.transform.position - GetMouseWorldPos();
            SetColorPickup(psmainA);
            SetColorPickup(psmainB);
            isBeingDragged = true;
            playerIsDraggingAnEnergySphere = true;
            particleTrailPrefabA.GetComponent<ParticleSystemRenderer>().sortingOrder = initialSortingOrderA;
            particleTrailPrefabB.GetComponent<ParticleSystemRenderer>().sortingOrder = initialSortingOrderB;
            particleTrailLight.GetComponent<ParticleSystem>().Play();
            draggedObject = this;
        }
    }
    private void OnMouseDrag()
    {
        if (!hasHitNode && !isGhost && isBeingDragged && !isDead)
        {
            AnimationCurvePrint curve = FindObjectOfType<AnimationCurvePrint>();
            Vector3 oldPosition = transform.position;
            Vector3 worldPos = GetMouseWorldPos();
            StayWithinScreenEdges(ref worldPos);
            transform.position = worldPos + mOffset;
            currentSpeed = (transform.position - oldPosition).sqrMagnitude;
            curve.valueSpeed = currentSpeed;
        }
    }
    
    private void OnMouseUp()
    {
        if (!hasHitNode && !isGhost && isBeingDragged && !isDead)
        {
            SpherePickedUpNoMore();
            SetColorAlive(psmainA);
            SetColorAlive(psmainB);
            isBeingDragged = false;
            playerIsDraggingAnEnergySphere = false;
        }
    }

    #region Ghost
    private void BecomeGhost()
    {
        isGhost = true;
        SetColorGhost(psmainA);
        SetColorGhost(psmainB);
    }
    public void BecomeAlive()
    {
        isGhost = false;
        SetColorAlive(psmainA);
        SetColorAlive(psmainB);
        particleSpawnA.GetComponent<ParticleSystem>().Play();
        particleSpawnB.GetComponent<ParticleSystem>().Play();
    }
    #endregion

    #region Position Methods
    private void StayWithinScreenEdges(ref Vector3 worldPos)
    {
        if (worldPos.x > OuterEdges.xMax)
        {
            worldPos.x = OuterEdges.xMax;
        }
        if (worldPos.x < OuterEdges.xMin)
        {
            worldPos.x = OuterEdges.xMin;
        }
        if (worldPos.y > OuterEdges.yMax)
        {
            worldPos.y = OuterEdges.yMax;
        }
        if (worldPos.y < OuterEdges.yMin)
        {
            worldPos.y = OuterEdges.yMin;
        }
    }

    public void SpherePickedUpNoMore()
    {
        sphereBeingDraggedForEagerGhosts = null;
        soundManager.OrbPickedUpNoMore();
    }
    
    private Vector3 GetMouseWorldPos()
    {
        mPos = Input.mousePosition;
        mPos.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mPos);
    }

    private bool ObjectHasMoved()
    {
        bool hasMoved;
        if (mPosLast != mPos)
            hasMoved = true;
        else
        {
            hasMoved = false;
        }
        return hasMoved;
    }
    private void SnapPositionOffsetToMousePosition()
    {
        if (snapToMousePos)
        {
            if (!onlySnapIfMoving || ObjectHasMoved())
                mOffset = mOffset * snapSoftness;
        }
    }
    #endregion

    #region Orb Visuals
    private void SetColorGhost(ParticleSystem.MainModule psmain)
    {
        // is ghost
        psmain.startColor = cGhost;
    }
    private void SetColorAlive(ParticleSystem.MainModule psmain)
    {
        // alive
        psmain.startColor = cAlive;
    }
    private void SetColorPickup(ParticleSystem.MainModule psmain)
    {
        // pickup
        psmain.startColor = cPickup;
    }
    private void SetColorGlued(ParticleSystem.MainModule psmain)
    {
        // is glued
        psmain.startColor = cGlued;
    }
    public void SetColorGoodCollision()
    {
        psmainA.startColor = cPickup;
        psmainB.startColor = cPickup;
    }


    public void ApplyColors(Color ghost, Color alive, Color pickup, Color glued)
    {
        cGhost = ghost;
        cAlive = alive;
        cPickup = pickup;
        cGlued = glued;
        //ParticleSystem.MainModule psmainBad = particleCollisionBad.GetComponent<ParticleSystem>().main;
        //psmainBad.startColor = collBad;
        //ParticleSystem.MainModule psmainGood = particleCollisionGood.GetComponent<ParticleSystem>().main;
        //psmainGood.startColor = collGood;
    }

    private void SetParticleDelay()
    {
        if (ObjectHasMoved())
            particleDelay = 0;
        else
        {
            particleDelay += 0.001f;
            if (particleDelay > 0.1f)
                particleDelay = 0.1f;
        }
    }
    private void AlterParticleParameters(ref GameObject particle, int order)
    {
        ParticleSystemRenderer psrend = particle.GetComponent<ParticleSystemRenderer>();
        psrend.sortingOrder = order;
    }
    //private void ResetGluedParticleModifications(ref GameObject particle, ref ParticleSystem.MainModule psmain)
    //{
    //    // i think this is supposed to reset the sorting order, when player releases the mouse click
    //}
    #endregion

    #region External Calls
    public void ClickedObjectLeftTheCollider(GameObject clickedObject)
    {
        ghostEagerToBeGlued = false;
        if (!isGhost)
        {
            GlueUnclickedObjectToClickedObject(clickedObject);
        }
    }
    public void GlueUnclickedObjectToClickedObject(GameObject clickedObject)
    {
        sphereBeingDragged = clickedObject;
        if (isGhost)
        {
            ghostEagerToBeGlued = true;
            sphereBeingDraggedForEagerGhosts = clickedObject;
        }
        if (!isDead && !isGlued && !isGhost)
        {
            soundManager.OrbGlued();
            isGlued = true;
            clickedObjectExternal = clickedObject;
            SetColorGlued(psmainA);
            SetColorGlued(psmainB);
            AlterParticleParameters(ref particleTrailPrefabA, 1);
            AlterParticleParameters(ref particleTrailPrefabB, 1);

            int arrayLength = 1;
            if (gluedObjects != null)
                arrayLength = gluedObjects.Length + 1;
            EnergySphereBehavior[] gluedObjectArray = new EnergySphereBehavior[arrayLength];
            for (int i = 0; i < arrayLength - 1; i++)
            {
                gluedObjectArray[i] = gluedObjects[i];
            }
            gluedObjects = gluedObjectArray;
            gluedObjects[arrayLength - 1] = this;
        }
    }
    public void ForceNodeCollisionOnGluedObject(Vector3 objectPos)
    {
        hasHitNode = true;
        collObjectPos = objectPos;
        nodeSnapSpeed = 1; // maybe reduce this speed, if it looks too weird
    }

    public void StopParticles()
    {
        particleTrailPrefabA.GetComponent<ParticleSystem>().Stop();
        particleTrailPrefabB.GetComponent<ParticleSystem>().Stop();
    }
    public void CollisionParticleEffectBad()
    {
        if (!isDead)
            particleCollisionBad.GetComponent<ParticleSystem>().Play();
    }
    public void CollisionParticleEffectGood()
    {
        particleCollisionGood.GetComponent<ParticleSystem>().Play();
    }
    public void KillTheLights()
    {
        //particleTrailAlight.GetComponent<ParticleSystem>().Stop();
    }
    #endregion
}