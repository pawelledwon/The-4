using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : MonoBehaviour
{
    [SerializeField]
    private float throwForce = 10f;

    [SerializeField]
    private Animator animator;

    private PickUpObject pickUpObject;

    void Start()
    {
        pickUpObject = GetComponent<PickUpObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown("q") && pickUpObject.hasItem && !pickUpObject.isDragging)
        {
            animator.SetTrigger("throw");
        }
    }

    void Throw()
    {
        Rigidbody rb = pickUpObject.objectToPickUp.GetComponent<Rigidbody>();

        pickUpObject.calculateTargetPosition();

        var throwDirection = pickUpObject.positionBetweenHands - transform.position;

        pickUpObject.dropItem();

        if (rb != null)
        {
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }
    }

}
