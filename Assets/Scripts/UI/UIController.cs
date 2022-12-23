using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    private void Start()
    {
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
}
