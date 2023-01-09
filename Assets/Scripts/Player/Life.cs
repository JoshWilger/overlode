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
    private Mining MiningScript;
    private bool dead;

    [SerializeField] private Tilemap mineralTilemap;

    // Start is called before the first frame update
    private void Start()
    {
        dead = false;
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        movementScript = GetComponent<Movement>();
        MiningScript = GetComponent<Mining>();
    }

    private void Update()
    {
        int playerX = Mathf.FloorToInt(coll.bounds.center.x);
        int playerY = Mathf.FloorToInt(coll.bounds.center.y);

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
        MiningScript.enabled = false;
        anim.SetTrigger("death");
        Debug.Log("Dying...");
        Invoke(nameof(RestartLevel), 1.1f);
    }

    private void RestartLevel()
    {
        Debug.Log("Restarting...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        movementScript.enabled = true;
        MiningScript.enabled = true;
        dead = false;
    }
}
