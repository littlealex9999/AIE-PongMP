using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSpawn : Transformer
{
    public float gravityStrength;

    float destroyTime = 2.0f;
    public BlackHole blackHolePrefab;

    public override void ApplyModifier()
    {
        if (!GameManager.instance.blackHoleActive) {
            GameManager.instance.blackHoleActive = true;
            BlackHole spawn = Instantiate(blackHolePrefab, GameManager.instance.GetRandomTransformerSpawnPoint(), Quaternion.identity);
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
