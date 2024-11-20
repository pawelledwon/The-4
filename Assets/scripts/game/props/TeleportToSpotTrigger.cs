using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToSpotTrigger : MonoBehaviour
{
    [SerializeField]
    private List<Transform> spotTransform;

    private Dictionary<GameObject, bool> playersTeleported = new Dictionary<GameObject, bool>();
    private int currentSpotIndex = 0;

    private void Start()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach(var p in players)
        {
            playersTeleported.Add(p, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var player in new List<GameObject>(playersTeleported.Keys)) 
        {
            if (other.gameObject == player && !playersTeleported[player])
            {
                other.gameObject.transform.position = spotTransform[currentSpotIndex].position; 
                playersTeleported[player] = true;

                currentSpotIndex++;
            }
        }
    }

}
