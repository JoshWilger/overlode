using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

[CreateAssetMenu(fileName = "New Item", menuName = "Item Class")]
public class ItemClass : ScriptableObject
{
    public string itemName;
    public TileBase placeableTile;
    public ItemType itemType;
    public long itemWorth;
    public int amountCollected = 0;

    public enum ItemType
    {
        mineral,
        ground,
        artifact,
        background,
        miscGround
    };
}
