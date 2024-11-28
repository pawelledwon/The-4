using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject title;
    [SerializeField]
    private GameObject readyPanel;
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private Button readyButton;

    private int playerIndex;
    private float ignoreInputTime = 1.5f;
    private bool inputEnabled = false;
    
    public void SetPlayerIndex(int playerIndex)
    {
        this.playerIndex = playerIndex;
        ignoreInputTime = Time.time + ignoreInputTime;

        var titleText = title.GetComponent<TextMeshProUGUI>();
        if (titleText != null)
        {
            titleText.SetText($"Player {playerIndex + 1}");
        }
    }

    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    public void SetPlayer(string characterName)
    {
        if (!inputEnabled) 
        {
            return;
        }

        AssignCharacterToPlayer(characterName);

        readyPanel.SetActive(true);
        readyButton.Select();
        menuPanel.SetActive(false);
    }

    private void AssignCharacterToPlayer(string characterName)
    {
        PlayerConfigurationManager.instance.SetPlayer(playerIndex, characterName);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled)
        {
            return;
        }

        PlayerConfigurationManager.instance.ReadyPlayer(playerIndex);
        readyButton.gameObject.SetActive(false);
    }
}
