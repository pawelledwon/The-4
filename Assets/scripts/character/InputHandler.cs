using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InputHandler : MonoBehaviour
{
    private NetworkPlayer playerController;
    private PlayerConfiguration playerConfig;
    private PlayerControls playerControls;
    private PickUpObject pickUpObject;
    private ThrowItem throwItem;
    private PauseMenu pauseMenu;

    void Awake()
    {
        playerController = GetComponent<NetworkPlayer>();
        pickUpObject = GetComponentInChildren<PickUpObject>();
        throwItem = GetComponentInChildren<ThrowItem>();
        playerControls = new PlayerControls();
    }

    public void InitializePlayer(PlayerConfiguration pc)
    {
        playerConfig = pc;
        playerConfig.playerInput.onActionTriggered += OnActionTriggered;
    }
    
    private void OnActionTriggered(CallbackContext obj)
    {
        if (obj.action.name == playerControls.PlayerMovement.Movement.name) 
        {
            OnMove(obj);
        }

        if (obj.action.name == playerControls.PlayerMovement.SelectJump.name)
        {
            OnJump(obj);
        }

        if (obj.action.name == playerControls.PlayerMovement.PickUp.name)
        {
            OnPickUp(obj);
        }

        if (obj.action.name == playerControls.PlayerMovement.Throw.name)
        {
            OnThrow(obj);
        }

        if (obj.action.name == playerControls.PlayerMovement.Pause.name)
        {
            OnPause(obj);
        }
    }

    private void OnMove(CallbackContext context)
    {
        if (playerController != null && context.performed)
        {
            print("Move");
            playerController.SetMoveInput(context.ReadValue<Vector2>());
        }
    }

    private void OnJump(CallbackContext context)
    {
        if (playerController != null && context.performed)
        {
            print("Jump");
            print(playerController.GetCharacterName());
            playerController.DetectDoubleSpacePress();
        }
    }

    private void OnPickUp(CallbackContext context)
    {
        if (pickUpObject != null && context.performed) 
        {
            print("PickUp");
            pickUpObject.PickUpItem();
        }
        
    }

    private void OnThrow(CallbackContext context)
    {
        if (throwItem != null && context.performed)
        {
            print("Throw");
            throwItem.ThrowItemAway();
        }
    }

    private void OnPause(CallbackContext context)
    {
        pauseMenu = FindAnyObjectByType<PauseMenu>();

        if (pauseMenu != null && context.performed)
        {
            print("Pause");
            pauseMenu.HandlePauseMenu();
        }
    }

}
