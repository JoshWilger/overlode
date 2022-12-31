using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private UIController controller;

    [SerializeField] private float time;
    [SerializeField] private Animator breaking;
    [SerializeField] private Transform move;
    [SerializeField] private Tilemap baseTilemap;
    [SerializeField] private Tilemap mineralTilemap;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private LayerMask ground;

    private readonly int[,] directionAdders = { { 0, 1 }, { 0, -1 }, { -1, 0 }, { 1, 0 } }; // up, down, left, right
    private bool mining;
    private int currentDirectionNum;
    private int previousDirectionNum;
    private Vector3Int currentTile;
    private Vector3Int previousTile;
    private ItemClass[] artifacts;
    private ItemClass[] minerals;
    private TextMeshProUGUI[] inventoryMineralTexts;

    // Start is called before the first frame update
    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        currentDirectionNum = 4;
        currentTile = new Vector3Int(10000, 10000);
        artifacts = atlas.CreateInstance(ItemClass.ItemType.artifact);
        minerals = atlas.CreateInstance(ItemClass.ItemType.mineral);
        inventoryMineralTexts = controller.RetrieveInventoryText(ItemClass.ItemType.mineral);

        foreach (var item in minerals)
        {
            item.amountCollected = 0;
        }
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
        int playerX = Mathf.FloorToInt(coll.bounds.center.x);
        int playerY = Mathf.FloorToInt(coll.bounds.center.y);

        if (baseTilemap.GetTile(new Vector3Int(playerX, playerY)))
        {
            Debug.Log("Player is in a tile!");
            return false;
        }

        currentTile = new Vector3Int(playerX + directionAdders[currentDirectionNum, 0], playerY + directionAdders[currentDirectionNum, 1]);
        TileBase tile = baseTilemap.GetTile(currentTile);
        TileBase mineral = mineralTilemap.GetTile(currentTile);

        if (tile)
        {
            if (tile.name == "pxy-border")
            {
                return false;
            }
        }
        if (mineral)
        {
            if (mineral.name == "pxy-lava_2a" || mineral.name == "stone")
            {
                return false;
            }
        }
        if (tile || mineral)
        {
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
        int playerX = Mathf.FloorToInt(coll.bounds.center.x);
        int playerY = Mathf.FloorToInt(coll.bounds.center.y);

        if (baseTilemap.GetTile(new Vector3Int(playerX, playerY)))
        {
            Debug.Log("Player is in a tile!");
            mining = false;
            return;
        }

        currentTile = new Vector3Int(playerX + directionAdders[currentDirectionNum, 0], playerY + directionAdders[currentDirectionNum, 1]);

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
                    minerals[i].amountCollected++;
                    inventoryMineralTexts[i].text = "x" + minerals[i].amountCollected;
                    return;
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

    private bool IsGrounded()
    {
        return baseTilemap.GetTile(new Vector3Int(Mathf.FloorToInt(coll.bounds.center.x), Mathf.FloorToInt(coll.bounds.center.y) - 1));
    }
}
