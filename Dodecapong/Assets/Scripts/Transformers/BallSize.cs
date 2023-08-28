using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSize : Transformer
{
    public float sizeMod = 1.0f;

    public override void ApplyModifier()
    {
        GameManager.instance.ball.radius += sizeMod;
    }

    protected override void RemoveModifier()
    {
        GameManager.instance.ball.constantVel -= sizeMod;
    }
}
