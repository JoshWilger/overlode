using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemMovement : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private Health healthScript;
    [SerializeField] private float damageAffector;
    [SerializeField] private GameObject message;
    [SerializeField] private Collider2D playerColl;
    [SerializeField] private GameObject rock;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackInterval;
    [SerializeField] private float stabDelay;
    [SerializeField] private float throwDelay;
    [SerializeField] private float rockSpeed;
    [SerializeField] private float summonDelay;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private Rigidbody2D rockRb;
    private bool doingAttack;
    private bool invoked;
    private bool summoned;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        rockRb = rock.GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        doingAttack = true;
        invoked = true;
        summoned = false;
        rb.velocity = new Vector2(0, movementSpeed);
        anim.SetTrigger("throw");
        anim.speed = 0.3f;
    }

    private void OnDisable()
    {
        CancelInvoke();
        anim.SetTrigger("throw");
        anim.speed = 0;
        rb.velocity = Vector2.down;
    }

    public void EndSummoningSequence()
    {
        doingAttack = false;
        invoked = false;
    }

    private void MessageActive()
    {
        rb.velocity = Vector2.zero;
        transform.position = new Vector3(transform.position.x, -555f, transform.position.z);

        message.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (transform.position.y > -554.5f && !summoned)
        {
            summoned = true;
            rb.velocity = Vector2.down;
            anim.speed = 1;
            Invoke(nameof(MessageActive), summonDelay);
        }

        var playerX = playerColl.bounds.center.x;

        if (!invoked)
        {
            invoked = true;
            anim.SetTrigger("run");
            Invoke(nameof(Attack), attackInterval);
        }

        if (playerX > transform.position.x - 1 && playerX < transform.position.x + 1)
        {

        }
        else if (playerX > transform.position.x && !doingAttack)
        {
            sprite.flipX = true;
            rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
        }
        else if (playerX < transform.position.x && !doingAttack)
        {
            sprite.flipX = false;
            rb.velocity = new Vector2(-movementSpeed, rb.velocity.y);
        }
    }

    private void Attack()
    {
        doingAttack = true;
        rb.velocity = Vector2.zero;
        var playerX = playerColl.bounds.center.x;
        var playerY = playerColl.bounds.center.y;

        switch (Random.Range(1, 3))
        {
            case 1:
                anim.SetTrigger("stab");
                Invoke(nameof(DoDamage), stabDelay / 2f);
                Invoke(nameof(EndAttack), stabDelay);
                break;

            case 2:
                anim.SetTrigger("throw");
                rockRb.velocity = new Vector2(-(transform.position.x - playerX) * rockSpeed, -(transform.position.y + 2f - playerY) * rockSpeed);
                rock.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f);

                Invoke(nameof(EndAttack), throwDelay);
                break;

            default:
                break;
        }
    }

    private void EndAttack()
    {
        invoked = false;
        doingAttack = false;
    }

    private void DoDamage()
    {
        var diffY = playerColl.bounds.center.y - transform.position.y + 2f;
        var diffX = playerColl.bounds.center.x - transform.position.x;

        if (diffY <= 0 && (sprite.flipX ? diffX <= 4f && diffX >= 0f : diffX >= -4f && diffX <= 0f))
        {
            float healthUpgrade = atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.health];
            var damage = 1f / ((healthUpgrade / 12f)) * damageAffector;
            Debug.Log("Oh! " + damage);
            healthScript.UpdateHealth(damage);
        }
    }
}
