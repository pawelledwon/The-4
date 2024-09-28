using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    [SerializeField]
    Collider thisCollider;

    [SerializeField]
    Collider[] collidersToIgnore;
    void Start()
    {
        foreach(var otherCollider in collidersToIgnore){
            Physics.IgnoreCollision(thisCollider, otherCollider, true);
        }
    }
}
