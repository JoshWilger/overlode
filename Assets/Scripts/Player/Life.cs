using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Life : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D coll;
    private Movement movementScript;
    private ItemCollector itemCollectorScript;
    private bool dead;

    [SerializeField] private Tilemap mineralTilemap;

    // Start is called before the first frame update
    private void Start()
    {
        dead = false;
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        movementScript = GetComponent<Movement>();
        itemCollectorScript = GetComponent<ItemCollector>();
    }

    private void Update()
    {
        int playerX = (int)Math.Floor(coll.bounds.center.x);
        int playerY = (int)Math.Floor(coll.bounds.center.y);

        TileBase mineral = mineralTilemap.GetTile(new Vector3Int(playerX, playerY));

        if (mineral && !dead)
        {
            if (mineral.name == "pxy-lava_2a")
            {
                Die();
            }
        }
    }

    private void Die()
    {
        dead = true;
        movementScript.enabled = false;
        itemCollectorScript.enabled = false;
        anim.SetTrigger("death");
        Debug.Log("Dying...");
        Invoke(nameof(RestartLevel), 1.1f);
    }

    private void RestartLevel()
    {
        Debug.Log("Restarting...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        movementScript.enabled = true;
        itemCollectorScript.enabled = true;
        dead = false;
    }
}
