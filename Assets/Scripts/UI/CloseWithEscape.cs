using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWithEscape : MonoBehaviour
{
    [SerializeField] private GameObject focus;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            focus.SetActive(false);
            gameObject.SetActive(false);
        } 
    }
}
