using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenMovement : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private Health healthScript;
    [SerializeField] private float damageAffector;
    [SerializeField] private Collider2D playerColl;
    [SerializeField] private GameObject laser;
    [SerializeField] private AudioClip laserSound;
    [SerializeField] private AudioClip peckSound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip prepareSound;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackInterval;
    [SerializeField] private float peckDelay;
    [SerializeField] private float laserDelay;
    [SerializeField] private float summonDelay;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private HingeJoint2D laserHinge;
    private SpriteRenderer laserSprite;
    private BoxCollider2D laserCollider;
    private AudioSource aud;
    private AudioSource loop;
    private bool doingAttack;
    private bool invoked;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        laserHinge = laser.GetComponent<HingeJoint2D>();
        laserSprite = laser.GetComponent<SpriteRenderer>();
        laserCollider = laser.GetComponent<BoxCollider2D>();
        aud = GetComponent<AudioSource>();
        loop = laser.GetComponent<AudioSource>();
        doingAttack = true;
        invoked = true;
        transform.position = new Vector3(TerrainGeneration.WIDTH / 2, -560f, transform.position.z);
        rb.velocity = new Vector2(0, movementSpeed);
        anim.SetTrigger("laser");
        anim.speed = 0.5f;
    }

    private void OnDisable()
    {
        anim.SetTrigger("laser");
        laserSprite.enabled = false;
        laserCollider.enabled = false;
        anim.speed = 0;
        rb.velocity = Vector2.down;
        CancelInvoke();
        loop.clip = walkSound;
        loop.Stop();
    }

    public void EndSummoningSequence()
    {
        doingAttack = false;
        invoked = false;
    }

    private void FixedUpdate()
    {
        if (transform.position.y > -555f)
        {
            rb.velocity = Vector2.zero;
            transform.position = new Vector3(transform.position.x, -555f, transform.position.z);
            anim.speed = 1;
            Invoke(nameof(EndSummoningSequence), summonDelay);
        }

        var playerX = playerColl.bounds.center.x;

        if (!invoked)
        {
            invoked = true;
            anim.SetTrigger("waddle");
            Invoke(nameof(Attack), attackInterval);
            loop.clip = walkSound;
            loop.Play();
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
                anim.SetTrigger("peck");
                loop.Stop();
                aud.clip = prepareSound;
                aud.Play();

                Invoke(nameof(DoDamage), peckDelay / 2f);
                Invoke(nameof(EndAttack), peckDelay);
                break;

            case 2:
                anim.SetTrigger("laser");
                loop.Stop();
                laserHinge.anchor = sprite.flipX ? new Vector2(-0.9f, 0) : new Vector2(-0.9f, 0);
                laserHinge.connectedAnchor = sprite.flipX ? new Vector2(1.9f, 0) : new Vector2(-1.9f, 0);
                aud.clip = laserSound;
                aud.Play();

                laserSprite.enabled = true;
                laserCollider.enabled = true;
                Invoke(nameof(EndAttack), laserDelay);
                break;

            default:
                break;
        }
    }

    private void EndAttack()
    {
        laserSprite.enabled = false;
        laserCollider.enabled = false;
        invoked = false;
        doingAttack = false;
    }

    private void DoDamage()
    {
        var diffY = playerColl.bounds.center.y - transform.position.y + 2f;
        var diffX = playerColl.bounds.center.x - transform.position.x;

        if (diffY <= 0 && (sprite.flipX ? diffX <= 4f && diffX >= 0f : diffX >= -4f && diffX <= 0f ))
        {
            float healthUpgrade = atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.health];
            var damage = 1f / ((healthUpgrade / 12f)) * damageAffector;
            Debug.Log("Oh! " + damage);
            healthScript.UpdateHealth(damage);
        }

        aud.clip = peckSound;
        aud.Play();
    }
}
