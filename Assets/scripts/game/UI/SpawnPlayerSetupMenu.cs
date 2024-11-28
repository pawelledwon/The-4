using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SpawnPlayerSetupMenu : MonoBehaviour
{
    public GameObject playerSetupMenuPrefab;
    public PlayerInput input;

    private GameObject setupPlayerMenu;
    private void Awake()
    {
        var rootMenu = GameObject.Find("Canvas");
        if(rootMenu != null)
        {
            setupPlayerMenu = Instantiate(playerSetupMenuPrefab, rootMenu.transform);
            input.uiInputModule = setupPlayerMenu.GetComponentInChildren<InputSystemUIInputModule>();
            setupPlayerMenu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(input.playerIndex);
        }
    }

    private void Update()
    {
        if (PlayerConfigurationManager.instance.AllPlayersReady() && setupPlayerMenu != null)
        {
            setupPlayerMenu.SetActive(false);
        }
    }
}
