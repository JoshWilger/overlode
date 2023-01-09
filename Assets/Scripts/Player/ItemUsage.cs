using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUsage : MonoBehaviour
{
    [SerializeField] private GameObject item;
    private BoxCollider2D coll;

    private Movement movementScript;
    private Mining miningScript;
    private Animator itemAnim;
    private Transform itemMove;

    // Start is called before the first frame update
    private void Start()
    {
        movementScript = GetComponent<Movement>();
        miningScript = GetComponent<Mining>();
        coll = GetComponent<BoxCollider2D>();
        itemAnim = item.GetComponent<Animator>();
        itemMove = item.GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void Animate(string animationName)
    {
        float playerX = coll.bounds.center.x;
        float playerY = coll.bounds.center.y;

        itemMove.position = new Vector3(playerX, playerY);
        itemAnim.SetTrigger(animationName);
    }

    public void ActivateEnergy()
    {
        Animate("energy");

    }

    public void ActivateHealth()
    {
        Animate("health");

    }

    public void ActivateTeleport()
    {
        Animate("teleport");

    }

    public void ActivateDynamite()
    {
        Animate("dynamite");

    }

    public void ActivateC4()
    {
        Animate("c4");

    }

    public void ActivateBlock()
    {
        Animate("block");

    }
}
