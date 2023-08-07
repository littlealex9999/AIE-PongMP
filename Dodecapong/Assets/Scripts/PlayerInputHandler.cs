using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{


    PlayerInput playerInput;

    GameManager.Player mainPlayer;
    GameManager.Player secondaryPlayer;

    bool splitControls = false;

    GameManager gameManager;

    Vector2 mainPlayerMoveInput;
    Vector2 secondaryPlayerMoveInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        gameManager = GameManager.instance;
        mainPlayer = gameManager.AddPlayer();
    }

    private void Update()
    {
        if (mainPlayer != null)
        {
            if (mainPlayer.paddle) mainPlayer.paddle.Move(mainPlayerMoveInput);
        }

        if (secondaryPlayer != null)
        {
            if (secondaryPlayer.paddle) secondaryPlayer.paddle.Move(secondaryPlayerMoveInput);
        }
    }

    public void SwapActionMap(InputAction.CallbackContext callbackContext)
    {
        //if (gameManager.gameState != GameManager.GameState.JOINMENU) return;
        if (callbackContext.started)
        {

        }
        else if (callbackContext.performed)
        {

        }
        else if (callbackContext.canceled)
        {
            if (splitControls)
            {
                splitControls = false;
                playerInput.SwitchCurrentActionMap("Player");
                gameManager.RemovePlayer(secondaryPlayer);
            }
            else
            {
                splitControls = true;
                playerInput.SwitchCurrentActionMap("Split Player");
                secondaryPlayer = gameManager.AddPlayer();
            }
        }
        
    }

    public void Player1_Move(InputAction.CallbackContext callbackContext)
    {
        mainPlayerMoveInput = callbackContext.ReadValue<Vector2>();
    }

    public void Player1_Dash(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {

        }
        else if (callbackContext.performed)
        {

        }
        else if (callbackContext.canceled)
        {
          
        }
    }
    public void Player2_Move(InputAction.CallbackContext callbackContext)
    {
        secondaryPlayerMoveInput = callbackContext.ReadValue<Vector2>();
    }

    public void Player2_Dash(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {

        }
        else if (callbackContext.performed)
        {

        }
        else if (callbackContext.canceled)
        {

        }
    }

    public void StartGame(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {

        }
        else if (callbackContext.performed)
        {

        }
        else if (callbackContext.canceled)
        {
            //gameManager.UpdateGameState(GameManager.GameState.GAMEPLAY);
        }
    }
}
