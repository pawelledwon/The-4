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
    private Slider loadingSlider;

    public void LoadLevelButton(string levelToLoad)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Player"))
            {
                obj.SetActive(true);
                obj.gameObject.GetComponent<NetworkPlayer>().enabled = true;
                obj.transform.position = new Vector3(0, 2, 0);
            }
        }

        currentScreen.SetActive(false);
        loadingScreen.SetActive(true);

        StartCoroutine(LoadLevelAsync(levelToLoad));
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
}
