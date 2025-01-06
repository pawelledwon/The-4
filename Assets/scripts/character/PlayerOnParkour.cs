using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnParkour : MonoBehaviour
{
    private Vector3 platformLastPosition;
    private bool isOnPlatform = false;
    private Transform currentPlatform; 

    private void Update()
    {
        if (isOnPlatform && currentPlatform != null)
        {
            Vector3 platformDelta = currentPlatform.position - platformLastPosition;

            transform.position += platformDelta;

            platformLastPosition = currentPlatform.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ParkourObject"))
        {
            isOnPlatform = true;                         
            currentPlatform = collision.transform;        
            platformLastPosition = currentPlatform.position; 
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ParkourObject"))
        {
            isOnPlatform = false;     
            currentPlatform = null;   
        }
    }
}
