using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Life : MonoBehaviour
{
    [SerializeField] private HudUI hudUiScript;

    private Animator anim;
    private BoxCollider2D coll;
    private Movement movementScript;
    private Mining miningScript;
    private Energy energyScript;
    private Health healthScript;
    private bool dead;

    [SerializeField] private Tilemap mineralTilemap;

    // Start is called before the first frame update
    private void Start()
    {
        dead = false;
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        movementScript = GetComponent<Movement>();
        miningScript = GetComponent<Mining>();
        energyScript = GetComponent<Energy>();
        healthScript = GetComponent<Health>();
    }

    private void Update()
    {
        if (energyScript.energy <= 0 && !dead)
        {
            Die();
        }
        else if (healthScript.health <= 0 && !dead)
        {
            Die();
        }
    }

    private void Die()
    {
        dead = true;
        movementScript.enabled = false;
        miningScript.enabled = false;
        energyScript.enabled = false;
        hudUiScript.enabled = false;
        anim.SetTrigger("death");
        Debug.Log("Dying...");
        Invoke(nameof(RestartLevel), 1.1f);
    }

    private void RestartLevel()
    {
        Debug.Log("Restarting...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        movementScript.enabled = true;
        miningScript.enabled = true;
        energyScript.enabled = true;
        hudUiScript.enabled = true;
        dead = false;
    }
}
