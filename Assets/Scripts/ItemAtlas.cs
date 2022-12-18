using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Atlas", menuName = "Item Atlas")]
public class ItemAtlas : ScriptableObject
{
    [SerializeField] private ItemClass[] items;
     
    public ItemClass[] CreateInstance( ItemClass.ItemType type)
    {
        List<ItemClass> itemsByType = new();
        foreach (var item in items)
        {
            if (item.itemType == type)
            {
                itemsByType.Add(item);
            }
        }
        itemsByType.Sort((x, y) => x.itemWorth.CompareTo(y.itemWorth));

        return itemsByType.ToArray();
    }
}


