using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Item", menuName = "Item Class")]
public class ItemClass : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public TileBase placeableTile;
    public ItemType itemType;
    public ItemClass upgradeType;
    public long itemWorth;
    public int amountCollected = 0;

    public enum ItemType
    {
        mineral,
        ground,
        artifact,
        background,
        miscGround,
        shopItem,
        upgrade,
        upgradeType
    };
}
