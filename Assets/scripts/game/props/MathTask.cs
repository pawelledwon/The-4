using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Answer
{
    public TextMeshPro answerText;
    public GameObject answerBox;
}

public class MathTask : MonoBehaviour
{
    [SerializeField]
    private int minTaskValue;
    [SerializeField]
    private int maxTaskValue;
    [SerializeField]
    private GameObject cageToOpen;
    [SerializeField]
    private TextMeshPro taskText;
    [SerializeField]
    private List<Answer> taskAnswers = new List<Answer>();

    private List<char> signs = new List<char>(){ '-', '+', '*' };
    private int leftNumber;
    private int rightNumber;
    private char chosenSign;
    private Answer correctAnswer;
    private bool taskSolved = false;

    void Start()
    {
        RandomizeTask();
    }

    private void SetTaskText()
    {
        taskText.text = $"{leftNumber}{chosenSign}{rightNumber}=?";
    }

    private void SetTaskAnswers()
    {
        correctAnswer = taskAnswers.PickRandom();

        foreach (var answer in taskAnswers)
        {
            if(answer == correctAnswer)
            {
                switch (chosenSign)
                {
                    case '+':
                        answer.answerText.text = (leftNumber + rightNumber).ToString();
                        break;
                    case '-':
                        answer.answerText.text = (leftNumber - rightNumber).ToString();
                        break;
                    case '*':
                        answer.answerText.text = (leftNumber * rightNumber).ToString();
                        break;
                }
            }
            else
            {
                answer.answerText.text = Random.Range(0, maxTaskValue).ToString();
            }
        }
    }

    private void RandomizeTask()
    {
        chosenSign = signs.PickRandom();

        if (chosenSign == '-' || chosenSign == '+')
        {
            maxTaskValue *= 10;
        }

        leftNumber = Random.Range(minTaskValue, maxTaskValue);
        rightNumber = Random.Range(minTaskValue, maxTaskValue);

        SetTaskText();
        SetTaskAnswers();
    }

    public void CheckIfCorrectAnswer(GameObject answerBox, GetBackToSpawn ball)
    {
        if (!taskSolved)
        {
            if (answerBox == correctAnswer.answerBox)
            {
                cageToOpen.SetActive(false);
                taskSolved = true;
            }
            else
            {
                ball.ResetToSpawn();
                RandomizeTask();
            }
        }
    }


}
