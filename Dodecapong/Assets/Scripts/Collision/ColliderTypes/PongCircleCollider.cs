using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PongCircleCollider : PongCollider
{
    public override CollisionSystem.ColliderTypes Type() => CollisionSystem.ColliderTypes.CIRCLE;

    public float radius = 1;
}
