using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    private List<NetworkPlayer> players = new List<NetworkPlayer>();
    private int currentCheckpointIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();

            foreach (NetworkPlayer player in players)
            {
                this.players.Add(player);
            }
        }
        else
            Destroy(gameObject);
    }

    public int GetCurrentCheckPointIndex()
    {
        return currentCheckpointIndex;
    }

    public void PlayerReachedCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex > currentCheckpointIndex)
        {
            currentCheckpointIndex = checkpointIndex;
            UpdateAllPlayersCheckpoint(currentCheckpointIndex);
        }
    }

    private void UpdateAllPlayersCheckpoint(int checkpointIndex)
    {
        foreach (var player in players)
        {
            player.UpdateCheckpoint(checkpoints[checkpointIndex].transform.position);
        }
    }
}
