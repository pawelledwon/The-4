using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField]
    private float newMass = 1f; // The mass to set when the door falls
    [SerializeField]
    private float fallForce = 10f; // The force applied to make the door fall
    [SerializeField]
    private List<GameObject> buttons;

    private Rigidbody doorRigidbody;
    private int activeButtons = 0;
    private bool doorOpened = false;

    private void Start()
    {
        doorRigidbody = GetComponent<Rigidbody>();
    }

    // Call this function from each button when activated
    public void ButtonActivated()
    {
        activeButtons++;
        CheckDoorStatus();
    }

    // Call this function from each button when deactivated
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

    private void OpenTheDoor()
    {
        doorRigidbody.mass = newMass;

        doorRigidbody.AddForce(Vector3.forward * fallForce, ForceMode.Impulse);
    }
}
