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

    private void OnDestroy()
    {
        instance.RemovePlayer(playerA);
        instance.RemovePlayer(playerB);
    }

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerInput = GetComponent<PlayerInput>();
        playerA = instance.GetNewPlayer();
        if (playerA.ID == 0) playerInput.actions = UIMasterInputActionAsset;
    }

    public void LeftStick(InputAction.CallbackContext context)
    {
        if (instance.gameState != GameState.GAMEPLAY) return;

        playerA.movementInput = context.ReadValue<Vector2>();
    }
    public void RightStick(InputAction.CallbackContext context)
    {

            if (instance.gameState != GameState.GAMEPLAY) return;

        if (splitControls) playerB.movementInput = context.ReadValue<Vector2>();
    }
    public void ButtonSouth(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (instance.gameState != GameState.GAMEPLAY) return;

            if (splitControls) playerB.Dash();
            else playerA.Dash();
        }
    }
    public void DPadDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (instance.gameState != GameState.GAMEPLAY) return;

            if (splitControls) playerA.Dash();
        }
    }
    public void SwapControllerScheme(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (instance.gameState != GameState.JOINMENU) return;

            if (splitControls)
            {
                instance.RemovePlayer(playerB);
                splitControls = false;
            }
            else
            {
                playerB = instance.GetNewPlayer();
                splitControls = true;
            }
            Debug.Log(playerInputManager.playerCount);
        }
    }
    public void ButtonEast(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (instance.gameState != GameState.JOINMENU) return;

            Debug.Log(playerInputManager.playerCount);

            Destroy(gameObject);
        }
    }
}
