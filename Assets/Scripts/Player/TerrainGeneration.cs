using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGeneration : MonoBehaviour
{
    [SerializeField] private TileBase[] background;
    [SerializeField] private TileBase[] ground;
    [SerializeField] private TileBase[] minerals;
    [SerializeField, Range(0, 1)] private float airChance;
    [SerializeField, Range(0, 1)] private float mineralChance;

    [SerializeField] private const int width = 33;
    [SerializeField] private const int depth = 98; // 599 is good actual value

    private Tilemap[] tilemaps;

    // Start is called before the first frame update
    void Start()
    {
        tilemaps = GetComponentsInChildren<Tilemap>();

        for (int y = 0; y < depth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int tilePos = new Vector3Int(x, -y - 2);
                tilemaps[0].SetTile(tilePos, background[0]);

                bool isAir = Random.Range(0f, 1f) < airChance;
                tilemaps[1].SetTile(tilePos,  isAir ? null : ground[Mathf.FloorToInt(y / ((float)(depth / ground.Length) + 1))]);

                if (!isAir)
                {
                    float mineralRoll = Random.Range(0f, 1f);
                    tilemaps[2].SetTile(tilePos, mineralRoll < mineralChance ? minerals[Mathf.FloorToInt(y / ((float)(depth / minerals.Length) + 1))] : null);
                }
            }
        }
    }
}
