using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurePlayers : MonoBehaviour
{
    [SerializeField]
    private GameObject setupPlayer;

    [SerializeField]
    private GameObject gameMenu;

    [SerializeField]
    private Button setupPlayersButton;

    public void EnableSetupPlayerMenu()
    {
        setupPlayer.SetActive(true);
        gameMenu.SetActive(false);
    }

    public void DisableSetupMenu()
    {
        gameMenu.SetActive(true);
        setupPlayersButton.interactable = false;
    }
}
