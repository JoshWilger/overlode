using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Image healthBar;
    [SerializeField] private float xVelocityDamageThreshold;
    [SerializeField] private float yVelocityDamageThreshold;
    [SerializeField] private float damageCooldownTime;
    [SerializeField] private Animator hurt;

    public float health;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private bool canBeDamaged;
    private bool hasHitCeil;
    private bool hasHitWall;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        health = 1;
        canBeDamaged = true;
        hasHitCeil = false;
        hasHitWall = false;

    }

    // Update is called once per frame
    void Update()
    {
        float upgrade = atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.health];
        if (!IsWall(Vector2.up))
        {
            hasHitCeil = false;
        }
        if (!(IsWall(Vector2.left) || IsWall(Vector2.right)))
        {
            hasHitWall = false;
        }
        if (IsWall(Vector2.up) && Mathf.Abs(rb.velocity.y) > yVelocityDamageThreshold && canBeDamaged && !hasHitCeil)
        {
            canBeDamaged = false;
            hasHitCeil = true;
            var damage = Mathf.Abs(rb.velocity.y - xVelocityDamageThreshold) / (xVelocityDamageThreshold * (upgrade / 20f));

            Debug.Log("Ow! " + damage);
            UpdateHealth(damage);
            Invoke(nameof(DamageCooldown), damageCooldownTime);
        }
        else if (IsWall(Vector2.down) && rb.velocity.y < -yVelocityDamageThreshold && canBeDamaged)
        {
            canBeDamaged = false;
            var damage = Mathf.Abs(rb.velocity.y + yVelocityDamageThreshold) / (yVelocityDamageThreshold * (upgrade / 20f));

            Debug.Log("Ahh! " + damage);
            UpdateHealth(damage);
            Invoke(nameof(DamageCooldown), damageCooldownTime);
        }

        else if ((IsWall(Vector2.left) || IsWall(Vector2.right)) && Mathf.Abs(rb.velocity.x) > xVelocityDamageThreshold && canBeDamaged && !hasHitWall)
        {
            canBeDamaged = false;
            hasHitWall = true;
            var damage = (Mathf.Abs(rb.velocity.x) - xVelocityDamageThreshold) / (xVelocityDamageThreshold * (upgrade / 20f));

            Debug.Log("Oof! " + damage);
            UpdateHealth(damage);
            Invoke(nameof(DamageCooldown), damageCooldownTime);
        }
    }

    private void DamageCooldown()
    {
        canBeDamaged = true;
    }

    public void UpdateHealth(float removalAmount)
    {
        health -= removalAmount;
        UpdateHealthBar();
        hurt.SetTrigger("hurt");
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = health;
    }

    private bool IsWall(Vector2 direction)
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, direction, 0.1f, ground);
    }
}
