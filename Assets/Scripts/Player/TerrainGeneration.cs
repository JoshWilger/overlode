using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGeneration : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;

    [SerializeField, Range(0, 1)] private float airChance;
    [SerializeField, Range(0, 1)] private float mineralChance;
    [SerializeField, Range(0, 1)] private float stoneLavaChance;
    [SerializeField, Range(0, 1)] private float artifactChance;

    [SerializeField] private int width = 33;
    [SerializeField] private int depth = 98; // 599 is good actual value

    private Tilemap[] tilemaps;
    private ItemClass[] background;
    private ItemClass[] ground;
    private ItemClass[] minerals;
    private ItemClass[] miscGround; // lava, stone, stoney dirts (2)
    private ItemClass[] artifacts;

    // Start is called before the first frame update
    private void Start()
    {
        background = atlas.CreateInstance(ItemClass.ItemType.background);
        ground = atlas.CreateInstance(ItemClass.ItemType.ground);
        minerals = atlas.CreateInstance(ItemClass.ItemType.mineral);
        miscGround = atlas.CreateInstance(ItemClass.ItemType.miscGround);
        artifacts = atlas.CreateInstance(ItemClass.ItemType.artifact);
        tilemaps = GetComponentsInChildren<Tilemap>();
        int[] artifactCounter = new int[artifacts.Length + 1];

        for (int y = 0; y < depth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int tilePos = new(x, -y - 2);
                bool isAir = Random.Range(0f, 1f) < airChance;
                float holyRolly = Random.Range(0f, 1f);
                float dungeonRoll = 1 - holyRolly;
                int layerLength = Mathf.Min(ground.Length, minerals.Length);
                int layerNum = Mathf.Min(Mathf.FloorToInt(y / ((float)(depth / layerLength) + 1 + holyRolly)), layerLength);

                tilemaps[0].SetTile(tilePos, dungeonRoll < artifactChance && !isAir && (y > depth / 16) ? background[1].placeableTile : background[0].placeableTile);

                tilemaps[1].SetTile(tilePos,  isAir ? null : 
                    layerNum == 2 && holyRolly < 0.2f ? miscGround[2].placeableTile : 
                    layerNum == 3 && holyRolly < 0.5f ? miscGround[3].placeableTile : 
                    ground[layerNum].placeableTile);

                if (!isAir)
                {
                    float stoneLavaRoll = Random.Range(0f, 1f);
                    Vector3Int oneAbove = new(tilePos.x, tilePos.y + 1);
                    int mineralNum = GetMineralNum(y);
                    int artifactNum = Mathf.FloorToInt((float)(holyRolly / artifactChance) * (artifacts.Length));

                    if ((y > depth / 2) && !tilemaps[1].GetTile(oneAbove) && (stoneLavaRoll < (2 * stoneLavaChance) * y / depth))
                    {
                        tilemaps[2].SetTile(oneAbove, miscGround[0].placeableTile);
                    }

                    tilemaps[2].SetTile(tilePos, mineralNum == -1 ?
                        y > depth / 4 && stoneLavaRoll < stoneLavaChance * y / depth ? miscGround[1].placeableTile 
                        : holyRolly < artifactChance && (y > depth / 16) ? artifacts[artifactNum].placeableTile : null
                        : minerals[mineralNum].placeableTile);

                    if (holyRolly < artifactChance && (y > depth / 16)) artifactCounter[artifactNum]++; // for logging the artifacts
                    if (dungeonRoll < artifactChance && !isAir && (y > depth / 16)) artifactCounter[artifacts.Length]++;
                }
            }
        }

        foreach (var item in minerals)
        {
            Debug.Log(item.itemName + " " + item.itemWorth);
        }
        Debug.Log("Artifacts: " + string.Join(" ", artifactCounter));

        GenerateBottom();
    }
    
    // TODO: Better mineral, lava, and stone distribution
    private int GetMineralNum(int y)
    {
        float mineralRoll = Random.Range(0f, 1f);

        int layer = Mathf.FloorToInt(y / ((float)(depth / (minerals.Length - 3)) + 1));

        if (mineralRoll < (float)(mineralChance / 20)) { return layer + 3; }
        if (mineralRoll < (float)(mineralChance / 10)) { return layer + 2; }
        if (mineralRoll < (float)(mineralChance / 2)) { return layer + 1; }
        if (mineralRoll < mineralChance) { return layer; }

        return -1;
    }

    private void GenerateBottom()
    {
        Vector3Int[] coords = { new Vector3Int(7, -depth - 3), new Vector3Int(21, -depth - 3), new Vector3Int(35, -depth - 3) };
        tilemaps[1].SetTiles(coords, new TileBase[] { miscGround[4].placeableTile, miscGround[4].placeableTile, miscGround[4].placeableTile });
    }
}
