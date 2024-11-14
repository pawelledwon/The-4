using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowRiddle : MonoBehaviour
{
    [SerializeField]
    private GameObject correctGameObject;
    [SerializeField]
    private float throwAwayForce = 5.0f;
    [SerializeField]
    private OpenRiddleDoor openRiddleDoor;

    public bool isActive = false;

    private Light activationLight;

    private void Start()
    {
        activationLight = GetComponentInChildren<Light>();
        activationLight.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PickUpObject")
        {
            if(other.gameObject ==  correctGameObject)
            {
                ActivateWindow();
            }
            else
            {
                ThrowObjectAway(other.gameObject);
            }
        }
    }

    private void ActivateWindow()
    {
        isActive = true;
        activationLight.enabled = true;
        openRiddleDoor.OpenDoor();

        Debug.LogWarning("Active");
    }

    private void ThrowObjectAway(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(Vector3.right * throwAwayForce, ForceMode.Impulse);
            Debug.LogWarning("Throw");
        }
        else
        {
            Debug.LogWarning("No Rigidbody found on the object.");
        }
    }
}
