using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongClippedPlaneCollider : PongCollider
{
    public float limit;
    public Vector2 offset;

    public override CollisionSystem.ColliderTypes Type() { return CollisionSystem.ColliderTypes.CLIPPEDPLANE; }
}
