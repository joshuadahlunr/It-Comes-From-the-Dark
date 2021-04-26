using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSwitches : MonoBehaviour
{
    public GameObject[] SpawnPoints;
    public GameObject Switches;

    public Light[] lights;

    public int numSwitches = 5;
    public float timeOn = 30;

    private bool on;

    SphereCollider switchCollider;

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
        LightLogic();
    }

    void Spawn()
    {
        int spawnIndex = Random.Range(0, SpawnPoints.Length); // generate random array number
        Instantiate(Switches, SpawnPoints[spawnIndex].transform.position, SpawnPoints[spawnIndex].transform.rotation);
        switchCollider = SpawnPoints[spawnIndex].GetComponent<SphereCollider>(); // get appropriate collider
        switchCollider.radius = 1; // make the collider larger so player can interact

        if (switchCollider.tag == "Player" && Input.GetKeyDown(KeyCode.E)) // should be able to activate switch if in range
        {
            on = !on;
            lights[spawnIndex].enabled = on;
        }
    }

    void LightLogic()
    {
        // to do later, after certain amt of time has passed turn off light
    }
}
