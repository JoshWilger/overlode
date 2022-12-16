using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGeneration : MonoBehaviour
{
    [SerializeField] private TileBase[] background;
    [SerializeField] private TileBase[] ground;
    [SerializeField] private TileBase[] minerals;
    [SerializeField] private TileBase[] extras; // lava, stone, stoney dirts (2)
    [SerializeField] private TileBase[] artifacts;
    [SerializeField, Range(0, 1)] private float airChance;
    [SerializeField, Range(0, 1)] private float mineralChance;
    [SerializeField, Range(0, 1)] private float stoneLavaChance;
    [SerializeField, Range(0, 1)] private float artifactChance;

    [SerializeField] private int width = 33;
    [SerializeField] private int depth = 98; // 599 is good actual value

    private Tilemap[] tilemaps;

    // Start is called before the first frame update
    void Start()
    {
        tilemaps = GetComponentsInChildren<Tilemap>();
        int[] artifactCounter = new int[4];

        for (int y = 0; y < depth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int tilePos = new(x, -y - 2);
                int layerNum = Mathf.FloorToInt(y / ((float)(depth / Mathf.Min(ground.Length, minerals.Length)) + 1));
                bool isAir = Random.Range(0f, 1f) < airChance;
                float holyRolly = Random.Range(0f, 1f);
                float dungeonRoll = 1 - holyRolly;

                tilemaps[0].SetTile(tilePos, dungeonRoll < artifactChance && !isAir && (y > depth / 16) ? background[1] : background[0]);

                tilemaps[1].SetTile(tilePos,  isAir ? null : 
                    layerNum == 2 && holyRolly < 0.2f ? extras[2] : 
                    layerNum == 3 && holyRolly < 0.5f ? extras[3] : 
                    ground[layerNum]);

                if (!isAir)
                {
                    float stoneLavaRoll = Random.Range(0f, 1f);
                    Vector3Int oneAbove = new(tilePos.x, tilePos.y + 1);
                    int mineralNum = GetMineralNum(y);
                    int artifactNum = Mathf.FloorToInt((float)(holyRolly / artifactChance) * (artifacts.Length));

                    if ((y > depth / 2) && !tilemaps[1].GetTile(oneAbove) && (stoneLavaRoll < (2 * stoneLavaChance) * y / depth))
                    {
                        tilemaps[2].SetTile(oneAbove, extras[0]);
                    }

                    tilemaps[2].SetTile(tilePos, mineralNum == -1 ?
                        y > depth / 4 && stoneLavaRoll < stoneLavaChance * y / depth ? extras[1] 
                        : holyRolly < artifactChance && (y > depth / 16) ? artifacts[artifactNum] : null
                        : minerals[mineralNum]);

                    if (holyRolly < artifactChance && (y > depth / 16)) artifactCounter[artifactNum]++; // for logging the artifacts
                    if (dungeonRoll < artifactChance && !isAir && (y > depth / 16)) artifactCounter[3]++;
                }
            }
        }
        Debug.Log("Artifacts: " + string.Join(" ", artifactCounter));
    }
    
    // TODO: Better mineral distribution
    private int GetMineralNum(int y)
    {
        float mineralRoll = Random.Range(0f, 1f);

        int layer = Mathf.FloorToInt(y / ((float)(depth / (minerals.Length - 3)) + 1));

        if (mineralRoll < (float)(mineralChance / 16)) { return layer + 3; }
        if (mineralRoll < (float)(mineralChance / 8)) { return layer + 2; }
        if (mineralRoll < (float)(mineralChance / 2)) { return layer + 1; }
        if (mineralRoll < mineralChance) { return layer; }

        return -1;
    }
}
