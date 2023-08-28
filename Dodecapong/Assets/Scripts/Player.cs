using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Paddle paddle;
    [HideInInspector] public int ID { get { return GameManager.instance.players.IndexOf(this); } private set { } }

    [HideInInspector] public Vector2 movementInput;

    [HideInInspector] public int shieldHealth;

    [HideInInspector] public Color color;

    [HideInInspector] public float dashDuration;
    [HideInInspector] public float dashCooldown;
    float dashCooldownProgress;
    bool readyToDash = true;

    [HideInInspector] public float hitDuration;
    [HideInInspector] public float hitCooldown;
    float hitCooldownProgress;
    bool readyToHit = true;

    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    public void Dash()
    {
        if (!readyToDash) return;

        readyToDash = false;
        dashCooldownProgress = dashCooldown + dashDuration;
        StartCoroutine(paddle.Dash(movementInput.normalized, dashDuration));
    }

    public void Hit()
    {
        if (!readyToHit) return;

        readyToHit = false;
        hitCooldownProgress = hitCooldown + hitDuration;
        StartCoroutine(paddle.Hit(hitDuration));
    }

    void FixedUpdate()
    {
        // animation value: progress / (dashCooldown + dashDuration)

        readyToDash = UpdateCooldown(ref dashCooldownProgress);
        readyToHit = UpdateCooldown(ref hitCooldownProgress);

        if (!paddle.dashing && !GameManager.instance.holdGameplay) paddle.Move(movementInput);
    }

    private bool UpdateCooldown(ref float progress)
    {
        if (progress > 0)
        {
            progress -= Time.fixedDeltaTime;
        }
        else
        {
            return true;
        }
        return false;
    }
}
