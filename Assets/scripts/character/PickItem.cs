using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    [SerializeField]
    GameObject leftHand;

    [SerializeField]
    GameObject rightHand;

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
}