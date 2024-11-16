using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePlatformBreak : MonoBehaviour
{
    public float breakDelay = 3f; 
    public float shakeIntensity = 0.05f; 
    public float shakeIncreaseRate = 0.01f; 
    public float shakeDuration = 3f;
    public float resetDelay = 2f;

    private MeshRenderer meshRenderer;
    private Collider platformCollider;
    private Vector3 originalPosition;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        platformCollider = GetComponent<Collider>();
        originalPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ShakeAndBreak());
        }
    }

    private IEnumerator ShakeAndBreak()
    {
        float elapsedTime = 0f;
        float currentShakeIntensity = shakeIntensity;

        while (elapsedTime < shakeDuration)
        {
            transform.position = originalPosition + new Vector3(
                Random.Range(-currentShakeIntensity, currentShakeIntensity),
                0,
                Random.Range(-currentShakeIntensity, currentShakeIntensity)
            );

            currentShakeIntensity += shakeIncreaseRate * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        transform.position = originalPosition;
        yield return new WaitForSeconds(breakDelay - shakeDuration);

        meshRenderer.enabled = false;
        platformCollider.enabled = false;

        yield return new WaitForSeconds(resetDelay);

        meshRenderer.enabled = true;
        platformCollider.enabled = true;
        transform.position = originalPosition;
    }
}
