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
        GameManager.instance.ballPrefab.constantVel += speedMod;
    }

    protected override void RemoveModifier()
    {
        GameManager.instance.ballPrefab.constantVel -= speedMod;
    }
}
