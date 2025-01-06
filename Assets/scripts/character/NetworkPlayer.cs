using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    [SerializeField]
    private string characterName = "";
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
    [SerializeField]
    private float fallThreshold = -5f;
    [SerializeField]
    private PhysicMaterial zeroFrictionMaterial;

    public Vector3 checkPoint = new(0, 0, 0);

    private Vector2 moveInput = Vector2.zero;
    private bool jumpButtonPressed = false;
    private float lastJumpPressTime = 0f;
    private float doubleClickThreshold = 0.5f;
    private bool doubleSpaceTriggered = false;
    private bool isGrounded = false;
    private RaycastHit[] raycastHits = new RaycastHit[10];
    private bool isCharacterVisible = false;
    private Collider playerCollider;
    private SyncPhysicsWithJoint[] syncPhysicsWithJoints;

    void Awake()
    {
        syncPhysicsWithJoints = GetComponentsInChildren<SyncPhysicsWithJoint>();
        playerCollider = GetComponent<Collider>();
    }

    public string GetCharacterName()
    {
        return characterName;
    }

    public void SetMoveInput(Vector2 move)
    {
        moveInput = move;
    }

    void FixedUpdate(){
        CheckIfCharacterFell();

        isGrounded = CheckIfGrounded();

        if (isGrounded)
        {
            playerCollider.material = null;
        }
        else
        {
            playerCollider.material = zeroFrictionMaterial;
        }

        if (!isGrounded && jumpButtonPressed){
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
                if (!cameraPositioner.IsNotFacingCamera(this.gameObject.transform))
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

    public void DetectDoubleSpacePress()
    {
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

        if (doubleSpaceTriggered && isGrounded)
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

    void CheckIfCharacterFell()
    {
        if (transform.position.y < fallThreshold)
        {
            this.rigidBody.velocity = Vector3.zero;
            this.rigidBody.angularVelocity = Vector3.zero;
            checkPoint.y += 2.0f;
            transform.position = checkPoint;
            checkPoint.y -= 2.0f;
        }
    }

    public void UpdateCheckpoint(Vector3 checkPointPosition)
    {
        checkPoint = checkPointPosition;
    }

    public void SavePlayer(string fileName)
    {
        SaveSystem.SavePlayer(this, fileName);
    }

    public PlayerData LoadPlayer(string fileName)
    {
        PlayerData data = SaveSystem.LoadPlayer(fileName);

        if(data == null)
        {
            return null;
        }

        Vector3 position;
        position.x = data.checkPointPosition[0];
        position.y = data.checkPointPosition[1] + 2;
        position.z = data.checkPointPosition[2];

        transform.position = position;

        return data;
    }
}
