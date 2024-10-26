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
    [SerializeField]
    private CameraPositioner cameraPositioner;
    [SerializeField]
    private float maxSpeed = 2.0f;

    private Vector2 moveInput = Vector2.zero;
    private bool jumpButtonPressed = false;
    private float lastJumpPressTime = 0f;
    private float doubleClickThreshold = 0.5f;
    private bool doubleSpaceTriggered = false;
    private bool isGrounded = false;
    private RaycastHit[] raycastHits = new RaycastHit[10];
    private bool isCharacterVisible = false;

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

        if(!isGrounded && jumpButtonPressed){
            rigidBody.AddForce(Vector3.down * 20);
        }

        float inputMagnitued = moveInput.magnitude;

        float localForwardSpeed = Vector3.Magnitude(rigidBody.velocity);

        if(inputMagnitued != 0){
            
            Quaternion desiredDirection = Quaternion.LookRotation(new Vector3(moveInput.x * -1, 0, moveInput.y), transform.up);
            
            mainJoint.targetRotation = Quaternion.RotateTowards(mainJoint.targetRotation, desiredDirection, Time.fixedDeltaTime * 300);


            isCharacterVisible = cameraPositioner.IsInView(this.gameObject);

            if(isCharacterVisible)
            {
                if (localForwardSpeed < maxSpeed)
                {
                    rigidBody.AddForce(-transform.right * inputMagnitued * 30);
                }
            }
            else
            {
                if (!cameraPositioner.IsFacingCamera(this.gameObject.transform))
                {
                    rigidBody.AddForce(-transform.right * inputMagnitued * 30);
                }
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
        float rayLength = 0.7f; 
        Vector3[] raycastOrigins = new Vector3[3];

        raycastOrigins[0] = transform.position + Vector3.up * 0.1f;  
        raycastOrigins[1] = transform.position + Vector3.right * 0.2f; 
        raycastOrigins[2] = transform.position + Vector3.left * 0.2f; 

        foreach (var origin in raycastOrigins)
        {
            if (Physics.Raycast(origin, Vector3.down, rayLength))
            {
                return true;  
            }
        }

        return false;  
    }

void DetectDoubleSpacePress()
    {
        print(doubleSpaceTriggered);
        print(isGrounded);
        print(jumpButtonPressed);
        if (Time.time - lastJumpPressTime < doubleClickThreshold)
        {
            doubleSpaceTriggered = true; 
            SetConfigurableJointToFree();
        }
        else
        {
            jumpButtonPressed = true;
        }

        lastJumpPressTime = Time.time;

        if (doubleSpaceTriggered && isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            SetConfigurableJointToLocked();
            doubleSpaceTriggered = false;
        }
    }

    void SetConfigurableJointToFree()
    {
        mainJoint.angularXMotion = ConfigurableJointMotion.Free;
        mainJoint.angularYMotion = ConfigurableJointMotion.Free;
        mainJoint.angularZMotion = ConfigurableJointMotion.Free;

        JointDrive slerpDrive = mainJoint.slerpDrive;
        slerpDrive.positionSpring = 5;
        mainJoint.slerpDrive = slerpDrive;
    }

    void SetConfigurableJointToLocked()
    {
        mainJoint.angularXMotion = ConfigurableJointMotion.Locked;
        mainJoint.angularYMotion = ConfigurableJointMotion.Free;
        mainJoint.angularZMotion = ConfigurableJointMotion.Locked;

        JointDrive slerpDrive = mainJoint.slerpDrive;
        slerpDrive.positionSpring = 100;
        mainJoint.slerpDrive = slerpDrive;
    }
}
