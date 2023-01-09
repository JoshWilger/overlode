using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemUsage : MonoBehaviour
{
    [SerializeField] private float dynamiteDelay;
    [SerializeField] private float c4Delay;
    [SerializeField] private GameObject item;
    [SerializeField] private Tilemap baseTilemap;
    [SerializeField] private Tilemap mineralTilemap;

    private Movement movementScript;
    private Mining miningScript;
    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private Animator itemAnim;

    // Start is called before the first frame update
    private void Start()
    {
        movementScript = GetComponent<Movement>();
        miningScript = GetComponent<Mining>();
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        itemAnim = item.GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void Animate(string animationName)
    {
        float playerX = coll.bounds.center.x;
        float playerY = coll.bounds.center.y;

        item.transform.position = new Vector3(playerX, playerY);
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
        rb.bodyType = RigidbodyType2D.Static;
        movementScript.enabled = false;
        miningScript.enabled = false;

        AnimationClip animation = itemAnim.runtimeAnimatorController.animationClips.Where((anim) => anim.name == "teleport").FirstOrDefault();
        Invoke(nameof(RestOfTeleport), animation.length);
    }

    private void RestOfTeleport()
    {
        rb.position = new Vector3(8.5f, 1.5f);
        rb.bodyType = RigidbodyType2D.Dynamic;
        movementScript.enabled = true;
        miningScript.enabled = true;
    }

    public void ActivateDynamite()
    {
        Animate("dynamite");
        Invoke(nameof(RestOfDynamite), dynamiteDelay);
    }

    private void RestOfDynamite()
    {
        RemoveTiles(3);
    }

    public void ActivateC4()
    {
        Animate("c4");
        Invoke(nameof(RestOfC4), c4Delay);
    }
    
    private void RestOfC4()
    {
        RemoveTiles(5);
    }

    private void RemoveTiles(int size)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int x = Mathf.FloorToInt(item.transform.position.x) + i - size / 2;
                int y = Mathf.FloorToInt(item.transform.position.y) + j - size / 2;

                baseTilemap.SetTile(new Vector3Int(x, y), null);
                mineralTilemap.SetTile(new Vector3Int(x, y), null);
            }
        }


    }

    public void ActivateBlock()
    {
        Animate("block");

    }
}
