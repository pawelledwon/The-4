using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPhysicsWithJoint : MonoBehaviour
{
    Rigidbody rigidbody;
    ConfigurableJoint joint;

    [SerializeField]
    Rigidbody animatedRigidBody;

    [SerializeField]
    bool syncAnimation = false;

    Quaternion startRotation;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();

        startRotation = transform.localRotation;
    }

    public void UpdateJointFromAnimation()
    {
        if(!syncAnimation)
        {
            return;
        }

        ConfigurableJointExtensions.SetTargetRotationLocal(joint, animatedRigidBody.transform.localRotation, startRotation);
    }
}
