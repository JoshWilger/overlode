using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityAndDestroy : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new(Random.Range(-5f, 5f), Random.Range(5f, 15f));
        Invoke(nameof(DestroyMe), 2);
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
