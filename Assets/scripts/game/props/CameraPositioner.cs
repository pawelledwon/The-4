using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositioner : MonoBehaviour
{
    [SerializeField]
    private Transform[] characters; 

    [SerializeField]
    private float padding = 2f;

    [SerializeField]
    private float distanceFromCharacters = 7f;

    [SerializeField]
    private float xCameraAngle = 30f;

    [SerializeField]
    private float yCameraAngle = 30f;

    [SerializeField]
    private float transitionSpeed = 2f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (characters.Length == 0 || CheckpointManager.Instance == null || CheckpointManager.Instance.checkpoints.Count < 2)
            return;

        List<Checkpoint> checkpoints = CheckpointManager.Instance.checkpoints;
        int currentCheckpointIndex = CheckpointManager.Instance.GetCurrentCheckPointIndex();

        if (currentCheckpointIndex < 0)
            return;

        Transform currentCheckpoint;
        Transform nextCheckpoint;

        if (currentCheckpointIndex >= checkpoints.Count - 1)
        {
            currentCheckpoint = checkpoints[currentCheckpointIndex].transform;
            nextCheckpoint = checkpoints[currentCheckpointIndex].transform;
        }
        else
        {
            currentCheckpoint = checkpoints[currentCheckpointIndex].transform;
            nextCheckpoint = checkpoints[currentCheckpointIndex + 1].transform;
        }
        

        Vector3 centerPosition = GetCharacterCenterPosition();

        float startY = currentCheckpoint.position.y;
        float endY = nextCheckpoint.position.y;
        float progress = Mathf.InverseLerp(startY, endY, centerPosition.y);
        float interpolatedY = Mathf.Lerp(startY, endY, progress);

        Vector3 newPosition = new Vector3(
             centerPosition.x - distanceFromCharacters,
             Mathf.Lerp(transform.position.y, interpolatedY + padding, Time.deltaTime * transitionSpeed),
             centerPosition.z 
         );

        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(xCameraAngle, yCameraAngle, 0);
    }
    private Vector3 GetCharacterCenterPosition()
    {
        Bounds bounds = new Bounds(characters[0].position, Vector3.zero);
        foreach (Transform character in characters)
        {
            bounds.Encapsulate(character.position);
        }

        return bounds.center;
    }

    public bool IsInView(GameObject toCheck)
    {
        Vector3 pointOnScreen = cam.WorldToScreenPoint(toCheck.GetComponentInChildren<Renderer>().bounds.center);

        if (pointOnScreen.z < 0)
        {
            return false;
        }

        if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) ||
                (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height))
        {
            return false;
        }

        return true;
    }

    public bool IsFacingCamera(Transform characterTransform)
    {
        var characterRotation = characterTransform.rotation;
        var cameraRotation = cam.transform.rotation;

        var leftLimit = cameraRotation * Quaternion.Euler(0, 150, 0);
        var rightLimit = cameraRotation * Quaternion.Euler(0, -30,0);

        Debug.Log($"lewo: {leftLimit.eulerAngles.y}, prawo: {rightLimit.eulerAngles.y}, postac: {characterRotation.eulerAngles.y}");

        if(rightLimit.eulerAngles.y < leftLimit.eulerAngles.y)
        {
            return characterRotation.eulerAngles.y > rightLimit.eulerAngles.y && characterRotation.eulerAngles.y < leftLimit.eulerAngles.y;
        }
        
        if(rightLimit.eulerAngles.y > leftLimit.eulerAngles.y)
        {
            return characterRotation.eulerAngles.y < rightLimit.eulerAngles.y && characterRotation.eulerAngles.y > leftLimit.eulerAngles.y;
        }

        return false;
    }

}
