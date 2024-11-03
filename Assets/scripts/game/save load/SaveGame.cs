using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerFileMapping
{
    public NetworkPlayer player;
    public string fileName;
}

public class SaveGame : MonoBehaviour
{
    [SerializeField]
    private List<PlayerFileMapping> playerFileMappings;

    [SerializeField]
    private AsyncLoader asyncLoader;

    public void SaveGameState()
    {
        foreach (var mapping in playerFileMappings)
        {
            if (mapping.player != null && !string.IsNullOrEmpty(mapping.fileName))
            {
                mapping.player.SavePlayer(mapping.fileName);
            }
        }
    }

    public void LoadGameState()
    {
        string levelName = "";
        foreach (var mapping in playerFileMappings)
        {
            if (mapping.player != null && !string.IsNullOrEmpty(mapping.fileName))
            {
                var data = mapping.player.LoadPlayer(mapping.fileName);
                levelName = data.levelName;
            }
        }

        if (asyncLoader != null)
        {
            asyncLoader.LoadLevelButton(levelName);
        }
        else
        {
            Debug.LogError("AsyncLoader reference is missing in SaveGame.");
        }
    }
}
