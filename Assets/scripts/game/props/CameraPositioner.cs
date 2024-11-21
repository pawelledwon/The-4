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
    private float cameraHeight = 5f;

    [SerializeField]
    private float distanceFromCharacters = 7f;

    [SerializeField]
    private float minFOV = 20f;

    [SerializeField]
    private float xCameraAngle = 30f;

    [SerializeField]
    private float yCameraAngle = 30f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (characters.Length == 0) return;

        Bounds bounds = new Bounds(characters[0].position, Vector3.zero);
        foreach (Transform character in characters)
        {
            bounds.Encapsulate(character.position);
        }

        Vector3 centerPosition = bounds.center;

        Vector3 newPosition = new Vector3(centerPosition.x + distanceFromCharacters*1.5f, centerPosition.y + cameraHeight, centerPosition.z - distanceFromCharacters);
        
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(xCameraAngle, yCameraAngle, 0);

        AdjustCameraToFit(bounds);
    }

    void AdjustCameraToFit(Bounds bounds)
    {
        float distance = cameraHeight;
        float size = Mathf.Max(bounds.size.x, bounds.size.z) / 2f + padding;
        float fov = 2f * Mathf.Atan(size / distance) * Mathf.Rad2Deg;
        cam.fieldOfView = Mathf.Clamp(fov, minFOV, 179f);
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
