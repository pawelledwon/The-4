using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePlatformBreak : MonoBehaviour
{
    public float breakDelay = 3f; // Total time before the platform disappears
    public float blinkDuration = 3f; // Duration for blinking effect
    public float resetDelay = 2f; // Time before the platform reappears

    [SerializeField]
    private Material originalMaterial; // Material for normal state
    [SerializeField]
    private Material blinkMaterial; // Material during blinking effect

    private MeshRenderer meshRenderer;
    private bool isBlinking = false; // Track if blinking is active
    private bool isUsingBlinkMaterial = false; // Flag to track current material

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = originalMaterial; // Ensure the platform starts with the original material
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isBlinking)
        {
            StartCoroutine(BlinkAndBreak());
        }
    }

    private IEnumerator BlinkAndBreak()
    {
        isBlinking = true;
        float elapsedTime = 0f;

        // Start blinking effect
        while (elapsedTime < blinkDuration)
        {
            float blinkInterval = Mathf.Lerp(0.5f, 0.05f, elapsedTime / blinkDuration); // Blinking frequency increases over time

            // Toggle material between original and blink materials using flag
            if (isUsingBlinkMaterial)
            {
                meshRenderer.material = originalMaterial;
            }
            else
            {
                meshRenderer.material = blinkMaterial;
            }
            isUsingBlinkMaterial = !isUsingBlinkMaterial;

            yield return new WaitForSeconds(blinkInterval); // Wait for the interval
            elapsedTime += blinkInterval;
        }

        // Ensure the platform is back to its original material before disappearing
        meshRenderer.material = originalMaterial;
        yield return new WaitForSeconds(breakDelay - blinkDuration);

        // Simulate disappearance
        meshRenderer.enabled = false;
        Collider platformCollider = GetComponent<Collider>();
        platformCollider.enabled = false;
        DeleteChildren();

        yield return new WaitForSeconds(resetDelay);

        // Re-enable platform and reset material
        platformCollider.enabled = true;
        meshRenderer.enabled = true;
        meshRenderer.material = originalMaterial;
        isBlinking = false;
    }

    private void DeleteChildren()
    {
        foreach (Transform child in this.transform)
        {
            if (child.tag == "Player")
                child.parent = null;
        }
    }
}
