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

    private readonly int[,] directionAdders = { { 0, 1 }, { 0, -1 }, { -1, 0 }, { 1, 0 } }; // up, down, left, right
    private bool mining;
    private int currentDirectionNum;
    private Vector3Int currentTile;
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

        CheckMining();

        for (int i = 0; i < 4; i++)
        {
            if (!mining)
            {
                currentDirectionNum = i;
                CheckMining();
                BreakTile();
            }
        }
    }

    private bool CheckMining()
    {
        float dirX = Input.GetAxis("Horizontal");
        float dirY = Input.GetAxis("Vertical");
        
        //Debug.Log("dirX " + dirX + "\n Horizontal? " + Input.GetButton("Horizontal") + " Velocity 0? " + (rb.velocity.y == 0f));

        bool[] directionPressed = {
            dirY > 0f && Input.GetButton("Vertical") && rb.velocity.y == 0f,
            dirY < 0f && Input.GetButton("Vertical") && rb.velocity.y == 0f,
            dirX < 0f && Input.GetButton("Horizontal") && rb.velocity.y == 0f,
            dirX > 0f && Input.GetButton("Horizontal") && rb.velocity.y == 0f,
            false
        };

        mining = directionPressed[currentDirectionNum];
        currentDirectionNum = mining ? currentDirectionNum : 4;

        return mining;
    }

    private void BreakTile()
    {
        int playerX = Mathf.FloorToInt(coll.bounds.center.x);
        int playerY = Mathf.FloorToInt(coll.bounds.center.y);

        if (currentDirectionNum == 4) return;
        currentTile = new Vector3Int(playerX + directionAdders[currentDirectionNum, 0], playerY + directionAdders[currentDirectionNum, 1]);

        //Debug.Log(currentTile.x + ", " + currentTile.y);

        TileBase tile = baseTilemap.GetTile(currentTile);
        TileBase mineral = mineralTilemap.GetTile(currentTile);

        //Debug.Log("Player pos: " + playerX + ", " + playerY);

        if (tile)
        {
            if (tile.name == "pxy-border") return;
        }
        if (mineral)
        {
            if (mineral.name == "pxy-lava_2a" || mineral.name == "stone") return;
        }
        if (tile || mineral)
        {
            move.position = new Vector3(currentTile.x + 0.5f, currentTile.y + 0.5f, 2);
            breaking.speed = 0.33f / time;
            breaking.SetTrigger("break");
            Invoke(nameof(RemoveTile), time);
        }
    }

    private void RemoveTile()
    {
        if (CheckMining())
        {
            MineMineral();
            baseTilemap.SetTile(currentTile, null);
            mineralTilemap.SetTile(currentTile, null);
        }

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
}
