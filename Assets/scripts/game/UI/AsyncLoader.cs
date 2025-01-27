using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AsyncLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingScreen;

    [SerializeField] 
    private GameObject currentScreen;

    [SerializeField]
    private GameObject setupPlayersInfo;

    [SerializeField] 
    private Slider loadingSlider;

    private bool loadCheckpoint;

    public void LoadLevelButton(string levelToLoad)
    {
        if (PlayerConfigurationManager.instance != null && PlayerConfigurationManager.instance.AllPlayersReady())
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("Player"))
                {
                    obj.SetActive(true);
                    obj.gameObject.GetComponent<NetworkPlayer>().enabled = true;

                    if (!loadCheckpoint)
                    {
                        obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                        obj.transform.position = new Vector3(0, 2, 0);
                    }

                    InitializePlayerInput(obj);
                }
            }

            loadCheckpoint = false;

            currentScreen.SetActive(false);
            loadingScreen.SetActive(true);

            StartCoroutine(LoadLevelAsync(levelToLoad));
        }
        else
        {
            setupPlayersInfo.SetActive(true);
            StartCoroutine(DisableInfoAfterDelay(1.5f));
        }
        
    }

    private void InitializePlayerInput(GameObject playerObject)
    {
        var configs = PlayerConfigurationManager.instance.GetPlayerConfigurations();

        foreach (var config in configs) 
        {
            var playerController = playerObject.GetComponent<NetworkPlayer>();

            if(playerController.GetCharacterName().Equals(config.CharacterName))
            {
                var playerInput = playerObject.GetComponent<InputHandler>();
                playerInput.InitializePlayer(config);    
            }
        } 
    }

    private IEnumerator DisableInfoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (setupPlayersInfo != null)
        {
            setupPlayersInfo.SetActive(false);
        }
    }

    IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        
        while(!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }

    public void SetCheckpointToLoad(bool loadCheckpoint)
    {
        this.loadCheckpoint = loadCheckpoint;
    }
}
