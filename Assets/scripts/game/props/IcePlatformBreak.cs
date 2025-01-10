using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePlatformBreak : MonoBehaviour
{
    public float breakDelay = 3f; 
    public float blinkDuration = 3f;
    public float resetDelay = 2f; 

    [SerializeField]
    private Material originalMaterial;
    [SerializeField]
    private Material blinkMaterial;

    private MeshRenderer meshRenderer;
    private bool isBlinking = false; 
    private bool isUsingBlinkMaterial = false; 

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = originalMaterial; 
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

        while (elapsedTime < blinkDuration)
        {
            float blinkInterval = Mathf.Lerp(0.5f, 0.05f, elapsedTime / blinkDuration); 

            if (isUsingBlinkMaterial)
            {
                meshRenderer.material = originalMaterial;
            }
            else
            {
                meshRenderer.material = blinkMaterial;
            }
            isUsingBlinkMaterial = !isUsingBlinkMaterial;

            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        meshRenderer.material = originalMaterial;
        yield return new WaitForSeconds(breakDelay - blinkDuration);

        meshRenderer.enabled = false;
        Collider platformCollider = GetComponent<Collider>();
        platformCollider.enabled = false;
        DeleteChildren();

        yield return new WaitForSeconds(resetDelay);

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
