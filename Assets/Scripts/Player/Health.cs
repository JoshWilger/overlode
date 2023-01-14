using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private LayerMask ground;
    [SerializeField] private Image healthBar;
    [SerializeField] private float xVelocityDamageThreshold;
    [SerializeField] private float yVelocityDamageThreshold;
    [SerializeField] private float damageCooldownTime;

    public float health;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private bool canBeDamaged;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        health = 1;
        canBeDamaged = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGrounded() && rb.velocity.y < -yVelocityDamageThreshold && canBeDamaged)
        {
            canBeDamaged = false;
            var damage = Mathf.Abs(rb.velocity.y + yVelocityDamageThreshold) / yVelocityDamageThreshold;

            Debug.Log(damage);
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
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = health;
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, ground);
    }

    /*    private void OnCollisionEnter2D(Collision2D collision)
        {
            ApplyCollisionDamage(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            ApplyCollisionDamage(collision);
        }

        private void ApplyCollisionDamage(Collision2D collision)
        {
            if (collision.relativeVelocity.x >= xVelocityDamageThreshold || collision.relativeVelocity.y >= yVelocityDamageThreshold)
            {
                Debug.Log(collision.relativeVelocity);
                UpdateHealth(Mathf.Max(collision.relativeVelocity.x, collision.relativeVelocity.y) / 100f);
            }
        }*/
}
