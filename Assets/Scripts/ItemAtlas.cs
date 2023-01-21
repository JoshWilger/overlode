using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Atlas", menuName = "Item Atlas")]
public class ItemAtlas : ScriptableObject
{
    [SerializeField] private ItemClass[] items;
    [SerializeField] private ItemClass[] unminables;
    [SerializeField] private ItemClass[] indestructables;
    [SerializeField] private ItemClass[] upgrades;

    public float[] currentUpgradeAmounts = { 10f, 20f, 7f, 10f, 150f, 0f };

    public enum UpgradeTypes
    {
        battery,
        drill,
        storage,
        health,
        jetpack,
        cooling
    }

    public ItemClass[] CreateInstance(ItemClass.ItemType type, bool sort = true)
    {
        List<ItemClass> itemsByType = new();

        foreach (var item in (type == ItemClass.ItemType.upgrade || type == ItemClass.ItemType.upgradeType) ? upgrades : items)
        {
            if (item.itemType == type)
            {
                itemsByType.Add(item);
            }
        }

        if (sort)
        {
            itemsByType.Sort((x, y) => x.itemWorth.CompareTo(y.itemWorth));
        }

        return itemsByType.ToArray();
    }

    public ItemClass[][] CreateMultiInstance(ItemClass.ItemType type, bool sort = true)
    {
        ItemClass[] upgradeItemTypes = CreateInstance(ItemClass.ItemType.upgradeType);
        List<ItemClass>[] itemsByType = new List<ItemClass>[type == ItemClass.ItemType.upgrade ? upgradeItemTypes.Length : 1];
        for (int i = 0; i < itemsByType.Length; i++)
        {
            itemsByType[i] = new();
        }

        foreach (var item in (type == ItemClass.ItemType.upgrade || type == ItemClass.ItemType.upgradeType) ? upgrades : items)
        {
            if ((type == ItemClass.ItemType.upgrade || type == ItemClass.ItemType.upgradeType) && item.itemType == ItemClass.ItemType.upgrade)
            {
                itemsByType[Array.IndexOf(upgradeItemTypes, item.upgradeType)].Add(item);
            }
            else if (type != ItemClass.ItemType.upgrade && type != ItemClass.ItemType.upgradeType && item.itemType == type)
            {
                itemsByType[0].Add(item);
            }
        }
        if (sort)
        {
            foreach (var item in itemsByType)
            {
                item.Sort((x, y) => x.itemWorth.CompareTo(y.itemWorth));
            }
        }

        return itemsByType.Select(a => a.ToArray()).ToArray();
    }

    public bool IsUnminable(string tileName)
    {
        return unminables.Where((unminableTile) => unminableTile.placeableTile.name == tileName).Any();
    }

    public bool IsIndestructable(string tileName)
    {
        return indestructables.Where((unminableTile) => unminableTile.placeableTile.name == tileName).Any();
    }

    public long GroundWorth(TileBase tile)
    {
        foreach (var item in items)
        {
            if (item.placeableTile == tile)
            {
                return item.itemType == ItemClass.ItemType.shopItem ? item.itemWorth / 100 : item.itemWorth;
            }
        }

        Debug.Log("Tile not found!");

        return -1;
    }
}
