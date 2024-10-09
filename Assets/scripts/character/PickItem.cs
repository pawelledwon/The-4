using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    [SerializeField]
    GameObject leftHand;

    [SerializeField]
    GameObject rightHand;

    [SerializeField]
    Animator animator;

    [SerializeField]
    string layerName = "PickUp";

    [SerializeField]
    List<ConfigurableJoint> handsJoints;

    bool pickUpPossible; 
    GameObject objectToPickUp; 
    bool hasItem;

    void Start()
    {
        pickUpPossible = false;   
        hasItem = false;
    }

    void Update()
    {
        if (pickUpPossible == true) 
        {
            if (Input.GetKeyDown("e"))  
            {
                
                animatePickUp();

                hasItem = true;

                objectToPickUp.GetComponent<Rigidbody>().isKinematic = true;

                Vector3 pickeObjectPosition = calculateTargetPosition();

                objectToPickUp.transform.position = pickeObjectPosition;
                objectToPickUp.transform.parent = leftHand.transform;
                objectToPickUp.GetComponent<Collider>().enabled = false;
            }
        }

        if (Input.GetKeyDown("q") && hasItem == true) 
        {
            objectToPickUp.GetComponent<Rigidbody>().isKinematic = false; 
            objectToPickUp.transform.parent = null;
            objectToPickUp.GetComponent<Collider>().enabled = true;

            hasItem = false;

            disableAnimatePickUp();
        }
    }
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Object") 
        {
            pickUpPossible = true;  
            objectToPickUp = other.gameObject; 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        pickUpPossible = false;
    }

    private Vector3 calculateTargetPosition()
    {
        float a = leftHand.transform.position.x;
        float b = leftHand.transform.position.y;
        float c = leftHand.transform.position.z;
        float d = rightHand.transform.position.x;
        float e = rightHand.transform.position.y;
        float f = rightHand.transform.position.z;


        return new Vector3((a + d) / 2, (b + e) / 2, (c + f) / 2);
    }

    private void animatePickUp()
    {
        foreach(var joint in handsJoints)
        {
            JointDrive slerpDrive = joint.slerpDrive;
            slerpDrive.positionSpring = 8;

            joint.slerpDrive = slerpDrive;
        }
        animator.SetBool("pickUp", true);
        animator.SetLayerWeight(animator.GetLayerIndex(layerName), 1);
    }

    private void disableAnimatePickUp()
    {

        foreach (var joint in handsJoints)
        {
            JointDrive slerpDrive = joint.slerpDrive;
            slerpDrive.positionSpring = 1;

            joint.slerpDrive = slerpDrive;
        }

        animator.SetBool("pickUp", false);
        animator.SetLayerWeight(animator.GetLayerIndex(layerName), 0);
    }
}