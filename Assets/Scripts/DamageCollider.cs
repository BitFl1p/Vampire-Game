using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DamageCollider : MonoBehaviour
{
    public int damage, knockback;
    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            health.Damage(damage, GetComponent<Collider>(), knockback);
        }
    }

}