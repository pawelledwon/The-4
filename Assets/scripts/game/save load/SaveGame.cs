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
    private AsyncLoader asyncLoader;

    [SerializeField]
    private GameObject noSavedDataInfo;

    private List<PlayerFileMapping> playerFileMappings = new List<PlayerFileMapping>();

    void LoadPlayerList()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        List<GameObject> playerObjects = new List<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Player"))
            {
                playerObjects.Add(obj);
            }
        }

        foreach (var playerObj in playerObjects)
        {
            NetworkPlayer networkPlayer = playerObj.GetComponent<NetworkPlayer>();

            if (networkPlayer != null)
            {
                PlayerFileMapping mapping = new PlayerFileMapping
                {
                    player = networkPlayer,
                    fileName = networkPlayer.name
                };

                playerFileMappings.Add(mapping);
            }
            else
            {
                Debug.LogWarning("GameObject tagged as 'Player' does not have a NetworkPlayer component.");
            }
        }
    }

    public void SaveGameState()
    {
        if(playerFileMappings.Count == 0) 
        {
            LoadPlayerList();
        }

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
        if (playerFileMappings.Count == 0)
        {
            LoadPlayerList();
        }

        string levelName = "";
        foreach (var mapping in playerFileMappings)
        {
            if (mapping.player != null && !string.IsNullOrEmpty(mapping.fileName))
            {
                var data = mapping.player.LoadPlayer(mapping.fileName);

                if(data == null)
                {
                    noSavedDataInfo.SetActive(true);
                    StartCoroutine(NoDataSavedInfoShow(1.5f));
                    return;
                }

                levelName = data.levelName;
            }
        }

        if (asyncLoader != null)
        {
            asyncLoader.LoadLevelButton(levelName, loadCharacterPosition: false);
        }
        else
        {
            Debug.LogError("AsyncLoader reference is missing in SaveGame.");
        }
    }

    private IEnumerator NoDataSavedInfoShow(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (noSavedDataInfo != null)
        {
            noSavedDataInfo.SetActive(false);
        }
    }
}
