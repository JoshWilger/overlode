using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerWhenHit : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private Health healthScript;
    [SerializeField] private GameObject player;
    [SerializeField] private float damageAffector;

    private bool canDamage = true;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == player && canDamage)
        {
            canDamage = false;
            Invoke(nameof(DamageDelay), healthScript.damageCooldownTime);

            float healthUpgrade = atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.health];
            float coolingUpgrade = atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.cooling];
            var damage = 1f / ((healthUpgrade / 12f) + (coolingUpgrade / 10f)) * damageAffector;
            Debug.Log("Eeek! " + damage);
            healthScript.UpdateHealth(damage);
        }
    }

    private void DamageDelay()
    {
        canDamage = true;
    }
}
