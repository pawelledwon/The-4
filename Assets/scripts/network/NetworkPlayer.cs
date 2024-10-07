using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    [SerializeField]
    Rigidbody rigidBody;
    [SerializeField]
    ConfigurableJoint mainJoint;
    [SerializeField]
    Animator animator;



    Vector2 moveInput = Vector2.zero;
    bool jumpButtonPressed = false;

    float maxSpeed = 3.0f;
    bool isGrounded = false;
    RaycastHit[] raycastHits = new RaycastHit[10];

    SyncPhysicsWithJoint[] syncPhysicsWithJoints;

    void Awake()
    {
        syncPhysicsWithJoints = GetComponentsInChildren<SyncPhysicsWithJoint>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        if(Input.GetKeyDown(KeyCode.Space)){
            jumpButtonPressed = true;
        }
    }

    void FixedUpdate(){
        isGrounded = false;

        int numberOfHits = Physics.SphereCastNonAlloc(rigidBody.position, 0.1f, transform.up * -1, raycastHits, 0.5f);

        for(int i = 0; i < numberOfHits; i++){
            if(raycastHits[i].transform.root != transform){
                isGrounded = true;
                break;
            }
        }

        if(!isGrounded && jumpButtonPressed){
            rigidBody.AddForce(Vector3.down * 20);
        }

        float inputMagnitued = moveInput.magnitude;

        float localForwardSpeed = Vector3.Magnitude(rigidBody.velocity);

        if(inputMagnitued != 0){
            Quaternion desiredDirection = Quaternion.LookRotation(new Vector3(moveInput.x, 0, moveInput.y * -1), transform.up);
            
            mainJoint.targetRotation = Quaternion.RotateTowards(mainJoint.targetRotation, desiredDirection, Time.fixedDeltaTime * 300);
            
            if(localForwardSpeed < maxSpeed){
                rigidBody.AddForce(-transform.right * inputMagnitued * 30);
            }
        }

        if(isGrounded && jumpButtonPressed){
            rigidBody.AddForce(Vector3.up * 12, ForceMode.Impulse);

            jumpButtonPressed = false;
        }

        animator.SetFloat("movementSpeed", localForwardSpeed * 0.7f);

        foreach(var obj in syncPhysicsWithJoints)
        {
            obj.UpdateJointFromAnimation();
        }
    }
}
