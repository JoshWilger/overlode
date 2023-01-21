using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemUsage : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private HudUI hudUiScript;
    [SerializeField] private float explosionDelay;
    [SerializeField] private GameObject item;
    [SerializeField] private Tilemap baseTilemap;
    [SerializeField] private Tilemap mineralTilemap;

    private Movement movementScript;
    private Mining miningScript;
    private Energy energyScript;
    private Health healthScript;
    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private Animator itemAnim;
    private List<GameObject> items;

    // Start is called before the first frame update
    private void Start()
    {
        movementScript = GetComponent<Movement>();
        miningScript = GetComponent<Mining>();
        energyScript = GetComponent<Energy>();
        healthScript = GetComponent<Health>();
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        itemAnim = item.GetComponent<Animator>();
        items = new();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void Animate(string animationName)
    {
        float playerX = coll.bounds.center.x;
        float playerY = coll.bounds.center.y;
        items.Add(Instantiate(item));
        itemAnim = items.Last().GetComponent<Animator>();

        items.Last().transform.position = new Vector3(playerX, playerY);
        itemAnim.SetTrigger(animationName);
    }

    public void ActivateEnergy()
    {
        Animate("energy");

        var upgradeAmount = atlas.currentUpgradeAmounts[(int)ItemAtlas.upgradeTypes.battery];
        var newEnergyLevel = energyScript.energy * upgradeAmount + 30f;
        energyScript.energy = newEnergyLevel > upgradeAmount ? 1 : newEnergyLevel / upgradeAmount;
        energyScript.UpdateEnergyBar();
        Finish();
    }

    public void ActivateHealth()
    {
        Animate("health");
        healthScript.health = 1;
        healthScript.UpdateHealthBar();
        Finish();
    }

    public void ActivateTeleport()
    {
        Animate("teleport");
        rb.bodyType = RigidbodyType2D.Static;
        movementScript.enabled = false;
        miningScript.enabled = false;
        hudUiScript.enabled = false;
        energyScript.decreaseEnergy = false;

        AnimationClip animation = itemAnim.runtimeAnimatorController.animationClips.Where((anim) => anim.name == "teleport").FirstOrDefault();
        Invoke(nameof(RestOfTeleport), animation.length);
    }

    private void RestOfTeleport()
    {
        rb.position = new Vector3(8.5f, 1.5f);
        rb.bodyType = RigidbodyType2D.Dynamic;
        movementScript.enabled = true;
        miningScript.enabled = true;
        hudUiScript.enabled = true;
        energyScript.decreaseEnergy = true;
        Finish();
    }

    public void ActivateDynamite()
    {
        Animate("dynamite");
        Invoke(nameof(RestOfDynamite), explosionDelay);
    }

    private void RestOfDynamite()
    {
        RemoveTiles(3);
        Finish();
    }

    public void ActivateC4()
    {
        Animate("c4");
        Invoke(nameof(RestOfC4), explosionDelay);
    }
    
    private void RestOfC4()
    {
        RemoveTiles(5);
        Finish();
    }

    private void RemoveTiles(int size)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector3Int currentPosition = new(Mathf.FloorToInt(items.First().transform.position.x) + i - size / 2, 
                    Mathf.FloorToInt(items.First().transform.position.y) + j - size / 2);
                TileBase tile = baseTilemap.GetTile(currentPosition);
                TileBase mineral = mineralTilemap.GetTile(currentPosition);

                if (tile)
                {
                    if (atlas.IsIndestructable(tile.name))
                    {
                        continue;
                    }
                }
                if (mineral)
                {
                    if (atlas.IsIndestructable(mineral.name))
                    {
                        continue;
                    }
                }
                if (tile || mineral)
                {
                    baseTilemap.SetTile(currentPosition, null);
                    mineralTilemap.SetTile(currentPosition, null);
                }
            }
        }
        DoDamageIfClose(size);
    }

    private void DoDamageIfClose(int size)
    {
        float playerX = coll.bounds.center.x;
        float playerY = coll.bounds.center.y;
        float distance = Mathf.Max(Mathf.Abs(playerX - items.First().transform.position.x), Math.Abs(playerY - items.First().transform.position.y));

        if (distance <= size / 2f)
        {
            var damage = 1 - (distance / (size / 2f));
            healthScript.UpdateHealth(damage);
        }
    }

    public void ActivateBlock(ItemClass blockItem)
    {
        int playerX = Mathf.FloorToInt(coll.bounds.center.x);
        int playerY = Mathf.FloorToInt(coll.bounds.center.y);
        int[,] directionAdders = new[,] { { 0, 1 }, { -1, 0 }, { 1, 0 } };
        TileBase currentTile;

        for (int i = 0; i < directionAdders.Length; i++)
        {
            currentTile = baseTilemap.GetTile(new Vector3Int(playerX + directionAdders[i, 0], playerY + directionAdders[i, 1]));
            if (!currentTile)
            {
                transform.position = new Vector3(playerX + 0.5f + directionAdders[i, 0], playerY + directionAdders[i, 1]);
                baseTilemap.SetTile(new Vector3Int(playerX, playerY), blockItem.placeableTile);
                return;
            }
        }

    }

    private void Finish()
    {
        Destroy(items.First(), explosionDelay);
        items.Remove(items.First());
    }
}
