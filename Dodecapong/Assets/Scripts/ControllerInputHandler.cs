using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class ControllerInputHandler : MonoBehaviour
{
    public InputActionAsset UIMasterInputActionAsset;
    public InputActionAsset inputActionAsset;

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
        gameManager = GameManager.gameManagerInstance;
        mainPlayer = gameManager.AddPlayer();
        if (mainPlayer.id != 0) playerInput.actions = inputActionAsset;
    }

    private void FixedUpdate()
    {
        if (mainPlayer == null) return;
        if (mainPlayer.paddle == null) return;
        if (!mainPlayer.paddle.gameObject.activeSelf) return;
            
        mainPlayer.paddle.Move(mainPlayerMoveInput);

        if (secondaryPlayer == null) return;
        if (secondaryPlayer.paddle == null) return;
        if (!secondaryPlayer.paddle.gameObject.activeSelf) return;

        secondaryPlayer.paddle.Move(secondaryPlayerMoveInput);
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
