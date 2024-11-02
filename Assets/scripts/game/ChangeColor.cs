using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [SerializeField]
    private Material turnOnMaterial;

    [SerializeField] 
    private Material turnOffMaterial;

    [SerializeField]
    private OpenDoor openDoorScript;

    private MeshRenderer meshRenderer;
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            meshRenderer.material = turnOnMaterial;
            openDoorScript.ButtonActivated();
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            meshRenderer.material = turnOffMaterial;
            openDoorScript.ButtonDeactivated();
        }
    }
}
