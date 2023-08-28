using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpeed : Transformer
{
    public float speedMod = 1.0f;

    public override void ApplyModifier()
    {
        GameManager.instance.ball.constantVel += speedMod;
    }
}
