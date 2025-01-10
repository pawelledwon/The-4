using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField]
    private float newMass = 1f; 
    [SerializeField]
    private float fallForce = 10f;
    [SerializeField]
    private List<GameObject> buttons;

    private Rigidbody doorRigidbody;
    private int activeButtons = 0;
    private bool doorOpened = false;

    private void Start()
    {
        doorRigidbody = GetComponent<Rigidbody>();
    }

    public void ButtonActivated()
    {
        activeButtons++;
        CheckDoorStatus();
    }

    public void ButtonDeactivated()
    {
        activeButtons--;
        CheckDoorStatus();
    }

    private void CheckDoorStatus()
    {
        if (activeButtons == buttons.Count && !doorOpened)
        {
            OpenTheDoor();

            doorOpened = true;
        }
    }

    public void OpenTheDoor()
    {
        doorRigidbody.mass = newMass;

        doorRigidbody.AddForce(Vector3.forward * fallForce, ForceMode.Impulse);
    }
}
