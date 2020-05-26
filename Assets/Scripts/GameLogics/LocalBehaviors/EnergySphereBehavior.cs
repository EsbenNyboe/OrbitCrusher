using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EnergySphereBehavior : MonoBehaviour
{
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

    bool isGlued;
    GameObject clickedObjectExternal;
    int initialSortingOrderA;
    int initialSortingOrderB;
    [HideInInspector]
    public bool isGhost;
    [HideInInspector]
    public bool isDead;

    SoundManager soundManager;

    private void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        hasHitNode = false;
        initialSortingOrderA = particleTrailPrefabA.GetComponent<ParticleSystemRenderer>().sortingOrder;
        initialSortingOrderB = particleTrailPrefabB.GetComponent<ParticleSystemRenderer>().sortingOrder;
        psmainA = particleTrailPrefabA.GetComponent<ParticleSystem>().main;
        psmainB = particleTrailPrefabB.GetComponent<ParticleSystem>().main;
        psLight = particleTrailLight.GetComponent<ParticleSystem>().main;
        BecomeGhost();
        //SpawnParticle(particleTrailPrefabA, ref particleTrailA, ref psmainA);
        //SetTrailColorA(psmainA);
        //SpawnParticle(particleTrailPrefabB, ref particleTrailB, ref psmainB);
        //SetTrailColorA(psmainB);
    }
    void Update()
    {
        if (isDead && !movedToBack)
        {
            GetComponent<SphereCollider>().enabled = false;
            transform.localPosition += new Vector3(0, 0, 0); // this is stupid! disable collider instead
            movedToBack = true;
        }
        if (isDead && isBeingDragged)
        {
            soundManager.SpherePickedUpNoMore();
            isBeingDragged = false;
            playerIsDraggingAnEnergySphere = false;
            //psLight.loop = false;
        }
        if (!isDead)
        {
            if (hasHitNode)
            {
                isGlued = false;
                if (isBeingDragged)
                {
                    isBeingDragged = false;
                    //psLight.loop = false;
                    playerIsDraggingAnEnergySphere = false;
                }
                mPos = transform.position; // this is dumb
                transform.position = Vector3.Lerp(mPos, collObjectPos, nodeSnapSpeed);
            }
            if (isGlued && playerIsDraggingAnEnergySphere)
            {
                transform.position = Vector3.Lerp(transform.position, clickedObjectExternal.transform.position, glueSpeed);
            }
            else if (isGlued && !playerIsDraggingAnEnergySphere)
            {
                SetTrailColorA(psmainA);
                SetTrailColorA(psmainB);
                isGlued = false;
            }
            SetParticleDelay();
            SnapPositionOffsetToMousePosition();
            mPosLast = mPos;
        }
    }
    public void BecomeGhost()
    {
        isGhost = true;
        SetTrailColorD(psmainA);
        SetTrailColorD(psmainB);
    }
    public void BecomeAlive()
    {
        isGhost = false;
        SetTrailColorA(psmainA);
        SetTrailColorA(psmainB);
        particleSpawnA.GetComponent<ParticleSystem>().Play();
        particleSpawnB.GetComponent<ParticleSystem>().Play();
    }

    void Start()
    {
        
    }
    bool movedToBack;
    
    private void OnMouseDown()
    {
        if (!hasHitNode && !isGhost && !isDead)
        {
            soundManager.SpherePickedUp();
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            mOffset = gameObject.transform.position - GetMouseWorldPos();
            SetTrailColorB(psmainA);
            SetTrailColorB(psmainB);
            isBeingDragged = true;
            playerIsDraggingAnEnergySphere = true;
            particleTrailPrefabA.GetComponent<ParticleSystemRenderer>().sortingOrder = initialSortingOrderA;
            particleTrailPrefabB.GetComponent<ParticleSystemRenderer>().sortingOrder = initialSortingOrderB;
            //psLight.loop = true;
            particleTrailLight.GetComponent<ParticleSystem>().Play();
        }
    }

    float currentSpeed;
    private void OnMouseDrag()
    {
        if (!hasHitNode && !isGhost && isBeingDragged && !isDead)
        {
            AnimationCurvePrint curve = FindObjectOfType<AnimationCurvePrint>();
            Vector3 oldPosition = transform.position;
            Vector3 worldPos = GetMouseWorldPos();
            IsWithinScreenEdges(ref worldPos);
            transform.position = worldPos + mOffset;
            currentSpeed = (transform.position - oldPosition).sqrMagnitude;
            curve.valueSpeed = currentSpeed;
        }
    }

    private void IsWithinScreenEdges(ref Vector3 worldPos)
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

    private void OnMouseUp()
    {
        if (!hasHitNode && !isGhost && isBeingDragged && !isDead)
        {
            soundManager.SpherePickedUpNoMore();
            SetTrailColorA(psmainA);
            SetTrailColorA(psmainB);
            isBeingDragged = false;
            playerIsDraggingAnEnergySphere = false;
            //psLight.loop = false;
        }
    }
    private void SpawnParticle(GameObject prefab, ref GameObject go, ref ParticleSystem.MainModule psmain)
    {
        go = Instantiate(prefab, transform);
        go.transform.localPosition = new Vector3(0, 0, 0);
        psmain = go.GetComponent<ParticleSystem>().main;
        go.SetActive(true);
    }
    public void SetTrailColorA(ParticleSystem.MainModule psmain)
    {
        psmain.startColor = Color.magenta;
    }
    public void SetTrailColorB(ParticleSystem.MainModule psmain)
    {
        psmain.startColor = Color.cyan;
    }
    public void SetTrailColorC(ParticleSystem.MainModule psmain)
    {
        psmain.startColor = Color.magenta;
    }
    public void SetTrailColorD(ParticleSystem.MainModule psmain)
    {
        psmain.startColor = Color.gray;
    }
    private Vector3 GetMouseWorldPos()
    {
        mPos = Input.mousePosition;
        mPos.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mPos);
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
    public void GlueUnclickedObjectToClickedObject(GameObject clickedObject)
    {
        if (!isDead && !isGlued)
        {
            soundManager.SphereGlued();
            isGlued = true;
            clickedObjectExternal = clickedObject;
            SetTrailColorC(psmainA);
            SetTrailColorC(psmainB);
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

    public void AlterParticleParameters(ref GameObject particle, int order)
    {
        ParticleSystemRenderer psrend = particle.GetComponent<ParticleSystemRenderer>();
        psrend.sortingOrder = order;
    }
    public void ResetGluedParticleModifications(ref GameObject particle, ref ParticleSystem.MainModule psmain)
    {
        // i think this is supposed too reset the sorting order, when player releases the mouse click
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
}