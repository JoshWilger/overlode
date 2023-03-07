using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float minPosition;
    [SerializeField] private float maxPosition;
    [SerializeField] private float creditsScrollSpeed;
    [SerializeField] private float shakeAmount;

    public bool credits = false;
    public bool shake = false;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (credits && transform.position.y < 9f)
        {
            var amount = transform.position.y > 0f ? Mathf.Abs(transform.position.y - 9f) / 100f : Mathf.Abs(transform.position.y + 0.1f - player.position.y) / 100f;
            transform.position = new Vector3(transform.position.x, transform.position.y + (amount < creditsScrollSpeed ? amount : creditsScrollSpeed), transform.position.z);
        }
        else if (!credits)
        {
            transform.position = new Vector3(Mathf.Clamp(player.position.x, minPosition, maxPosition), player.position.y + (shake ? shakeAmount : 0), transform.position.z);
            if (shake)
            {
                shakeAmount = -shakeAmount;
                Debug.Log("shaking");

            }
        }
    }
}
