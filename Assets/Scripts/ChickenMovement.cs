using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenMovement : MonoBehaviour
{
    [SerializeField] private Collider2D playerColl;
    [SerializeField] private float movementSpeed;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private bool doingAttack;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        doingAttack = false;
    }

    private void FixedUpdate()
    {
        var playerX = playerColl.bounds.center.x;
        var playerY = playerColl.bounds.center.y;

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
}
