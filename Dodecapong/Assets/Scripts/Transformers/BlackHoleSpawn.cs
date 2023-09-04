using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSpawn : Transformer
{
    public float gravityStrength;

    float destroyTime = 2.0f;
    public BlackHole blackHolePrefab;

    public override TransformerTypes GetTransformerType()
    {
        return TransformerTypes.BLACKHOLE;
    }

    public override void ApplyModifier()
    {
        if (!GameManager.instance.blackHole) {
            BlackHole spawn = Instantiate(blackHolePrefab, GameManager.instance.GetRandomTransformerSpawnPoint(), Quaternion.identity);
            GameManager.instance.blackHole = spawn;
            spawn.duration = duration;
            spawn.gravityStrength = gravityStrength;
            spawn.destroyTime = destroyTime;
        } else {
            Destroy(gameObject);
        }
    }

    protected override void RemoveModifier()
    {
        // the black hole makes the assumption that it will set the game manager
        return;
    }
}
