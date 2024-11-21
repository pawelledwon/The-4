using System.Collections;
using UnityEngine;

public class FinishGameShowContinue : MonoBehaviour
{
    [SerializeField]
    private GameObject continueButton;

    void Start()
    {
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }

        StartCoroutine(EnableContinueButtonAfterDelay(10f));
    }

    private IEnumerator EnableContinueButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (continueButton != null)
        {
            continueButton.SetActive(true);
        }
    }
}
