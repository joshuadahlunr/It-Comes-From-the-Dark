using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSwitches : MonoBehaviour
{
    public Transform[] SpawnPoints;
    public int numSwitches = 5;
    public GameObject Switches;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numSwitches; i++)
        {
            Spawn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Spawn()
    {
        int spawnIndex = Random.Range(0, SpawnPoints.Length); // generate random array number
        Instantiate(Switches, SpawnPoints[spawnIndex].position, SpawnPoints[spawnIndex].rotation);
    }
}
