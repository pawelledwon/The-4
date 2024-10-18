using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Linq;
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
    private string pickUpLayerName = "PickUp";

    [SerializeField]
    private string dragLayerName = "Drag";

    [SerializeField]
    private List<ConfigurableJoint> handsJoints;

    [SerializeField]
    private float additionalPickUpDistance = 0.5f;

    [SerializeField]
    private float additionalDragDistance = 1.0f;

    [SerializeField]
    private float maxCarryMass = 10f;

    [SerializeField]
    private float maxCarrySize = 1.5f;

    private bool pickUpPossible;
    
    private List<GameObject> possibleObjectsToPickUp = new List<GameObject>();

    public GameObject objectToPickUp;
    public bool hasItem;
    public bool isDragging;
    public Vector3 positionBetweenHands;

    void Start()
    {
        pickUpPossible = false;   
        hasItem = false;
        isDragging = false;
    }

    void Update()
    {
        checkForItemToPick();

        if (pickUpPossible)
        {
            pickUpItem();
        }

        if ((Input.GetKeyDown("e") && hasItem) || !pickUpPossible)
        {
            dropItem();
        }
    }

    void checkForItemToPick()
    {
        if (possibleObjectsToPickUp.Count > 0)
        {
            pickUpPossible = true;

            if (!hasItem)
            {
                objectToPickUp = getObjectInSight();
                decidePickUpOrDrag();
            }
        }
        else
        {
            pickUpPossible = false;
            objectToPickUp = null;
            isDragging = false;
        }
    }
    
    void pickUpItem()
    {
        if (Input.GetKeyDown("e"))
        {

            if (isDragging)
            {
                animateDragging();
            }
            else
            {
                animatePickUp();
            }
        }
        
    }

    public void dropItem()
    {
        
        if (isDragging)
        {
            disableAnimateDragging();
            stopDragging();
        }
        else
        {
            disableAnimatePickUp();
            changeObjPosToDropped();
        }
        
    }

    public void onObjectInRange(GameObject obj)
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

    public void onObjectOutOfRange(GameObject obj)
    {
        if (obj.CompareTag("PickUpObject"))
        {
            possibleObjectsToPickUp.Remove(obj.gameObject);
        }
    }

    private GameObject getObjectInSight()
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

    public void calculateTargetPosition()
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

        positionBetweenHands = midpoint + direction * additionalPickUpDistance;
    }

    public void changeObjPosToPicked()
    {
        hasItem = true;
        objectToPickUp.GetComponent<Rigidbody>().isKinematic = true;

        calculateTargetPosition();
        objectToPickUp.transform.position = positionBetweenHands;
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

    private void animatePickUp()
    {
        foreach (var joint in handsJoints)
        {
            JointDrive slerpDrive = joint.slerpDrive;
            slerpDrive.positionSpring = 8;

            joint.slerpDrive = slerpDrive;
        }

        animator.SetBool("pickUp", true);
        animator.SetLayerWeight(animator.GetLayerIndex(pickUpLayerName), 1);
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
        animator.SetLayerWeight(animator.GetLayerIndex(pickUpLayerName), 0);
    }

    private void animateDragging()
    {
        foreach (var joint in handsJoints)
        {
            JointDrive slerpDrive = joint.slerpDrive;
            slerpDrive.positionSpring = 8;

            joint.slerpDrive = slerpDrive;
        }
        print("essa");
        animator.SetBool("drag", true);
        animator.SetLayerWeight(animator.GetLayerIndex(dragLayerName), 1);
    }

    private void disableAnimateDragging()
    {
        foreach (var joint in handsJoints)
        {
            JointDrive slerpDrive = joint.slerpDrive;
            slerpDrive.positionSpring = 1;

            joint.slerpDrive = slerpDrive;
        }

        animator.SetBool("drag", false);
        animator.SetLayerWeight(animator.GetLayerIndex(dragLayerName), 0);
    }

    // Dragging Logic
    private void startDragging()
    {
        hasItem = true;
        objectToPickUp.GetComponent<Rigidbody>().isKinematic = false;
        objectToPickUp.transform.parent = null;

        StartCoroutine(dragObject());
    }

    private void stopDragging()
    {
        hasItem = false;
        isDragging = false;
        StopCoroutine(dragObject());
    }

    private IEnumerator dragObject()
    {
        while (hasItem)
        {
            calculateTargetPosition();

            Vector3 direction = (positionBetweenHands - objectToPickUp.transform.position).normalized;
            positionBetweenHands = positionBetweenHands - direction * additionalDragDistance;
            positionBetweenHands.y = objectToPickUp.transform.position.y;

            objectToPickUp.transform.position = Vector3.Lerp(objectToPickUp.transform.position, positionBetweenHands, Time.deltaTime * 10);

            yield return null;
        }
    }
}
