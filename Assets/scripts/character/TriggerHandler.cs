using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public PickUpObject pickUpObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PickUpObject" && pickUpObject != null)
        {
            pickUpObject.onObjectInRange(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PickUpObject" && pickUpObject != null)
        {
            pickUpObject.onObjectOutOfRange(other.gameObject);
        }
    }
}
