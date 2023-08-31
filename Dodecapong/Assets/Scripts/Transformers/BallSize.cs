using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSize : Transformer
{
    public float sizeMod = 1.0f;

    public override TransformerTypes GetTransformerType()
    {
        return TransformerTypes.BALLSIZE;
    }

    public override void ApplyModifier()
    {
        GameManager.instance.ballPrefab.radius += sizeMod;
    }

    protected override void RemoveModifier()
    {
        GameManager.instance.ballPrefab.radius -= sizeMod;
    }
}
