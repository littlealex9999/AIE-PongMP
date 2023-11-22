using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCooldown : Transformer
{
    public float cooldownMod = 0.1f;

    public override TransformerTypes GetTransformerType()
    {
        //return TransformerTypes.DASHCOOLDOWN;
        return 0;
    }

    public override void ApplyModifier()
    {
        for (int i = 0; i < GameManager.instance.alivePlayers.Count; i++) {
            GameManager.instance.alivePlayers[i].dashCooldown += cooldownMod;
        }
    }

    protected override void RemoveModifier()
    {
        for (int i = 0; i < GameManager.instance.alivePlayers.Count; i++) {
            GameManager.instance.alivePlayers[i].dashCooldown -= cooldownMod;
        }
    }
}
