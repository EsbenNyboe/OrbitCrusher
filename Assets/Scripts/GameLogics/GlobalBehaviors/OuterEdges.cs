using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterEdges : MonoBehaviour
{
    // note to self: also apply to spawn-limiting 
    public static float xMax;
    public static float xMin;
    public static float yMax;
    public static float yMin;

    public MeshRenderer meshRenderer;

    public bool autoDisableRenderer;

    private void Awake()
    {
        Vector3 pos = transform.position;
        Vector3 scale = transform.localScale;
        xMax = pos.x + scale.x * 0.5f;
        xMin = pos.x - scale.x * 0.5f;
        yMax = pos.y + scale.y * 0.5f;
        yMin = pos.y - scale.y * 0.5f;
        if (autoDisableRenderer)
            //GetComponent<MeshRenderer>().enabled = false;
            meshRenderer.enabled = false;
    }
}
