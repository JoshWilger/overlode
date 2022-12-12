using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    public float dirX;
    public float dirY;

    [SerializeField] private LayerMask ground;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float flySpeed = 10f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float playerToCeilGap = 0.1f;

    private enum MovementState { idle, walking, flying, falling, drillup, drilldown, drillside }

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!enabled) return;

        dirX = Input.GetAxis("Horizontal");
        dirY = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (dirY > 0f)
        {
            if (!IsGrounded())
            {
                rb.velocity = new Vector2(dirX * flySpeed, dirY * jumpForce);
            }
            else if (!IsACeiling())
            {
                rb.velocity = new Vector2(dirX * flySpeed, dirY * jumpForce);
            }
        }

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            if (IsAWall(Vector2.right))
            {
                state = MovementState.drillside;
            }
            else
            {
                state = MovementState.walking;
            }
            sprite.flipX = true;
        }
        else if (dirX < 0f)
        {
            if (IsAWall(Vector2.left))
            {
                state = MovementState.drillside;
            }
            else
            {
                state = MovementState.walking;
            }
            sprite.flipX = false;
        }
        else if (dirY < -0.001f)
        {
            state = MovementState.drilldown;
        }
        else if (dirY > 0.001f && IsACeiling())
        {
            state = MovementState.drillup;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > 0.001f)
        {
            state = MovementState.flying;
        }
        else if (rb.velocity.y < -0.001f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool IsACeiling()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, playerToCeilGap, ground);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, ground);
    }

    private bool IsAWall(Vector2 direction)
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, direction, 0.1f, ground);
    }
}
