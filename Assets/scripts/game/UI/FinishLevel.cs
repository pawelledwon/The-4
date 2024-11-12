using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    [SerializeField]
    private GameObject finishLevelUI;

    private List<GameObject> players = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            players.Add(other.gameObject);
            other.gameObject.GetComponent<NetworkPlayer>().enabled = false;
        }
    }

    private void Update()
    {
        if (players.Count == 4)
        {
            LevelFinished();
        }
    }

    public void LevelFinished()
    {
        finishLevelUI.SetActive(true);
    }
}
