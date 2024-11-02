using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public List<NetworkPlayer> players = new List<NetworkPlayer>();
    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    private int currentCheckpointIndex = -1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
