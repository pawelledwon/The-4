using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBackToSpawn : MonoBehaviour
{
    [SerializeField]
    private float fallThreshold = -10.0f;

    private Vector3 startPosition;
    private Quaternion rotation;
    private Rigidbody rigidBody;

    void Start()
    {
        startPosition = transform.position;
        rotation = transform.rotation;
        rigidBody = GetComponent<Rigidbody>();

    }
    void Update()
    {
        CheckIfItemFell();
    }

    void CheckIfItemFell()
    {
        if (transform.position.y < fallThreshold)
        {
            this.rigidBody.velocity = Vector3.zero;
            this.rigidBody.angularVelocity = Vector3.zero;
            startPosition.y += 1.0f;
            transform.position = startPosition;
            transform.rotation = rotation;
            startPosition.y -= 1.0f;
        }
    }
}
