using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpeed : Transformer
{
    public float speedMod = 1.0f;

    public override TransformerTypes GetTransformerType()
    {
        return TransformerTypes.BALLSPEED;
    }

    public override void ApplyModifier()
    {
        for (int i = 0; i < GameManager.instance.balls.Count; i++) {
            GameManager.instance.balls[i].constantVel += speedMod;
        }
    }

    protected override void RemoveModifier()
    {
        for (int i = 0; i < GameManager.instance.balls.Count; i++) {
            GameManager.instance.balls[i].constantVel -= speedMod;
        }
    }
}
