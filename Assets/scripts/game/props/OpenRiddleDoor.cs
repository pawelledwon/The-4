using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OpenRiddleDoor : MonoBehaviour
{
    [SerializeField]
    private float openDuration = 3f;

    [SerializeField]
    private float moveDistance = -3f;

    [SerializeField]
    List<WindowRiddle> windows;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isOpening = false;
    private bool openDoor = false;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(0f, moveDistance, 0f);
    }

    public void OpenDoor()
    {
        CheckIfAllWindowsActive();

        if (!isOpening && openDoor)
        {
            StartCoroutine(OpenDoorCoroutine());
        }
    }

    private IEnumerator OpenDoorCoroutine()
    {
        isOpening = true;
        float elapsedTime = 0f;

        while (elapsedTime < openDuration)
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / openDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isOpening = false;
    }

    private void CheckIfAllWindowsActive()
    {
        openDoor = true;

        foreach(WindowRiddle win in windows)
        {
            if (!win.isActive)
            {
                openDoor = false;
                break;
            }
        }
    }
}
