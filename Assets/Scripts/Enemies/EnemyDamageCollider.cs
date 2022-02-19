using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageCollider : DamageCollider
{
    protected override void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerAttacks player))
        {
            player.PlayerAttacked(this);
        }
        if (collision.gameObject.TryGetComponent(out Destructible destructible))
        {
            destructible.killed = true;
        }
    }
}
