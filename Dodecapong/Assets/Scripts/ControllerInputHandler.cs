using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;

[RequireComponent(typeof(PlayerInput))]
public class ControllerInputHandler : MonoBehaviour
{
    PlayerInputManager playerInputManager;

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
        if (playerA.ID != 0) playerInput.actions.FindActionMap("UI").Disable();
    }

    public void LeftStick(InputAction.CallbackContext context)
    {
        playerA.movementInput = context.ReadValue<Vector2>();
    }
    public void RightStick(InputAction.CallbackContext context)
    {
        if (playerB) playerB.movementInput = context.ReadValue<Vector2>();
    }
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && instance.gameState == GameState.GAMEPLAY)
        {
            if (splitControls) playerA.Dash();
            else playerB.Dash();
        }
    }
    public void SplitDash(InputAction.CallbackContext context)
    {
        if (context.started && instance.gameState == GameState.GAMEPLAY)
        {
            if (splitControls) playerB.Dash();
        }
    }
    public void SwapControllerScheme(InputAction.CallbackContext context)
    {
        if (context.canceled && instance.gameState == GameState.JOINMENU)
        {
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
        }
    }
    public void DisconnectController(InputAction.CallbackContext context)
    {
        if (context.canceled && instance.gameState == GameState.JOINMENU)
        {
            Destroy(gameObject);
        }
    }
    public void SplitHit(InputAction.CallbackContext context)
    {
        if (context.started && instance.gameState == GameState.GAMEPLAY)
        {
            if (splitControls) playerA.Hit();
        }
    }
    public void Hit(InputAction.CallbackContext context)
    {
        if (instance.gameState == GameState.GAMEPLAY)
        {
            if (splitControls)
            {
                playerB.Hit();
            }
            else
            {
                playerA.Hit();
            }
        }
    }
    public void Grab(InputAction.CallbackContext context)
    {
        if (instance.gameState == GameState.GAMEPLAY)
        {
            if (splitControls)
            {
                playerB.Grab(context);
            }
            else
            {
                playerA.Grab(context);
            }
        }
    }
    public void SplitGrab(InputAction.CallbackContext context)
    {
        if (instance.gameState == GameState.GAMEPLAY)
        {
            if (splitControls) playerA.Grab(context);
        }
    }
}
