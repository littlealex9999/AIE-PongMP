using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongRectangleCollider : PongCollider
{
    public override CollisionSystem.ColliderTypes Type() => CollisionSystem.ColliderTypes.RECTANGLE;

    public Vector2 size = Vector2.one;
}
