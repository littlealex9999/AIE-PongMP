using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class ControllerInputHandler : MonoBehaviour
{
    [HideInInspector] public Player playerA;
    [HideInInspector] public Player playerB;

    [HideInInspector] public bool splitControls = false;

    private void OnDestroy()
    {
        GameManager.instance.controllers.Remove(this);
        GameManager.instance.RemovePlayer(playerA);
        GameManager.instance.RemovePlayer(playerB);
        GameManager.instance.UpdatePlayerImages();
    }

    private void Awake()
    {
        GameManager.instance.controllers.Add(this);
        playerA = GameManager.instance.GetNewPlayer();
        GameManager.instance.UpdatePlayerImages();
    }

    public void PlayerInput_onDeviceLost(PlayerInput obj)
    {
        //Destroy(gameObject);
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
            if (splitControls)
            {
                playerB.Dash();
            }
            else
            {
                playerA.Dash();
            }
        }
    }
    public void SplitDash(InputAction.CallbackContext context)
    {
        if (context.started && GameManager.instance.gameState == GameManager.GameState.GAMEPLAY)
        {
            if (splitControls) playerA.Dash();
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
            Destroy(gameObject);
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

    public void PageRight(InputAction.CallbackContext context)
    {
        if (context.performed && GameManager.instance.gameState == GameManager.GameState.SETTINGSMENU)
        {
            MenuManager.instance.SettingsScreenPageRight();
        }
    }
    public void PageLeft(InputAction.CallbackContext context)
    {
        if (context.performed && GameManager.instance.gameState == GameManager.GameState.SETTINGSMENU)
        {
            MenuManager.instance.SettingsScreenPageLeft();
        }
    }
}
