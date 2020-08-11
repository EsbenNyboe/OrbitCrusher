using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCollisionScript : MonoBehaviour
{
    public Collider goodNodeCollider;
    public Collider badNodeCollider;

    public void ChooseCollider(bool goodOrBad)
    {
        goodNodeCollider.enabled = goodOrBad;
        badNodeCollider.enabled = !goodOrBad;
    }
}
