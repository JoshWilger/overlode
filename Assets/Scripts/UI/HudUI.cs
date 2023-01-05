using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudUI : MonoBehaviour
{
    [SerializeField] private Toggle inventoryToggle;
    [SerializeField] private GameObject inventoryActive;
    [SerializeField] private GameObject selectedItem;
    [SerializeField] private GameObject focus;

    private ToggleGroup toggleGroup;
    private Toggle[] toggles;
    public Toggle activeToggle;

    // Start is called before the first frame update
    void Start()
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
    void Update()
    {
        if (!enabled) return;

        InventoryKeyPressed(Input.GetKeyDown(KeyCode.E));

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
    private void InventoryKeyPressed(bool isPressed)
    {
        if (isPressed)
        {
            inventoryToggle.isOn = !inventoryToggle.isOn;
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
