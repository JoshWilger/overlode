using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float minPosition;
    [SerializeField] private float maxPosition;

    // Update is called once per frame
    private void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(player.position.x, minPosition, maxPosition), player.position.y, transform.position.z);
    }
}
