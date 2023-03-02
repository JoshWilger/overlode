using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private Energy energyScript;
    [SerializeField] private LayerMask ground;
    [SerializeField] private AudioClip walk;
    [SerializeField] private AudioClip fly;
    [SerializeField] private AudioClip drill;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float xDivisor;
    [SerializeField] private float yDivisor;
    [SerializeField] private float cruise;
    [SerializeField] private float playerToCeilGap = 0.1f;

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;
    private AudioSource aud;
    private ItemClass[] minerals;
    private bool countedRecently;
    private float weight;

    public float dirX;
    public float dirY;

    private enum MovementState { idle, walking, flying, falling, drillup, drilldown, drillside }

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
        minerals = atlas.CreateInstance(ItemClass.ItemType.mineral);
        countedRecently = false;
        weight = 0;
    }

    private void FixedUpdate()
    {
        if (!enabled) return;

        dirX = Input.GetAxis("Horizontal");
        dirY = Input.GetAxis("Vertical");

        var amount = atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.jetpack] / 5f;
        if (!countedRecently)
        {
            countedRecently = true;
            weight = CountMineralWeight();
            Invoke(nameof(UpdateRecently), 1);
        }

        Vector2 force;
        if (dirY > 0f && (!IsGrounded() || !IsACeiling()))
        {
            force = new Vector2(dirX / 2f * (amount / (weight + 1f)) / xDivisor, dirY / 5f * (amount / (weight + 1f)) / yDivisor);
            energyScript.UpdateEnergy(300f);
        }
        else if (dirX != 0f && (!IsGrounded()))
        {
            force = new Vector2(dirX / 2f * (amount / (weight + 1f) / xDivisor), MathF.Abs(dirX / 5f * cruise));
            energyScript.UpdateEnergy(600f);
        }
        else
        {
            force = new Vector2(dirX > 0.1f ? moveSpeed : dirX < -0.1f ? -moveSpeed : -dirX, 0);
        }
        rb.AddForce(force);

        UpdateAnimationState();
    }

    private void UpdateRecently()
    {
        countedRecently = false;
    }

    private void UpdateAnimationState()
    {
        MovementState state;
        AudioClip newClip;

        if (dirX > 0f)
        {
            if (IsAWall(Vector2.right) && IsGrounded())
            {
                state = MovementState.drillside;
                newClip = drill;
            }
            else if (!IsGrounded())
            {
                state = MovementState.flying;
                newClip = fly;
            }
            else
            {
                state = MovementState.walking;
                newClip = walk;
            }
            sprite.flipX = true;
        }
        else if (dirX < 0f)
        {
            if (IsAWall(Vector2.left) && IsGrounded())
            {
                state = MovementState.drillside;
                newClip = drill;
            }
            else if (!IsGrounded())
            {
                state = MovementState.flying;
                newClip = fly;
            }
            else
            {
                state = MovementState.walking;
                newClip = walk;
            }
            sprite.flipX = false;
        }
        else if (dirY < -0.001f)
        {
            state = MovementState.drilldown;
            newClip = drill;
        }
        else if (dirY > 0.001f)
        {
            if (IsGrounded() && IsACeiling())
            {
                state = MovementState.drillup;
                newClip = drill;
            }
            else
            {
                state = MovementState.flying;
                newClip = fly;
            }
        }
        else if (!IsGrounded())
        {
            state = MovementState.falling;
            newClip = null;
        }
        else
        {
            state = MovementState.idle;
            newClip = null;
        }

        if (aud.clip != newClip)
        {
            aud.clip = newClip;
            aud.Play();
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

    private float CountMineralWeight()
    {
        float counter = 0;

        foreach (var item in minerals)
        {
            var amount = item.amountCollected;
            if (amount > 0)
            {
                var log = Mathf.Pow(item.itemWorth, 0.01f) - 1;
                counter += amount * log;
            }
        }
        return counter;
    }
}
