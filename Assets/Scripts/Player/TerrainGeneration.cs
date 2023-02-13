using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGeneration : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;

    [SerializeField, Range(0, 1)] private float earthquakeAirChance;
    [SerializeField, Range(0, 1)] private float airChance;
    [SerializeField, Range(0, 1)] private float mineralChance;
    [SerializeField, Range(0, 1)] private float stoneLavaChance;
    [SerializeField, Range(0, 1)] private float artifactChance;
    [SerializeField] private int earthquakeLayerAttempts;
    [SerializeField] private int artifactDepthDividend;
    [SerializeField] private int stoneDepthDividend;
    [SerializeField] private int lavaDepthDividend;

    private Tilemap[] tilemaps;
    private ItemClass[] background;
    private ItemClass[] ground;
    private ItemClass[] minerals;
    private ItemClass[] miscGround; 
    private ItemClass[] artifacts;

    public const int WIDTH = 36;
    public const int DEPTH = 550;
    
    // Start is called before the first frame update
    private void Start()
    {
        background = atlas.CreateInstance(ItemClass.ItemType.background);
        ground = atlas.CreateInstance(ItemClass.ItemType.ground);
        minerals = atlas.CreateInstance(ItemClass.ItemType.mineral);
        miscGround = atlas.CreateInstance(ItemClass.ItemType.miscGround, false);
        artifacts = atlas.CreateInstance(ItemClass.ItemType.artifact);
        tilemaps = GetComponentsInChildren<Tilemap>();
        int[] artifactCounter = new int[artifacts.Length + 1];

        for (int y = 0; y < DEPTH; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                Vector3Int tilePos = new(x, -y - 2);
                bool isAir = Random.Range(0f, 1f) < airChance;
                float holyRolly = Random.Range(0f, 1f);
                float dungeonRoll = 1 - holyRolly;
                int layerLength = Mathf.Min(ground.Length, minerals.Length);
                int layerNum = Mathf.Min(Mathf.FloorToInt(y / ((float)(DEPTH / layerLength) + 1 + holyRolly)), layerLength);

                tilemaps[0].SetTile(tilePos, dungeonRoll < artifactChance && !isAir && (y > DEPTH / artifactDepthDividend) ? background[1].placeableTile : background[0].placeableTile);

                tilemaps[1].SetTile(tilePos,  isAir ? null : 
                    layerNum == 2 && holyRolly < 0.2f ? miscGround[0].placeableTile : 
                    layerNum == 3 && holyRolly < 0.5f ? miscGround[1].placeableTile : 
                    ground[layerNum].placeableTile);

                if (!isAir)
                {
                    float stoneLavaRoll = Random.Range(0f, 1f);
                    Vector3Int oneAbove = new(tilePos.x, tilePos.y + 1);
                    int mineralNum = GetMineralNum(y);
                    int artifactNum = Mathf.FloorToInt((float)(holyRolly / artifactChance) * (artifacts.Length));

                    if ((y > DEPTH / lavaDepthDividend) && !tilemaps[1].GetTile(oneAbove) && (stoneLavaRoll < (2 * stoneLavaChance) * y / DEPTH))
                    {
                        tilemaps[2].SetTile(oneAbove, miscGround[3].placeableTile);
                    }

                    tilemaps[2].SetTile(tilePos, mineralNum == -1 ?
                        y > DEPTH / stoneDepthDividend && stoneLavaRoll < stoneLavaChance * y / DEPTH ? miscGround[2].placeableTile 
                        : holyRolly < artifactChance && (y > DEPTH / artifactDepthDividend) ? artifacts[artifactNum].placeableTile : null
                        : minerals[mineralNum].placeableTile);

                    if (holyRolly < artifactChance && (y > DEPTH / artifactDepthDividend)) artifactCounter[artifactNum]++; // for logging the artifacts
                    if (dungeonRoll < artifactChance && !isAir && (y > DEPTH / artifactDepthDividend)) artifactCounter[artifacts.Length]++;
                }
            }
        }
        Debug.Log("Artifacts: " + string.Join(" ", artifactCounter));

        GenerateBottom();
    }
    
    // TODO: Better mineral, lava, and stone distribution
    private int GetMineralNum(int y)
    {
        float mineralRoll = Random.Range(0f, 1f);

        int layer = Mathf.FloorToInt(y / ((float)(DEPTH / (minerals.Length - 3)) + 1));

        if (mineralRoll < (float)(mineralChance / 20)) { return layer + 3; }
        if (mineralRoll < (float)(mineralChance / 10)) { return layer + 2; }
        if (mineralRoll < (float)(mineralChance / 2)) { return layer + 1; }
        if (mineralRoll < mineralChance) { return layer; }

        return -1;
    }

    private void GenerateBottom()
    {
        Vector3Int[] topCoords = { new Vector3Int(7, -DEPTH - 3), new Vector3Int(WIDTH - 8, -DEPTH - 3) };
        Vector3Int[] bottomCoords = { new Vector3Int(7, -DEPTH - 11), new Vector3Int(19, -DEPTH - 11), new Vector3Int(31, -DEPTH - 11) };
        Vector3Int[] moreBottomCoords = { new Vector3Int(7, -DEPTH - 13), new Vector3Int(19, -DEPTH - 13), new Vector3Int(31, -DEPTH - 13) };

        tilemaps[1].SetTiles(GenerateLayerCoords(DEPTH + 2, 2), Enumerable.Repeat(miscGround[6].placeableTile, WIDTH * 2).ToArray());
        tilemaps[1].SetTiles(GenerateLayerCoords(DEPTH + 2, 2, 6, 15), Enumerable.Repeat(miscGround[8].placeableTile, 12).ToArray());
        tilemaps[2].SetTiles(topCoords, new TileBase[] { miscGround[4].placeableTile, miscGround[4].placeableTile });

        tilemaps[1].SetTiles(GenerateLayerCoords(DEPTH + 9, 2), Enumerable.Repeat(miscGround[8].placeableTile, WIDTH * 2).ToArray());
        tilemaps[0].SetTiles(GenerateLayerCoords(DEPTH + 9, 2), Enumerable.Repeat(background[0].placeableTile, WIDTH * 2).ToArray());
        tilemaps[1].SetTiles(GenerateLayerCoords(DEPTH + 11, 1), Enumerable.Repeat(miscGround[6].placeableTile, WIDTH).ToArray());
        tilemaps[2].SetTiles(bottomCoords, new TileBase[] { miscGround[7].placeableTile, miscGround[7].placeableTile, miscGround[7].placeableTile });
        tilemaps[2].SetTiles(moreBottomCoords, new TileBase[] { miscGround[7].placeableTile, miscGround[7].placeableTile, miscGround[7].placeableTile });
    }

    private Vector3Int[] GenerateLayerCoords(int baseDepth, int layers, int layerWidth = WIDTH, int xPos = 0)
    {
        Vector3Int[] coords = new Vector3Int[layerWidth * layers];

        for (int i = 0; i < coords.Length; ++i)
        {
            coords[i] = new(xPos + i % layerWidth, -baseDepth - Mathf.FloorToInt(i / layerWidth));
        }

        return coords;
    }

    public void Earthquake()
    {
        Debug.Log("Earthquake!!");

        for (int x = 0; x < WIDTH; x++)
        {
            if (!tilemaps[1].HasTile(new Vector3Int(x, -100)))
            {
                for (int y = 7; y < DEPTH && !tilemaps[1].HasTile(new Vector3Int(x, -y)); y++)
                {
                    for (int i = 0; i < earthquakeLayerAttempts; i++)
                    {
                        var random = x + Random.Range(-earthquakeLayerAttempts, earthquakeLayerAttempts);
                        var currentCoord = new Vector3Int(random > WIDTH ? random - WIDTH : random < 0 ? random + WIDTH : random, -y);
                        var pos = new Vector3Int(x, -y);
                        bool isAir = Random.Range(0f, 1f) < earthquakeAirChance;
                        for (int t = 0; t < tilemaps.Length; t++)
                        {
                            tilemaps[t].SetTile(pos, isAir ? t == 0 ? background[0].placeableTile : null : tilemaps[t].GetTile(currentCoord));
                        }
                    }
                }
            }
        }
    }
}
