using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour
{
    public HitType hitType;
    public EnemyController enemy;

    public bool CallForHit()
    {
        return enemy.TriggerHit(hitType);
    }
}
