using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSize : Transformer
{
    public float sizeMod = 1.0f;

    public override TransformerTypes GetTransformerType()
    {
        if (sizeMod > 0) {
            return TransformerTypes.BALLSIZEUP;
        } else {
            return TransformerTypes.BALLSIZEDOWN;
        }
    }

    public override void ApplyModifier()
    {
        for (int i = 0; i < GameManager.instance.balls.Count; i++) {
            GameManager.instance.balls[i].radius += sizeMod;
        }
    }

    protected override void RemoveModifier()
    {
        for (int i = 0; i < GameManager.instance.balls.Count; i++) {
            GameManager.instance.balls[i].radius -= sizeMod;
        }
    }
}
