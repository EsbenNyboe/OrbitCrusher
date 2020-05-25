using System.Security.Cryptography;
using UnityEngine;

public class CometMovement : MonoBehaviour
{
    CometBehavior cometBehavior;

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

    void Awake()
    {
        cometBehavior = FindObjectOfType<CometBehavior>();
        freeBirdStartDirection = initialCometSpeed;
        freeBirdStartDirection.x *= RandomDirection() * Random.Range(0.6f, 1.2f);
        freeBirdStartDirection.y *= RandomDirection() * Random.Range(0.6f, 1.2f);
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

    public void LoadLevelTransition()
    {
        nodes = LevelManager.nodes;
        currentDestination = 0;
        transNodePos = LevelManager.transitionNode.transform.position;
        LevelManager.transitionNode.transform.parent = null;
        inTransition = true;
    }
    public void LoadLevel()
    {
        inTransition = false;
    }
    private void FixedUpdate()
    {
        if (GameManager.betweenLevels)
        {
            MoveTheCometFreely();
        }
    }

    private void MoveTheCometFreely()
    {
        Vector3 pos = transform.localPosition;
        transform.localPosition = Vector3.Lerp(pos, pos + freeBirdStartDirection, speedFreeBird);
        if (pos.x > OuterEdges.xMax)
            freeBirdStartDirection.x = -Mathf.Abs(freeBirdStartDirection.x);
        if (pos.x < OuterEdges.xMin)
            freeBirdStartDirection.x = Mathf.Abs(freeBirdStartDirection.x);
        if (pos.y > OuterEdges.yMax)
            freeBirdStartDirection.y = -Mathf.Abs(freeBirdStartDirection.y);
        if (pos.y < OuterEdges.yMin)
            freeBirdStartDirection.y = Mathf.Abs(freeBirdStartDirection.y);
    }

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

    public float transitionSpeedFactor;
    private void MoveCometToTheTransition()
    {
        
        Vector3 levelNodePos = nodes[0].transform.position;
        Vector3 transNodeProgress = Vector3.Lerp(transNodePos, levelNodePos, cometBehavior.timeProgressAdapted);
        LevelManager.transitionNode.transform.localPosition = transNodeProgress;
        float cometTransitionMovement = transitionSpeedThis * cometBehavior.timeProgressAdapted * cometBehavior.timeProgressAdapted;
        Vector3 pos = transform.localPosition;
        transform.localPosition = Vector3.Lerp(pos, transNodeProgress, cometTransitionMovement);

        freeBirdStartDirection = Vector3.Lerp(freeBirdStartDirection, new Vector3(0, 0, 0), cometTransitionMovement * transitionSpeedFactor);
    }

    private void MoveCometToTheBeat()
    {
        float cometProgress = cometBehavior.timeProgressAdapted;

        if (cueDirectionChange && cometProgress < 0.5f) // seems like an unstable solution this one - but works so far so good, ok
        {
            previousDestination = currentDestination;
            currentDestination = nextDestination;
            cueDirectionChange = false;
        }
        // set the node outline as the comets destination (to postpone the visual feedback of a collision)
        Vector3 prevNodePos = nodes[previousDestination].transform.position;
        Vector3 nextNodePos = nodes[currentDestination].transform.position;
        float nodeDistance = Vector3.Distance(prevNodePos, nextNodePos); // read
        float nodeDistanceMinusRadius = nodeDistance - nodes[currentDestination].transform.localScale.x;
        Vector3 nextNodePosMinusRadius = Vector3.Lerp(prevNodePos, nextNodePos, nodeDistanceMinusRadius / nodeDistance);

        Vector3 currentTimePosition = Vector3.Lerp(prevNodePos, nextNodePos, cometProgress);
        transform.localPosition = Vector3.Lerp(transform.position, currentTimePosition, extraLerpin);
        MusicMeter m = FindObjectOfType<MusicMeter>();

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


        //AnimationCurvePrint.value = cometBehavior.timeProgressAdapted;
        //print(cometBehavior.timeProgressAdapted + "t:" + Time.time);
    }
    public void ChangeDirection(int nextDest)
    {
        cueDirectionChange = true;
        nextDestination = nextDest;
    }
}
