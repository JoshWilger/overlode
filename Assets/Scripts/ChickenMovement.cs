using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenMovement : MonoBehaviour
{
    [SerializeField] private Collider2D playerColl;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackInterval;
    [SerializeField] private float peckDelay;
    [SerializeField] private float laserDelay;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private bool doingAttack;
    private bool invoked;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        doingAttack = false;
        invoked = false;
    }

    private void FixedUpdate()
    {
        var playerX = playerColl.bounds.center.x;
        var playerY = playerColl.bounds.center.y;

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

        switch (Random.Range(1, 3))
        {
            case 1:
                anim.SetTrigger("peck");
                Invoke(nameof(Peck), peckDelay);
                break;

            case 2:
                anim.SetTrigger("laser");
                Invoke(nameof(Laser), laserDelay);
                break;

            default:
                break;
        }
    }

    private void Peck()
    {


        invoked = false;
        doingAttack = false;
    }

    private void Laser()
    {


        invoked = false;
        doingAttack = false;
    }
}
