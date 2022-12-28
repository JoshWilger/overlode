using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Toggle inventoryToggle;
    [SerializeField] private GameObject inventoryActive;
    [SerializeField] private GameObject selectedItem;
    [SerializeField] private GameObject focus;
    [SerializeField] private GameObject charging;
    [SerializeField] private GameObject factory;
    [SerializeField] private GameObject garage;
    [SerializeField] private GameObject shop;

    private ToggleGroup toggleGroup;
    private Toggle[] toggles;

    // Start is called before the first frame update
    private void Start()
    {
        toggleGroup = inventoryActive.GetComponent<ToggleGroup>();
        toggles = inventoryActive.GetComponentsInChildren<Toggle>();

        foreach (var item in toggles)
        {
            item.onValueChanged.AddListener(SelectItem);
        }
        inventoryToggle.onValueChanged.AddListener((value) =>
        {
            InventoryActive(value);
        });
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool collided = false;

        if (collision.gameObject.CompareTag("Charging"))
        {
            charging.SetActive(true);
            collided = true;
        }
        if (collision.gameObject.CompareTag("Factory"))
        {
            factory.SetActive(true);
            collided = true;
        }
        if (collision.gameObject.CompareTag("Garage"))
        {
            garage.SetActive(true);
            collided = true;
        }
        if (collision.gameObject.CompareTag("Shop"))
        {
            shop.SetActive(true);
            collided = true;
        }
        if (collided)
        {
            focus.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool collided = false;

        if (collision.gameObject.CompareTag("Charging"))
        {
            charging.SetActive(false);
            collided = true;
        }
        if (collision.gameObject.CompareTag("Factory"))
        {
            factory.SetActive(false);
            collided = true;
        }
        if (collision.gameObject.CompareTag("Garage"))
        {
            garage.SetActive(false);
            collided = true;
        }
        if (collision.gameObject.CompareTag("Shop"))
        {
            shop.SetActive(false);
            collided = true;
        }
        if (collided)
        {
            focus.SetActive(false);
        }
    }

    private void InventoryActive(bool toggledOn)
    {
        inventoryActive.SetActive(toggledOn);
        selectedItem.SetActive(!toggledOn);
    }

    public TextMeshProUGUI[] RetrieveInventoryText(ItemClass.ItemType type)
    {
        TextMeshProUGUI[] children = inventoryActive.GetComponentsInChildren<TextMeshProUGUI>();
        int amount = type == ItemClass.ItemType.mineral ? children.Length - 6 : type == ItemClass.ItemType.shopItem ? 6 : children.Length;
        TextMeshProUGUI[] inventoryTexts = new TextMeshProUGUI[amount];

        for (int i = 0; i < amount; i++)
        {
            if (type == ItemClass.ItemType.mineral)
            {
                inventoryTexts[i] = children[i + 6];
            }
            else
            {
                inventoryTexts[i] = children[i];
            }
        }

        return inventoryTexts;
    }

    private void SelectItem(bool value)
    {
        if (value)
        {
            var image = selectedItem.GetComponentInChildren<Image>();
            var image3 = image.GetComponentInChildren<Image>();

            var activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();
            var image2 = activeToggle.GetComponentInChildren<Image>();

            Debug.Log(image3.name);
            Debug.Log(image2.name);
        }
    }
}
