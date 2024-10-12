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
    private GameObject leftHand;

    [SerializeField]
    private GameObject rightHand;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    string layerName = "PickUp";

    [SerializeField]
    private List<ConfigurableJoint> handsJoints;

    [SerializeField]
    private float additionalDistance = 0.5f;

    [SerializeField]
    private float maxCarryMass = 10f;

    [SerializeField]
    private float maxCarrySize = 1.5f;

    private bool pickUpPossible;
    private bool hasItem;
    private bool isDragging;

    private List<GameObject> possibleObjectsToPickUp = new List<GameObject>();
    private GameObject objectToPickUp;
    void Start()
    {
        pickUpPossible = false;   
        hasItem = false;
        isDragging = false;
    }

    void Update()
    {
        print(possibleObjectsToPickUp.Count);
        if(possibleObjectsToPickUp.Count > 0) 
        {
            pickUpPossible = true;

            if (!hasItem)
            {
                objectToPickUp = GetObjectInSight();
                decidePickUpOrDrag();
            }
            
        }
        else
        {
            pickUpPossible = false;
            objectToPickUp = null;
            isDragging = false;
        }

        if (pickUpPossible)
        {
            if (Input.GetKeyDown("e"))
            {

                if (isDragging)
                {
                    StartDragging();
                }
                else
                {
                    animatePickUp();
                }
            }
        }

        if ((Input.GetKeyDown("q") && hasItem) || !pickUpPossible)
        {
            if (isDragging)
            {
                StopDragging();
            }
            else
            {
                disableAnimatePickUp();
                changeObjPosToDropped();
            }
        }
    }

    public void OnObjectInRange(GameObject obj)
    {
        if (obj.CompareTag("PickUpObject")) // Assuming objects have a tag "PickUpObject"
        {
            possibleObjectsToPickUp.Add(obj.gameObject);
        }
    }

    private void decidePickUpOrDrag()
    {
        Rigidbody objRb = objectToPickUp.GetComponent<Rigidbody>();
        Vector3 objSize = objectToPickUp.GetComponent<Collider>().bounds.size;

        if (objRb.mass > maxCarryMass || objSize.magnitude > maxCarrySize)
        {
            isDragging = true;
        }
        else
        {
            isDragging = false;
        }
    }

    public void OnObjectOutOfRange(GameObject obj)
    {
        if (obj.CompareTag("PickUpObject"))
        {
            possibleObjectsToPickUp.Remove(obj.gameObject);
        }
    }

    private GameObject GetObjectInSight()
    {
        GameObject closestObject = null;
        float maxDotProduct = -1f; 

        foreach (GameObject obj in possibleObjectsToPickUp)
        {
            Vector3 directionToObj = (obj.transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToObj);

            if (dotProduct > maxDotProduct)
            {
                maxDotProduct = dotProduct;
                closestObject = obj;
            }
        }

        return closestObject;
    }

    private Vector3 calculateTargetPosition()
    {
        float a = leftHand.transform.position.x;
        float b = leftHand.transform.position.y;
        float c = leftHand.transform.position.z;
        float d = rightHand.transform.position.x;
        float e = rightHand.transform.position.y;
        float f = rightHand.transform.position.z;

        Vector3 midpoint = new Vector3((a + d) / 2, (b + e) / 2, (c + f) / 2);
        Vector3 playerPosition = transform.position;

        Vector3 direction = (midpoint - playerPosition);
        direction.y = 0;

        Vector3 extendedPosition = midpoint + direction * additionalDistance;
        return extendedPosition;
    }

    private void animatePickUp()
    {
        foreach (var joint in handsJoints)
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

    public void changeObjPosToPicked()
    {
        hasItem = true;
        objectToPickUp.GetComponent<Rigidbody>().isKinematic = true;

        Vector3 pickedObjectPosition = calculateTargetPosition();
        objectToPickUp.transform.position = pickedObjectPosition;
        objectToPickUp.transform.parent = leftHand.transform;
        objectToPickUp.GetComponent<Collider>().enabled = false;
    }

    public void changeObjPosToDropped()
    {
        if (objectToPickUp != null)
        {
            objectToPickUp.GetComponent<Rigidbody>().isKinematic = false;
            objectToPickUp.transform.parent = null;
            objectToPickUp.GetComponent<Collider>().enabled = true;
        }

        hasItem = false;
    }

    // Dragging Logic
    private void StartDragging()
    {
        hasItem = true;
        objectToPickUp.GetComponent<Rigidbody>().isKinematic = false;
        objectToPickUp.transform.parent = null;

        StartCoroutine(DragObject());
    }

    private void StopDragging()
    {
        hasItem = false;
        isDragging = false;
        StopCoroutine(DragObject());
    }

    private IEnumerator DragObject()
    {
        while (hasItem)
        {
            Vector3 playerPosition = transform.position;
            Vector3 dragPosition = playerPosition + transform.forward; 
            dragPosition.y = objectToPickUp.transform.position.y;
            objectToPickUp.transform.position = Vector3.Lerp(objectToPickUp.transform.position, dragPosition, Time.deltaTime * 10);

            yield return null;
        }
    }
}
