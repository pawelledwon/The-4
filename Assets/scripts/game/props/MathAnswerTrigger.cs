using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MathAnswerTrigger : MonoBehaviour
{
    [SerializeField]
    private MathTask mathTask;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BasketBall")
        {
            mathTask.CheckIfCorrectAnswer(this.gameObject, other.gameObject.GetComponentInParent<GetBackToSpawn>());
        }
    }
}
