using System.Collections;
using System.Data;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class CometBehavior : MonoBehaviour
{

    [HideInInspector]
    public GameObject[] nodes;
    int currentDestination;
    int nextDestination;
    int previousDestination;
    public float extraLerpin;
    public static bool isMoving;
    bool cueDirectionChange;
    public float speedFreeBird;
    bool inTransition;
    public float transitionSpeedThis;
    Vector3 transNodePos;
    public Vector3 initialCometSpeed;
    Vector3 freeBirdStartDirection;
    public float levelConclusionCometSpeed;

    public float transitionSpeedFactor;

    public SoundManager soundManager;
    public MeshRenderer cometOutline;
    public CometManager cometManager;

    bool introductionSequence = true;
    public float introductionSequenceLength; // how long the comet is offscreen
    public Vector3 initialCometSpeedNew;

    public Vector3 cometSpawnPosition;

    public AnimationCurve lerpToTransitionNode;

    public Light light;

    public float intensityHigh;
    public float intensityLow;

    private void Start()
    {
        light.enabled = false;
        light.intensity = intensityLow;

        transform.position = cometSpawnPosition;
        freeBirdStartDirection = initialCometSpeedNew;
        //RandomStartDirection();
        cometOutline.enabled = true;
        StartCoroutine(IntroductionLength());
    }

    private void RandomStartDirection()
    {
        freeBirdStartDirection = initialCometSpeed;
        freeBirdStartDirection.x *= RandomDirection() * Random.Range(0.6f, 1.2f);
        freeBirdStartDirection.y *= RandomDirection() * Random.Range(0.6f, 1.2f);
    }

    IEnumerator IntroductionLength()
    {
        yield return new WaitForSeconds(introductionSequenceLength);
        introductionSequence = false;
    }

    private float RandomDirection()
    {
        if (Mathf.RoundToInt(Random.value) > 0.5f)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }    
    private void FixedUpdate()
    {
        if (GameManager.betweenLevels)
        {
            MoveTheCometFreely();
        }
    }
    #region External Calls

    public void FakeUpdate()
    {
        if (GameManager.betweenLevels && inTransition)
        {
            MoveCometToTheTransition();
        }
        else if (isMoving)
        {
            MoveCometToTheBeat();
        }
    }
    public void ChangeDirectionToNextDestination(int nextDest)
    {
        cueDirectionChange = true;
        nextDestination = nextDest;
    }
    public void LoadLevelTransition()
    {
        nodes = LevelManager.nodes;
        currentDestination = 0;
        //LevelManager.transitionNode.transform.parent = null;
        LevelManager.transitionNodeAnim.Play(0);
        transNodeStartPos = transNodePos = LevelManager.transitionNode.transform.position;
        inTransition = true;
    }
    public void LoadLevel()
    {
        inTransition = false;
    }

    Vector3 transNodeStartPos; // maybe this is doing nothing actually...
    public void LevelFailed()
    {
        //LevelManager.transitionNode.transform.localPosition = transNodeStartPos;
    }
    #endregion


    public CometWallHits cometWallHits;
    #region Move Comet
    private void MoveTheCometFreely()
    {
        Vector3 pos = transform.localPosition;
        transform.localPosition = Vector3.Lerp(pos, pos + freeBirdStartDirection, speedFreeBird);
        if (!introductionSequence)
        {
            if (pos.x > OuterEdges.xMax)
            {
                cometWallHits.EdgeHitRight();
                soundManager.CometWallHit();
                freeBirdStartDirection.x = -Mathf.Abs(freeBirdStartDirection.x);
            }
            if (pos.x < OuterEdges.xMin)
            {
                cometWallHits.EdgeHitLeft();
                soundManager.CometWallHit();
                freeBirdStartDirection.x = Mathf.Abs(freeBirdStartDirection.x);
            }
            if (pos.y > OuterEdges.yMax)
            {
                cometWallHits.EdgeHitTop();
                soundManager.CometWallHit();
                freeBirdStartDirection.y = -Mathf.Abs(freeBirdStartDirection.y);
            }
            if (pos.y < OuterEdges.yMin)
            {
                cometWallHits.EdgeHitBottom();
                soundManager.CometWallHit();
                freeBirdStartDirection.y = Mathf.Abs(freeBirdStartDirection.y);
            }
        }
    }
    private void MoveCometToTheTransition()
    {
        Vector3 levelNodePos = nodes[0].transform.position;
        
        Vector3 transNodeProgress = Vector3.Lerp(transNodePos, levelNodePos, cometManager.timeProgressAdapted); // replace

        //print("timeProgress:" + cometManager.timeProgressAdapted + "t:" + Time.time);
        //print("pos:" + transNodeProgress + "t:" + Time.time);

        //LevelManager.transitionNode.transform.localPosition = transNodeProgress; // replace

        float cometTransitionMovement = transitionSpeedThis * cometManager.timeProgressAdapted * cometManager.timeProgressAdapted; // replace
        Vector3 pos = transform.localPosition;
        //transform.localPosition = Vector3.Lerp(pos, transNodeProgress, cometTransitionMovement); // replace

        //transform.localPosition = Vector3.Lerp(pos, transNodeProgress, lerpToTransitionNode.Evaluate(cometManager.timeProgressAdapted));

        transform.localPosition = Vector3.Lerp(pos, LevelManager.transitionNode.transform.position, lerpToTransitionNode.Evaluate(cometManager.timeProgressAdapted));

        freeBirdStartDirection = Vector3.Lerp(freeBirdStartDirection, new Vector3(0, 0, 0), cometTransitionMovement * transitionSpeedFactor);
    }
    //public static float Distance(Vector3 a, Vector3 b)
    //{
    //    Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
    //    return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    //}

    private void MoveCometToTheBeat()
    {
        float cometProgress = cometManager.timeProgressAdapted;

        if (cueDirectionChange && cometProgress < 0.5f) // seems like an unstable solution this one - but works so far so good, ok
        {
            previousDestination = currentDestination;
            currentDestination = nextDestination;
            cueDirectionChange = false;
        }
        // set the node outline as the comets destination (to postpone the visual feedback of a collision)
        Vector3 prevNodePos = nodes[previousDestination].transform.position;
        Vector3 nextNodePos = nodes[currentDestination].transform.position;
        //float nodeDistance = Vector3.Distance(prevNodePos, nextNodePos); // read
        //float nodeDistanceMinusRadius = nodeDistance - nodes[currentDestination].transform.localScale.x;
        //Vector3 nextNodePosMinusRadius = Vector3.Lerp(prevNodePos, nextNodePos, nodeDistanceMinusRadius / nodeDistance);

        Vector3 currentTimePosition = Vector3.Lerp(prevNodePos, nextNodePos, cometProgress);
        transform.localPosition = Vector3.Lerp(transform.position, currentTimePosition, extraLerpin);


        SetFreeBirdDirectionToBeUsedInCaseOfLevelConclusion(prevNodePos, nextNodePos);
    }

    private void SetFreeBirdDirectionToBeUsedInCaseOfLevelConclusion(Vector3 prevNodePos, Vector3 nextNodePos)
    {
        freeBirdStartDirection = nextNodePos - prevNodePos;
        float freeBirdX = Mathf.Abs(freeBirdStartDirection.x);
        float freeBirdY = Mathf.Abs(freeBirdStartDirection.y);
        float xyRelation = 0;
        if (freeBirdX > freeBirdY)
        {
            xyRelation = freeBirdY / freeBirdX;
            freeBirdX = levelConclusionCometSpeed;
            freeBirdY = freeBirdX * xyRelation;
        }
        else
        {
            xyRelation = freeBirdX / freeBirdY;
            freeBirdY = levelConclusionCometSpeed;
            freeBirdX = freeBirdY * xyRelation;
        }
        if (freeBirdStartDirection.x < 0)
            freeBirdStartDirection.x = -freeBirdX;
        else
            freeBirdStartDirection.x = freeBirdX;
        if (freeBirdStartDirection.y < 0)
            freeBirdStartDirection.y = -freeBirdY;
        else
            freeBirdStartDirection.y = freeBirdY;
    }
    #endregion
}
