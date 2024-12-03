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

    public void PickUpItem()
    {
        checkForItemToPick();

        var pickedItemScript = objectToPickUp.GetComponent<PickedItem>();

        if (pickUpPossible && !hasItem && pickedItemScript != null && !pickedItemScript.IsItemPicked(this.gameObject))
        {
            if (pickedItemScript != null)
            {
                pickedItemScript.SetPlayer(this.gameObject);
            }

            pickUpItem();
        }

        if (!pickUpPossible || hasItem || pickedItemScript == null || pickedItemScript.IsItemPicked(this.gameObject))
        {

            if (pickedItemScript != null)
            {
                pickedItemScript.SetPlayer(null);
            }

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

            if(objectToPickUp != null)
            {
                dropItem();
                objectToPickUp = null;
            }
        }
    }
    
    void pickUpItem()
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
        if (obj.CompareTag("PickUpObject"))
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
        float minDistance = float.MaxValue;
        

        foreach (GameObject obj in possibleObjectsToPickUp)
        {
            if(obj != null)
            {
                float distance = Vector3.Distance(obj.GetComponent<Renderer>().bounds.center, transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestObject = obj;
                }
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
        if(objectToPickUp != null)
        {
            hasItem = true;
            objectToPickUp.GetComponent<Rigidbody>().isKinematic = true;

            calculateTargetPosition();
            objectToPickUp.GetComponent<Collider>().enabled = false;
            objectToPickUp.transform.position = positionBetweenHands;
            objectToPickUp.transform.parent = leftHand.transform;
        }
       
    }

    public void changeObjPosToDropped()
    {
        if (objectToPickUp != null)
        {
            objectToPickUp.GetComponent<Rigidbody>().isKinematic = false;
            objectToPickUp.transform.parent = null;
            objectToPickUp.GetComponent<Collider>().enabled = true;
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(
            objectToPickUp,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene()
        );
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
        if (objectToPickUp != null) 
        {
            hasItem = true;
            objectToPickUp.GetComponent<Rigidbody>().isKinematic = false;
            objectToPickUp.transform.parent = null;
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(
            objectToPickUp,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene()
            );

            StartCoroutine(dragObject());
        }
        else
        {
            disableAnimateDragging();
        }
       
    }

    private void stopDragging()
    {
        hasItem = false;
        isDragging = false;
        StopCoroutine(dragObject());
    }

    private IEnumerator dragObject()
    {
        Vector3 direction = -transform.right;
        Quaternion initialRotationOffset = Quaternion.Inverse(Quaternion.LookRotation(direction, Vector3.up)) * objectToPickUp.transform.rotation;

        float objectDragDistance = calculateRelevantDragDistance(objectToPickUp, direction);

        while (hasItem)
        {
            calculateTargetPosition();

            direction = -transform.right;

            float dynamicDragDistance = additionalDragDistance + objectDragDistance;

            positionBetweenHands = positionBetweenHands + direction * dynamicDragDistance;
            positionBetweenHands.y = objectToPickUp.transform.position.y;

            objectToPickUp.transform.position = Vector3.Lerp(objectToPickUp.transform.position, positionBetweenHands, Time.deltaTime * 10);

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up) * initialRotationOffset;
            objectToPickUp.transform.rotation = Quaternion.Lerp(
                objectToPickUp.transform.rotation,
                targetRotation,
                Time.deltaTime * 10
            );

            yield return null;
        }
    }

    private float calculateRelevantDragDistance(GameObject obj, Vector3 direction)
    {
        Bounds bounds = obj.GetComponent<Renderer>().bounds;

        Vector3 localDirection = obj.transform.InverseTransformDirection(direction);

        float relevantDistance = Mathf.Abs(Vector3.Dot(bounds.extents, localDirection.normalized));

        return relevantDistance;
    }


}
