using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class ControllerInputHandler : MonoBehaviour
{
    [HideInInspector] public Player playerA;
    [HideInInspector] public Player playerB;

    [HideInInspector] public bool splitControls = false;

    public PlayerInput playerInput { get; private set; }
    public Gamepad gamepad { get; private set; }
    bool hapticsRunning = false;

    private void OnDestroy()
    {
        if (gamepad != null) gamepad.ResetHaptics();
    }

    private void Awake()
    {
        GameManager.instance.controllers.Add(this);

        playerInput = GetComponent<PlayerInput>();
        if (playerInput)
        {
            for (int i = 0; i < playerInput.devices.Count; i++)
            {
                if (playerInput.devices[i] is Gamepad)
                {
                    gamepad = (Gamepad)playerInput.devices[i];
                    break;
                }
            }
        }

        JoinPlayer(out playerA);
    }

    public void PlayerInput_onDeviceLost(PlayerInput obj)
    {
        //Destroy(gameObject);
    }

    public void LeftStick(InputAction.CallbackContext context)
    {
        if (playerA.dead && playerB != null) playerB.movementInput = context.ReadValue<Vector2>();
        else playerA.movementInput = context.ReadValue<Vector2>();
    }
    public void RightStick(InputAction.CallbackContext context)
    {
        if (playerB == null) playerA.movementInput = context.ReadValue<Vector2>();
        else if (playerB.dead) playerA.movementInput = context.ReadValue<Vector2>();
        else playerB.movementInput = context.ReadValue<Vector2>();
    }
    public void RightDash(InputAction.CallbackContext context)
    {
        if (context.started && GameManager.instance.gameState == GameManager.GameState.GAMEPLAY && !GameManager.instance.holdGameplay)
        {
            if (splitControls)
            {
                if (playerB.dead) playerA.Dash();
                else playerB.Dash();
            }
            else
            {
                playerA.Dash();
            }
        }
    }
    public void LeftDash(InputAction.CallbackContext context)
    {
        if (context.started && GameManager.instance.gameState == GameManager.GameState.GAMEPLAY && !GameManager.instance.holdGameplay)
        {
            if (splitControls)
            {
                if (playerA.dead) playerB.Dash();
                else playerA.Dash();
            }
            else
            {
                playerA.Dash();
            }
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
                JoinPlayer(out playerB);
            }
        }
    }
    public void DisconnectController(InputAction.CallbackContext context)
    {
        if (context.canceled && GameManager.instance.gameState == GameManager.GameState.JOINMENU)
        {
            GameManager.instance.controllers.Remove(this);
            GameManager.instance.RemovePlayer(playerA);
            GameManager.instance.RemovePlayer(playerB);
            GameManager.instance.UpdatePlayerImages();

            Destroy(gameObject);
        }
    }

    public void SplitHit(InputAction.CallbackContext context)
    {
        if (context.started && GameManager.instance.gameState == GameManager.GameState.GAMEPLAY && !GameManager.instance.holdGameplay)
        {
            if (splitControls) playerA.Hit();
        }
    }
    public void Hit(InputAction.CallbackContext context)
    {
        if (GameManager.instance.gameState == GameManager.GameState.GAMEPLAY && !GameManager.instance.holdGameplay)
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
    public void RightGrab(InputAction.CallbackContext context)
    {
        if (GameManager.instance.gameState == GameManager.GameState.GAMEPLAY && !GameManager.instance.holdGameplay)
        {
            if (splitControls)
            {
                if (playerB.dead) playerA.Grab(context);
                else playerB.Grab(context);
            }
            else
            {
                playerA.Grab(context);
            }
        }
    }
    public void LeftGrab(InputAction.CallbackContext context)
    {
        if (GameManager.instance.gameState == GameManager.GameState.GAMEPLAY && !GameManager.instance.holdGameplay )
        {
            if (splitControls)
            {
                if (playerA.dead) playerB.Grab(context);
                else playerA.Grab(context);
            }
            else
            {
                playerA.Grab(context);
            }
        }
    }

    public void PageRight(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (GameManager.instance.gameState == GameManager.GameState.PRESETSELECT ||
            GameManager.instance.gameState == GameManager.GameState.EDITPRESET)
            {
                EventManager.instance.scrollPageEvent.Invoke();
                MenuManager.instance.PageRight();
            }
        }
    }
    public void PageLeft(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (GameManager.instance.gameState == GameManager.GameState.PRESETSELECT ||
            GameManager.instance.gameState == GameManager.GameState.EDITPRESET)
            {
                EventManager.instance.scrollPageEvent.Invoke();
                MenuManager.instance.PageLeft();
            }
        }
    }

    public IEnumerator SetHaptics(float lowFrequency, float highFrequency, float duration)
    {
        if (gamepad != null && !hapticsRunning && GameManager.instance.enableHaptics)
        {
            hapticsRunning = true;
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            gamepad.ResumeHaptics();
        }
        else
        {
            yield return null;
        }

        while (duration > 0.0f)
        {
            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        gamepad.ResetHaptics();
        hapticsRunning = false;

        yield return null;
    }

    public void SetHaptics(ControllerHaptics haptics, bool resetHaptics = true)
    {
        if (resetHaptics)
        {
            gamepad.ResetHaptics();
            hapticsRunning = false;
            StopAllCoroutines();
        }
        StartCoroutine(SetHaptics(haptics.lowFrequencyIntensity, haptics.highFrequencyIntensity, haptics.duration));
    }

    void JoinPlayer(out Player player)
    {
        player = GameManager.instance.GetNewPlayer();
        GameManager.instance.UpdatePlayerImages();
        player.controllerHandler = this;

        SetHaptics(GameManager.instance.playerJoinHaptics);
    }
}
