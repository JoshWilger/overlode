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
    private Toggle activeToggle;

    // Start is called before the first frame update
    private void Start()
    {
        toggleGroup = inventoryActive.GetComponent<ToggleGroup>();
        toggles = inventoryActive.GetComponentsInChildren<Toggle>();
        activeToggle = toggles[0];

        foreach (var item in toggles)
        {
            item.onValueChanged.AddListener(SelectedItem);
        }
        inventoryToggle.onValueChanged.AddListener((value) =>
        {
            InventoryActive(value);
        });
    }

    // Update is called once per frame
    private void Update()
    {


        if (!enabled) return;

        Vector2 mouseScrolling = Input.mouseScrollDelta;

        if (mouseScrolling.y > 0)
        {
            ChangeActiveToggle(-1);
        }
        if (mouseScrolling.y < 0)
        {
            ChangeActiveToggle(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool collided = false;

        if (collision.gameObject.CompareTag("Charging"))
        {
            charging.SetActive(true);
            collided = true;
        }
        else if (collision.gameObject.CompareTag("Factory"))
        {
            factory.SetActive(true);
            collided = true;
        }
        else if (collision.gameObject.CompareTag("Garage"))
        {
            garage.SetActive(true);
            collided = true;
        }
        else if (collision.gameObject.CompareTag("Shop"))
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
        else if (collision.gameObject.CompareTag("Factory"))
        {
            factory.SetActive(false);
            collided = true;
        }
        else if (collision.gameObject.CompareTag("Garage"))
        {
            garage.SetActive(false);
            collided = true;
        }
        else if (collision.gameObject.CompareTag("Shop"))
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

    private void SelectedItem(bool value)
    {
        if (value)
        {
            var image = selectedItem.GetComponentsInChildren<Image>().Last();
            var rect = image.GetComponent<RectTransform>();

            var maybeActiveToggle = toggleGroup.ActiveToggles().FirstOrDefault();
            if (maybeActiveToggle)
            {
                activeToggle = maybeActiveToggle;
            }
            var image2 = activeToggle.GetComponentInChildren<Image>();

            image.sprite = image2.sprite;

            var text = selectedItem.GetComponentInChildren<TextMeshProUGUI>();
            var text2 = activeToggle.GetComponentInChildren<TextMeshProUGUI>();

            text.text = text2.text;
            rect.localScale = new Vector3(0.8f, 0.8f);
        }
    }

    private void ChangeActiveToggle(int relativeIndex)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                int adder = i + relativeIndex;
                int newActiveIndex = adder < 0 ? adder + toggles.Length : adder > toggles.Length - 1 ? adder - toggles.Length : adder;

                toggles[newActiveIndex].isOn = true;
                toggles[i].isOn = false;

                activeToggle = toggles[newActiveIndex];
                toggleGroup.EnsureValidState();

                SelectedItem(true);

                return;
            }
        }
    }
}
