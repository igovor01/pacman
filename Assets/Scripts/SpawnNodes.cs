using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNodes : MonoBehaviour
{
    int numToSpawn = 28;

    public float currentSpawnOffset;
    public float spanOffset = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.name == "Node")
        {
            currentSpawnOffset = spanOffset;
            for(int i=0; i < numToSpawn; i++)
            {
                //clone a new node
                GameObject clone = Instantiate(gameObject, new Vector3(transform.position.x, transform.position.y + currentSpawnOffset, 0), Quaternion.identity);
                currentSpawnOffset += spanOffset;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
