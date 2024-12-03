using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnParkour : MonoBehaviour
{
    private Transform originalParent;  

    private void Start()
    {
        originalParent = transform.parent;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ParkourObject"))
        {
            transform.SetParent(collision.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ParkourObject"))
        {
            transform.SetParent(originalParent);
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
