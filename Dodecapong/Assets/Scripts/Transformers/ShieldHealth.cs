using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHealth : Transformer
{
    public int health = 1;

    public override TransformerTypes GetTransformerType()
    {
        return TransformerTypes.SHIELDHEALTH;
    }

    public override void ApplyModifier()
    {
        for (int i = 0; i < GameManager.instance.alivePlayers.Count; i++) {
            GameManager.instance.alivePlayers[i].shieldHealth += health;
        }
    }

    protected override void RemoveModifier()
    {
        for (int i = 0; i < GameManager.instance.alivePlayers.Count; i++) {
            GameManager.instance.alivePlayers[i].shieldHealth -= health;
        }
    }
}
