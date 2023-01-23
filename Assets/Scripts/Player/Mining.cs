using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.UI;

public class Mining : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private HudUI controller;
    [SerializeField, Range(0, 1)] private float tileMiningDistance;
    [SerializeField] private Animator breaking;
    [SerializeField] private Transform move;
    [SerializeField] private Tilemap baseTilemap;
    [SerializeField] private Tilemap mineralTilemap;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI storageText;
    [SerializeField] private Image storageProgress;
    [SerializeField] private LayerMask ground;

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private float[,] directionAdders;
    private bool mining;
    private int currentDirectionNum;
    private int previousDirectionNum;
    private Vector3Int currentTile;
    private Vector3Int previousTile;
    private ItemClass[] artifacts;
    private ItemClass[] minerals;

    // Start is called before the first frame update
    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        directionAdders = new[,] { { 0, 1 }, { 0, -1 }, { -tileMiningDistance, 0 }, { tileMiningDistance, 0 } }; // up, down, left, right
        currentDirectionNum = 4;
        currentTile = new Vector3Int(10000, 10000);
        artifacts = atlas.CreateInstance(ItemClass.ItemType.artifact);
        minerals = atlas.CreateInstance(ItemClass.ItemType.mineral);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!enabled) return;

        float dirX = Input.GetAxis("Horizontal");
        float dirY = Input.GetAxis("Vertical");

        if (dirY > 0f && Input.GetButton("Vertical") && rb.velocity.y == 0f && IsGrounded() && dirX == 0f && !mining)
        {
            currentDirectionNum = 0;
            mining = BreakTile();
        }
        else if (dirY < 0f && Input.GetButton("Vertical") && rb.velocity.y == 0f && IsGrounded() && dirX == 0f && !mining)
        {
            currentDirectionNum = 1;
            mining = BreakTile();
        }
        else if (dirX < 0f && IsGrounded() && Input.GetButton("Horizontal") && dirY == 0f && !mining)
        {
            currentDirectionNum = 2;
            mining = BreakTile();
        }
        else if (dirX > 0f && IsGrounded() && Input.GetButton("Horizontal") && dirY == 0f && !mining)
        {
            currentDirectionNum = 3;
            mining = BreakTile();
        }
    }
    private bool BreakTile()
    {
        currentTile = GetCurrentTile();
        TileBase tile = baseTilemap.GetTile(currentTile);
        TileBase mineral = mineralTilemap.GetTile(currentTile);

        if (tile)
        {
            if (atlas.IsUnminable(tile.name))
            {
                return false;
            }
        }
        if (mineral)
        {
            if (atlas.IsUnminable(mineral.name))
            {
                return false;
            }
        }
        if (tile || mineral)
        {
            var time = 1f / (atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.drill] / atlas.GroundWorth(tile));

            move.position = new Vector3(currentTile.x + 0.5f, currentTile.y + 0.5f, 2);
            breaking.speed = 0.33f / time;
            breaking.SetTrigger("break");
            previousDirectionNum = currentDirectionNum;
            previousTile = currentTile;
            Invoke(nameof(RemoveTile), time);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void RemoveTile()
    {
        currentTile = GetCurrentTile();

        if (mining && currentDirectionNum == previousDirectionNum && currentTile == previousTile)
        {
            MineMineral();
            baseTilemap.SetTile(currentTile, null);
            mineralTilemap.SetTile(currentTile, null);
        }
        mining = false;
    }

    private void MineMineral()
    {
        TileBase mineral = mineralTilemap.GetTile(currentTile);

        if (mineral)
        {
            for (int i = 0; i < minerals.Length; i++)
            {
                if (mineral.name == minerals[i].placeableTile.name)
                {
                    var collection = CountCollectedMinerals();
                    var space = atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.storage];
                    if (collection < space)
                    {
                        minerals[i].amountCollected++;
                        controller.UpdateMineralInfo();
                        return;
                    }
                    else
                    {
                        Debug.Log("Bag full!!!");
                    }
                }
            }
            foreach (var artifact in artifacts)
            {
                if (mineral.name == artifact.placeableTile.name)
                {
                    moneyText.text = "$" + (long.Parse(moneyText.text.Substring(1)) + artifact.itemWorth);
                    return;
                }
            }
        }
    }

    public bool IsGrounded()
    {
        return baseTilemap.GetTile(new Vector3Int(Mathf.FloorToInt(coll.bounds.center.x), Mathf.FloorToInt(coll.bounds.center.y) - 1));
    }

    private Vector3Int GetCurrentTile()
    {
        float playerX = coll.bounds.center.x;
        float playerY = coll.bounds.center.y;

        if (baseTilemap.GetTile(new Vector3Int(Mathf.FloorToInt(playerX), Mathf.FloorToInt(playerY))))
        {
            Debug.Log("Player is in a tile!");
            return new Vector3Int(10000, 10000);
        }

        return new Vector3Int(Mathf.FloorToInt(playerX + directionAdders[currentDirectionNum, 0]), 
            Mathf.FloorToInt(playerY + directionAdders[currentDirectionNum, 1]));
    }

    public int CountCollectedMinerals()
    {
        var counter = 0;
        foreach (var item in minerals)
        {
            counter += item.amountCollected;
        }

        return counter;
    }
}
