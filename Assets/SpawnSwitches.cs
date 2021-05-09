using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSwitches : MonoBehaviour
{
	public static SpawnSwitches inst; // Singleton object

	// List of possible locations switches can spawn in
    public GameObject[] SpawnPoints;
	// Prefab representing the switches which we instantiate
    public GameObject switchPrefab;

	// The object containing all of the lights
    public GameObject lights;

	// Audio sources
    public AudioSource switchSource;
    public AudioSource offSource;

	// The number of switches to spawn
    public int numSwitches = 5;
	// The time lights will remain on
    public float timeOn = 30;
	public float minTimeOn = 10;

	// Variable representing if the lights are on or off
    public bool on = true;

	// List of indexes in SpawnPoints that represent spawned switches
	public List<int> spawnedSwitches = new List<int>();

	// Variable tracking how many switches must be activated before a switch can be used again.
	public int switchesBetweenUse = 3;
	// Queue tracking the switches that have been recently activated
	public Queue<Transform> activatedSwitches = new Queue<Transform>();

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



    // Update is called once per frame
    void Update()
    {
		// Only do light logic whil the lights are on
		if(on) LightLogic();

		// Flicker active lights
		foreach(int index in spawnedSwitches){
			// If the light is still activateable, flicker the light over it
			if(!activatedSwitches.Contains(SpawnPoints[index].transform.GetChild(0).transform)) // child 0 is switch model
				SpawnPoints[index].transform.GetChild(1).GetComponent<Lamp>().flicker(); // child 1 is lamp model
			// If the light is not activatedable, then make sure the lights state is synced with the rest of the lights
			else
				SpawnPoints[index].transform.GetChild(1).GetComponent<Lamp>().stopFlicker(); // child 1 is lamp model
		}
    }

	// This function preforms all of the needed logic when a switch is activated
	public void Acivated(Transform hit){
		if(activatedSwitches.Contains(hit)) return; // If the switch has already been used... then skip the rest of the function

		on = true;
		foreach (Lamp lamp in lights.GetComponentsInChildren<Lamp>())
			lamp.setEnabled(true);
        switchSource.Play();
		// Add batteries to the level to replace the ones the player picked up in the last cycle
		BatterySpawn.inst.Respawn();
		// Rearange the doors
		DoorSpawner.inst.Respawn();
		// Each time a switch gets flipped, the difficulty increases (you can flip switches while the lights are on to make the game even harder)
		GameplayManager.inst.increaseDifficulty();

		// Add the current switch to the queue of used switches
		activatedSwitches.Enqueue(hit);
		// If there are more switches in the queue than the maximum, start removing switches from the queue.
		while(activatedSwitches.Count > switchesBetweenUse) activatedSwitches.Dequeue();
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
