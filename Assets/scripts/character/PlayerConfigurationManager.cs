using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour
{
    [SerializeField]
    private int maxPlayers = 4;
    [SerializeField]
    private ConfigurePlayers configurePlayersMenu;


    private List<PlayerConfiguration> playerConfigurations;
    private bool allPlayersReady = false;

    public static PlayerConfigurationManager instance { get; private set; }

    private void Awake()
    {
       if (instance != null)
       {
            Debug.Log("Singleton - trying to create another instance - PlayerConfigurationManager");
       }
       else
       {
            instance = this;
            playerConfigurations = new List<PlayerConfiguration>();
       }
    }

    public void ReadyPlayer(int index)
    {
        playerConfigurations[index].isReady = true;
        if (playerConfigurations.Count == maxPlayers && playerConfigurations.All(p => p.isReady)) 
        {
            allPlayersReady = true;
        }
    }

    public void SetPlayer(int playerIndex, string characterName)
    {
        playerConfigurations[playerIndex].CharacterName = characterName;
    }

    public bool AllPlayersReady()
    {
        return this.allPlayersReady;
    }

    public List<PlayerConfiguration> GetPlayerConfigurations()
    {
        return this.playerConfigurations;
    }

    public void HandlePlayerJoin(PlayerInput playerInput)
    {
        Debug.Log($"Player joined {playerInput.playerIndex}");
        if (!playerConfigurations.Any(p => p.playerIndex == playerInput.playerIndex)) 
        {
            playerInput.transform.SetParent(transform);
            playerConfigurations.Add(new PlayerConfiguration(playerInput));
        }
    }

}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput playerInput)
    {
        playerIndex = playerInput.playerIndex;
        this.playerInput = playerInput;
        CharacterName = "";
    }
    public PlayerInput playerInput { get; set; }
    public int playerIndex { get; set; }
    public bool isReady { get; set; }
    public string CharacterName {get; set;}
}