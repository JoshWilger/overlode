using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Tile", menuName = "Item Class")]
public class ItemClass : ScriptableObject
{
    public string itemName;
    public TileBase placeableTile;
    public ItemType itemType;
    public long itemWorth;

    public enum ItemType
    {
        mineral,
        ground,
        artifact,
        background,
        miscGround
    };
}
