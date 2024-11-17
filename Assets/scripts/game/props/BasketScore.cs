using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketScore : MonoBehaviour
{
    [SerializeField]
    private OpenDoor openDoorScript;

    private bool doorOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "BasketBall" && !doorOpened)
        {
            openDoorScript.OpenTheDoor();
            doorOpened = true;
        }
    }

}
