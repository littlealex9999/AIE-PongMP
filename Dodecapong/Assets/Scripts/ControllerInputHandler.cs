using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;

[RequireComponent(typeof(PlayerInput))]
public class ControllerInputHandler : MonoBehaviour
{
    PlayerInputManager playerInputManager;

    public InputActionAsset UIMasterInputActionAsset;

    PlayerInput playerInput;

    Player playerA;
    Player playerB;

    bool splitControls = false;

    int controllerID;
    private void OnDestroy()
    {
        gameManagerInstance.RemovePlayer(playerA);
        gameManagerInstance.RemovePlayer(playerB);
    }

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerInput = GetComponent<PlayerInput>();
        controllerID = playerInput.playerIndex;
        playerA = gameManagerInstance.GetNewPlayer();
        if (playerA.ID == 0) playerInput.actions = UIMasterInputActionAsset;
    }

    public void LeftStick(InputAction.CallbackContext context)
    {
        if (gameManagerInstance.gameState != GameState.GAMEPLAY) return;

        playerA.movementInput = context.ReadValue<Vector2>();
    }
    public void RightStick(InputAction.CallbackContext context)
    {

            if (gameManagerInstance.gameState != GameState.GAMEPLAY) return;

        if (splitControls) playerB.movementInput = context.ReadValue<Vector2>();
    }
    public void ButtonSouth(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (gameManagerInstance.gameState != GameState.GAMEPLAY) return;

            if (splitControls) playerB.Dash();
            else playerA.Dash();
        }
    }
    public void DPadDown(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (gameManagerInstance.gameState != GameState.GAMEPLAY) return;

            if (splitControls) playerA.Dash();
        }
    }
    public void SwapControllerScheme(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (gameManagerInstance.gameState != GameState.JOINMENU) return;

            if (splitControls)
            {
                gameManagerInstance.RemovePlayer(playerB);
                splitControls = false;
            }
            else
            {
                playerB = gameManagerInstance.GetNewPlayer();
                splitControls = true;
            }
            Debug.Log(playerInputManager.playerCount);
        }
    }
    public void ButtonEast(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (gameManagerInstance.gameState != GameState.JOINMENU) return;

            Debug.Log(playerInputManager.playerCount);

            Destroy(gameObject);
        }
    }
}
