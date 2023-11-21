using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSpawn : Transformer
{
    public float gravityStrength;
    public float spawnAreaMultiplier = 0.5f;

    float destroyTime = 2.0f;
    public GameObject blackHolePrefab;
    BlackHole blackHole;

    public override TransformerTypes GetTransformerType()
    {
        return TransformerTypes.BLACKHOLE;
    }

    public override void ApplyModifier()
    {
        if (!GameManager.instance.blackHole) {
            blackHole = Instantiate(blackHolePrefab, GameManager.instance.GetTransformerSpawnPoint() * spawnAreaMultiplier, Quaternion.identity).GetComponentInChildren<BlackHole>(true);
            GameManager.instance.blackHole = blackHole;
            blackHole.duration = duration;
            blackHole.gravityStrength = gravityStrength;
            blackHole.destroyTime = destroyTime;
        }

        GameManager.instance.spawnedTransformers.Remove(this);
        GameManager.instance.activeTransformers.Remove(this);
        Destroy(gameObject);
    }

    protected override void RemoveModifier()
    {
        // the black hole makes the assumption that it will set the game manager
        return;
    }
}
