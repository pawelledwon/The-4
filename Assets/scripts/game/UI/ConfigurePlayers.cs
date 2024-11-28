using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConfigurePlayers : MonoBehaviour
{
    [SerializeField]
    private GameObject setupPlayer;

    [SerializeField]
    private GameObject gameMenu;

    [SerializeField]
    private Button setupPlayersButton;

    private bool allPlayersReady = false;

    private void Update()
    {
        if (PlayerConfigurationManager.instance != null)
        {
            bool currentReadyState = PlayerConfigurationManager.instance.AllPlayersReady();

            if (currentReadyState != allPlayersReady)
            {
                allPlayersReady = currentReadyState;

                if (allPlayersReady)
                {
                    DisableSetupMenu();
                }
                else
                {
                    EnableSetupPlayerMenu();
                }
            }
        }
    }

    private void EnableSetupPlayerMenu()
    {
        setupPlayer.SetActive(true);
        gameMenu.SetActive(false);
    }

    private void DisableSetupMenu()
    {
        gameMenu.SetActive(true);
        setupPlayersButton.interactable = false;

        EventSystem eventSystem = EventSystem.current;
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(gameMenu.GetComponentInChildren<Button>()?.gameObject);
        }
    }
}
