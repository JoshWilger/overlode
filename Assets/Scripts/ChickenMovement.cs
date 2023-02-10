using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenMovement : MonoBehaviour
{
    [SerializeField] private Collider2D playerColl;
    [SerializeField] private GameObject laser;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackInterval;
    [SerializeField] private float peckDelay;
    [SerializeField] private float laserDelay;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private HingeJoint2D laserHinge;
    private SpriteRenderer laserSprite;
    private bool doingAttack;
    private bool invoked;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        laserHinge = laser.GetComponent<HingeJoint2D>();
        laserSprite = laser.GetComponent<SpriteRenderer>();
        doingAttack = false;
        invoked = false;
    }

    private void FixedUpdate()
    {
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

        switch (Random.Range(2, 3))
        {
            case 1:
                anim.SetTrigger("peck");
                Invoke(nameof(EndAttack), peckDelay);
                break;

            case 2:
                anim.SetTrigger("laser");
                laserHinge.anchor = sprite.flipX ? new Vector2(-0.9f, 0) : new Vector2(-0.9f, 0);
                laserHinge.connectedAnchor = sprite.flipX ? new Vector2(1.9f, 0) : new Vector2(-1.9f, 0);

                laserSprite.enabled = true;
                Invoke(nameof(EndAttack), laserDelay);
                break;

            default:
                break;
        }
    }

    private void EndAttack()
    {
        laserSprite.enabled = false;
        invoked = false;
        doingAttack = false;
    }
}
