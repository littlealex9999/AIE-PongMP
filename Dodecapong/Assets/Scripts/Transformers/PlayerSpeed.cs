using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeed : Transformer
{
    public float speedMod = 30.0f;

    public override void ApplyModifier()
    {
        for (int i = 0; i < GameManager.instance.alivePlayers.Count; i++) {
            GameManager.instance.alivePlayers[i].moveSpeed += speedMod;
        }
    }
}
