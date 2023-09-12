using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class ControllerInputHandler : MonoBehaviour
{
    PlayerInputManager playerInputManager;

    PlayerInput playerInput;

    [HideInInspector] public Player playerA;
    [HideInInspector] public Player playerB;

    [HideInInspector] public bool splitControls = false;

    private void OnDestroy()
    {
        GameManager.instance.RemovePlayer(playerA);
        GameManager.instance.RemovePlayer(playerB);
    }

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerInput = GetComponent<PlayerInput>();
        GameManager.instance.controllers.Add(this);
        playerA = GameManager.instance.GetNewPlayer();
        GameManager.instance.UpdatePlayerImages();
        if (playerA.ID != 0) playerInput.actions.FindActionMap("UI").Disable();
    }

    public void LeftStick(InputAction.CallbackContext context)
    {
        if (playerA) playerA.movementInput = context.ReadValue<Vector2>();
    }
    public void RightStick(InputAction.CallbackContext context)
    {
        if (playerB) playerB.movementInput = context.ReadValue<Vector2>();
    }
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && GameManager.instance.gameState == GameManager.GameState.GAMEPLAY)
        {
            if (splitControls) playerA.Dash();
            else playerB.Dash();
        }
    }
    public void SplitDash(InputAction.CallbackContext context)
    {
        if (context.started && GameManager.instance.gameState == GameManager.GameState.GAMEPLAY)
        {
            if (splitControls) playerB.Dash();
        }
    }
    public void SwapControllerScheme(InputAction.CallbackContext context)
    {
        if (context.canceled && GameManager.instance.gameState == GameManager.GameState.JOINMENU)
        {
            if (splitControls)
            {
                splitControls = false;
                GameManager.instance.RemovePlayer(playerB);
            }
            else
            {
                splitControls = true;
                playerB = GameManager.instance.GetNewPlayer();
                GameManager.instance.UpdatePlayerImages();
            }
        }
    }
    public void DisconnectController(InputAction.CallbackContext context)
    {
        if (context.canceled && GameManager.instance.gameState == GameManager.GameState.JOINMENU)
        {
            GameManager.instance.controllers.Remove(this);
            Destroy(gameObject);
            GameManager.instance.UpdatePlayerImages();
        }
    }
    public void SplitHit(InputAction.CallbackContext context)
    {
        if (context.started && GameManager.instance.gameState == GameManager.GameState.GAMEPLAY)
        {
            if (splitControls) playerA.Hit();
        }
    }
    public void Hit(InputAction.CallbackContext context)
    {
        if (GameManager.instance.gameState == GameManager.GameState.GAMEPLAY)
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
        if (GameManager.instance.gameState == GameManager.GameState.GAMEPLAY)
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
        if (GameManager.instance.gameState == GameManager.GameState.GAMEPLAY)
        {
            if (splitControls) playerA.Grab(context);
        }
    }
}
