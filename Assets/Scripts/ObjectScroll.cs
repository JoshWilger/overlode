using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectScroll : MonoBehaviour
{
    [SerializeField] private Sprite[] spritesToScroll;
    [SerializeField] private AnimationClip animClip;
    [SerializeField] private Animator anim;
    [SerializeField] private bool goingRight;

    private SpriteRenderer sprite;
    private Rigidbody2D rb;

    private void OnEnable()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (anim)
        {
            anim.SetTrigger(animClip.name);
        }
        else
        {
            sprite.sprite = spritesToScroll[Random.Range(0, spritesToScroll.Length)];
        }
        var speed = Random.Range(0.1f, 1);
        rb.velocity = new(goingRight ? speed : -speed, 0);

        transform.position = new(Random.Range(-5, 40), transform.position.y, transform.position.z);
    }

    private void Update()
    {
        if (transform.position.x > 40)
        {
            transform.position = new(-5, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -5)
        {
            transform.position = new(40, transform.position.y, transform.position.z);
        }
    }
}
