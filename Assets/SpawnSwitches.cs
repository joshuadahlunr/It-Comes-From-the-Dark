using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSwitches : MonoBehaviour
{
	public static SpawnSwitches inst; // Singleton object

    public GameObject[] SpawnPoints;
    public GameObject switchPrefab;

    public GameObject lights;

    public AudioSource switchSource;
    public AudioSource offSource;

    public int numSwitches = 5;
    public float timeOn = 30;

    public bool on = true;

	void Awake(){
		inst = this; // Setup singleton
	}

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numSwitches; i++)
        {
            Spawn();
        }
    }

    public List<int> spawnedSwitches = new List<int>();

    // Update is called once per frame
    void Update()
    {
		// Only do light logic whil the lights are on
		if(on) LightLogic();

		foreach(int index in spawnedSwitches){
			SpawnPoints[index].transform.GetChild(1).GetComponent<Lamp>().flicker();
		}
    }

	// This function preforms all of the needed logic when a switch is activated
	public void Acivated(Transform hit){
		on = true;
		foreach (Lamp lamp in lights.GetComponentsInChildren<Lamp>())
			lamp.setEnabled(true);
        switchSource.Play();
		// Add batteries to the level to replace the ones the player picked up in the last cycle
		BatterySpawn.inst.Respawn();
		// Rearange the doors
		DoorSpawner.inst.Respawn();
	}

	// Function which determines if the provided switch spawn point has line of sight to any other spawned switch
	bool inLineofSightofAny(int spawnIndex){
		RaycastHit hit;
		foreach(int i in spawnedSwitches){
			// Cast a ray from the spawn location in the direction of each other spawn point
			Vector3 direction = SpawnPoints[i].transform.position - SpawnPoints[spawnIndex].transform.position; direction.z = 0; // Make sure the direction has no vertical component
			if(Physics.Raycast(SpawnPoints[spawnIndex].transform.position + /*Actual height of switch*/new Vector3(0, 1.368f, 0), direction, out hit, 20)){
				Debug.DrawLine(SpawnPoints[spawnIndex].transform.position + /*Actual height of switch*/new Vector3(0, 1.368f, 0), hit.point, Color.red, 30);
				// If it hits a switch, then we are in line of sight
				if(hit.transform.tag == "Switch")
					return true;
			}
		}
		// If we didn't hit any swicthes then we are not in line of sight
		return false;
	}

    void Spawn()
    {
        int spawnIndex = Random.Range(0, SpawnPoints.Length); // generate random array number
		// Make sure the random number hasn't already been choosen (and that it isn't in line of sight with another already spawned switch)
        while(spawnedSwitches.Contains(spawnIndex) || inLineofSightofAny(spawnIndex)) spawnIndex = Random.Range(0, SpawnPoints.Length);
        spawnedSwitches.Add(spawnIndex);

		// Update the spawn point to now reference the instantiated switch!
        SpawnPoints[spawnIndex] = Instantiate(switchPrefab, SpawnPoints[spawnIndex].transform.position, Quaternion.identity, SpawnPoints[spawnIndex].transform);
		SpawnPoints[spawnIndex].transform.localRotation = SpawnPoints[spawnIndex].transform.rotation;
		// NOTE: In order for the lights to be correctly oriented the blue local vector needs to be facing right along the wall, and from top orthographic view the point needs to be lined up with the outermost line of the wall.
    }

    float timer = 0; // TODO... we have a game timer score, we could use that instead?
    void LightLogic()
    {
		timer += Time.deltaTime;

		// If the specified time has passed turn off the lights
		if(timer > timeOn){
			on = false;
			foreach (Lamp lamp in lights.GetComponentsInChildren<Lamp>())
				lamp.setEnabled(false);
            offSource.Play();
			timer = 0;
		}
    }
}
