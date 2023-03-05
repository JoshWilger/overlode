using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScrollingObjects : MonoBehaviour
{
    [SerializeField] private GameObject spawnMe;
    [SerializeField] private int spawnLayerAttempts;
    [SerializeField, Range(0, 1)] private float spawnChance;

    private List<GameObject> objects;
    private int highestY;

    // Start is called before the first frame update
    private void Awake()
    {
        objects = new();
        highestY = Mathf.FloorToInt(spawnMe.transform.position.y);
    }

    // Update is called once per frame
    private void Update()
    {
        var rightHeight = Mathf.FloorToInt(transform.position.y + 10);
        if (rightHeight > highestY && objects.Count < 100)
        {
            var diff = rightHeight - highestY;
            for (int i = 0; i < diff; i++)
            {
                for (int a = 0; a < spawnLayerAttempts; a++)
                {
                    float rand = Random.value;

                    if (rand < spawnChance)
                    {
                        var newObject = Instantiate(spawnMe);
                        newObject.transform.position = new Vector3(newObject.transform.position.x, highestY + i, newObject.transform.position.z);
                        objects.Add(newObject);
                    }
                }
            }
            highestY += diff;
            Debug.Log(highestY);
        }
        else if (rightHeight < spawnMe.transform.position.y && objects.Count > 0)
        {
            Debug.Log($"Destroying {objects.Count} {spawnMe.name}");
            foreach (var item in objects)
            {
                Destroy(item);
            }
            objects.Clear();
            highestY = Mathf.FloorToInt(spawnMe.transform.position.y);
        }
    }
}
