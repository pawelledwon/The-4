using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rigidBody;
    [SerializeField]
    private ConfigurableJoint mainJoint;
    [SerializeField]
    private Animator animator;

    private Vector2 moveInput = Vector2.zero;
    private bool jumpButtonPressed = false;
    private float lastJumpPressTime = 0f;
    private float doubleClickThreshold = 0.5f;
    private bool doubleSpaceTriggered = false;
    private float maxSpeed = 3.0f;
    private bool isGrounded = false;
    private RaycastHit[] raycastHits = new RaycastHit[10];

    private SyncPhysicsWithJoint[] syncPhysicsWithJoints;

    void Awake()
    {
        syncPhysicsWithJoints = GetComponentsInChildren<SyncPhysicsWithJoint>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        if(Input.GetKeyDown(KeyCode.Space)){
            DetectDoubleSpacePress();
        }
    }

    void FixedUpdate(){
        isGrounded = CheckIfGrounded();

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

    private bool CheckIfGrounded()
{
    float rayLength = 0.7f;  // Length of rays for detecting the ground
    Vector3[] raycastOrigins = new Vector3[3];

    // Define raycast origins (feet and center)
    raycastOrigins[0] = transform.position + Vector3.up * 0.1f;  // Center bottom
    raycastOrigins[1] = transform.position + Vector3.right * 0.2f;  // Right foot
    raycastOrigins[2] = transform.position + Vector3.left * 0.2f;  // Left foot

    foreach (var origin in raycastOrigins)
    {
        if (Physics.Raycast(origin, Vector3.down, rayLength))
        {
            return true;  // Return true if any ray hits the ground
        }
    }

    return false;  // Return false if no rays hit the ground
}

void DetectDoubleSpacePress()
    {
        print(doubleSpaceTriggered);
        print(isGrounded);
        print(jumpButtonPressed);
        if (Time.time - lastJumpPressTime < doubleClickThreshold) // Check if it's a double-click
        {
            doubleSpaceTriggered = true; // Double space detected, allow joint changes
            SetConfigurableJointToFree();
        }
        else
        {
            // Detected a single press, set jumpButtonPressed to true for normal jump
            jumpButtonPressed = true;
        }

        lastJumpPressTime = Time.time; // Update the time of last press

        if (doubleSpaceTriggered && isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            SetConfigurableJointToLocked();
            doubleSpaceTriggered = false; // Reset double space trigger after locking joint
        }
    }

    // Function to modify ConfigurableJoint when double space is detected
    void SetConfigurableJointToFree()
    {
        // Set angular motion for x, y, z to free
        mainJoint.angularXMotion = ConfigurableJointMotion.Free;
        mainJoint.angularYMotion = ConfigurableJointMotion.Free;
        mainJoint.angularZMotion = ConfigurableJointMotion.Free;

        // Set slerp drive to 5
        JointDrive slerpDrive = mainJoint.slerpDrive;
        slerpDrive.positionSpring = 5;
        mainJoint.slerpDrive = slerpDrive;

        Debug.Log("Double space pressed! Configurable joint is now free, slerp set to 5.");
    }

    // Function to modify ConfigurableJoint when grounded and space is pressed after double space
    void SetConfigurableJointToLocked()
    {
        // Set angular motion for x and z to locked, keep y free
        mainJoint.angularXMotion = ConfigurableJointMotion.Locked;
        mainJoint.angularYMotion = ConfigurableJointMotion.Free;
        mainJoint.angularZMotion = ConfigurableJointMotion.Locked;

        // Set slerp drive to 100
        JointDrive slerpDrive = mainJoint.slerpDrive;
        slerpDrive.positionSpring = 100;
        mainJoint.slerpDrive = slerpDrive;

        Debug.Log("Character grounded and space pressed. Configurable joint x and z locked, slerp set to 100.");
    }
}
