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
    [SerializeField] private BossController bossScript;
    [SerializeField] private float explosionDelay;
    [SerializeField] private GameObject item;
    [SerializeField] private Tilemap baseTilemap;
    [SerializeField] private Tilemap mineralTilemap;
    [SerializeField] private GameObject boss;
    [SerializeField] private AudioClip energySound;
    [SerializeField] private AudioClip healthSound;
    [SerializeField] private AudioClip teleportSound;
    [SerializeField] private AudioClip blockSound;
    [SerializeField] private AudioClip explosiveSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip chickenDamageSound;
    [SerializeField] private AudioClip golemDamageSound;

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

    private void Animate(string animationName)
    {
        float playerX = coll.bounds.center.x;
        float playerY = coll.bounds.center.y;
        items.Add(Instantiate(item));
        itemAnim = items.Last().GetComponent<Animator>();

        items.Last().transform.position = new Vector3(playerX, playerY);
        itemAnim.SetTrigger(animationName);
    }

    public bool ActivateEnergy()
    {
        if (energyScript.energy < 0.99f)
        {
            Animate("energy");
            var aud = items.Last().GetComponent<AudioSource>();
            aud.clip = energySound;
            aud.Play();

            var upgradeAmount = atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.battery];
            var newEnergyLevel = energyScript.energy * upgradeAmount + 25f;
            energyScript.energy = newEnergyLevel > upgradeAmount ? 1 : newEnergyLevel / upgradeAmount;
            energyScript.UpdateEnergyBar();
            Finish();

            return true;
        }

        return false;
    }

    public bool ActivateHealth()
    {
        if (healthScript.health < 1)
        {
            Animate("health");
            var aud = items.Last().GetComponent<AudioSource>();
            aud.clip = healthSound;
            aud.Play(); 
            
            var upgradeAmount = atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.health];
            var newHealthLevel = healthScript.health * upgradeAmount + 30f;
            healthScript.health = newHealthLevel > upgradeAmount ? 1 : newHealthLevel / upgradeAmount; 
            healthScript.UpdateHealthBar();
            Finish();
            return true;
        }

        return false;
    }

    public bool ActivateTeleport()
    {
        if (miningScript.IsGrounded())
        {
            Animate("teleport");
            FreezePlayer();
            var aud = items.Last().GetComponent<AudioSource>();
            aud.clip = teleportSound;
            aud.Play();

            AnimationClip animation = itemAnim.runtimeAnimatorController.animationClips.Where((anim) => anim.name == "teleport").FirstOrDefault();
            Invoke(nameof(RestOfTeleport), animation.length);
            return true;
        }

        return false;
    }

    public void FreezePlayer()
    {
        rb.bodyType = RigidbodyType2D.Static;
        movementScript.enabled = false;
        miningScript.enabled = false;
        hudUiScript.enabled = false;
        energyScript.decreaseEnergy = false;
    }

    private void RestOfTeleport()
    {
        var pos = new Vector3Int(12, 1);
        for (int i = 2; baseTilemap.GetTile(pos); i++)
        {
            pos = new Vector3Int(pos.x, i);
        }

        rb.position = new Vector2(pos.x + 0.5f, pos.y + 0.5f);
        rb.bodyType = RigidbodyType2D.Dynamic;
        movementScript.enabled = true;
        miningScript.enabled = true;
        hudUiScript.enabled = true;
        energyScript.decreaseEnergy = true;
        Finish();
    }

    public bool ActivateDynamite()
    {
        if (miningScript.IsGrounded())
        {
            Animate("dynamite");
            var aud = items.Last().GetComponent<AudioSource>();
            aud.clip = explosiveSound;
            aud.Play();
            
            Invoke(nameof(RestOfDynamite), explosionDelay);
            return true;
        }

        return false;
    }

    private void RestOfDynamite()
    {
        RemoveTiles(3);
        Finish();
    }

    public bool ActivateC4()
    {
        if (miningScript.IsGrounded())
        {
            Animate("c4");
            var aud = items.Last().GetComponent<AudioSource>();
            aud.clip = explosiveSound;
            aud.Play();
            
            Invoke(nameof(RestOfC4), explosionDelay);
            return true;
        }

        return false;
    }
    
    private void RestOfC4()
    {
        RemoveTiles(5);
        Finish();
    }

    public void RemoveTiles(int size, Vector3Int basePosition, int damageMultiplier = 1)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector3Int currentPosition = new(basePosition.x + i - size / 2, basePosition.y + j - size / 2);
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
        DoDamageIfClose(size * damageMultiplier, basePosition);
    }

    private void RemoveTiles(int size)
    {
        var aud = items.First().GetComponent<AudioSource>();
        aud.clip = explosionSound;
        aud.Play();

        RemoveTiles(size, new(Mathf.FloorToInt(items.First().transform.position.x),
                    Mathf.FloorToInt(items.First().transform.position.y)));
    }

    private void DoDamageIfClose(int size, Vector3Int basePosition)
    {
        float playerX = coll.bounds.center.x;
        float playerY = coll.bounds.center.y;
        float distance = Mathf.Max(Mathf.Abs(playerX - basePosition.x), Math.Abs(playerY - basePosition.y));

        if (distance <= size / 2f)
        {
            float healthUpgrade = atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.health];
            float coolingUpgrade = atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.cooling];
            var damage = (size - distance) / ((healthUpgrade / 12f) + (coolingUpgrade / 10f));
            Debug.Log("Youch! " + damage);
            healthScript.UpdateHealth(damage);
        }
        if (boss.activeSelf)
        {
            float distance2 = Mathf.Max(Mathf.Abs(boss.transform.position.x - basePosition.x), Math.Abs(boss.transform.position.y - basePosition.y - 2));

            if (distance2 <= size / 2f)
            {
                var damage = Mathf.Abs(size - distance2) / (bossScript.nextBoss ? bossScript.bossDamageDivisor * 2f : bossScript.bossDamageDivisor);
                Debug.Log("Agh! " + damage);
                bossScript.UpdateHealth(damage);

                var aud = boss.GetComponent<AudioSource>();
                aud.clip = bossScript.nextBoss ? golemDamageSound : chickenDamageSound;
                aud.Play();
            }
        }
    }

    public bool ActivateBlock(ItemClass blockItem)
    {
        if (miningScript.IsGrounded())
        {
            int playerX = Mathf.FloorToInt(coll.bounds.center.x);
            int playerY = Mathf.FloorToInt(coll.bounds.center.y);
            int[,] directionAdders = new[,] { { 0, 1 }, { -1, 0 }, { 1, 0 } };
            TileBase currentTile;

            for (int i = 0; i <= directionAdders.Length / 2 - 1; i++)
            {
                var newX = playerX + directionAdders[i, 0];
                currentTile = baseTilemap.GetTile(new Vector3Int(newX, playerY + directionAdders[i, 1]));
                if (!currentTile && newX < TerrainGeneration.WIDTH && newX > 0)
                {
                    transform.position = new Vector3(playerX + 0.5f + directionAdders[i, 0], playerY + directionAdders[i, 1]);
                    baseTilemap.SetTile(new Vector3Int(playerX, playerY), blockItem.placeableTile);

                    item.GetComponent<AudioSource>().Play();
                    return true;
                }
            }
        }

        return false;
    }

    private void Finish()
    {
        Destroy(items.First(), explosionDelay);
        items.Remove(items.First());
    }
}
