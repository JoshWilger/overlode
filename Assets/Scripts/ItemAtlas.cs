using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Atlas", menuName = "Item Atlas")]
public class ItemAtlas : ScriptableObject
{
    [SerializeField] private ItemClass[] items;
    [SerializeField] private ItemClass[] unminables;
    [SerializeField] private ItemClass[] indestructables;

    public ItemClass[] CreateInstance( ItemClass.ItemType type, bool sort = true)
    {
        List<ItemClass> itemsByType = new();
        foreach (var item in items)
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

    public bool IsUnminable(string tileName)
    {
        return unminables.Where((unminableTile) => unminableTile.placeableTile.name == tileName).Any();
    }

    public bool IsIndestructable(string tileName)
    {
        return indestructables.Where((unminableTile) => unminableTile.placeableTile.name == tileName).Any();
    }
}
