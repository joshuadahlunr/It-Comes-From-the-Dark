using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSwitches : MonoBehaviour
{
	public static SpawnSwitches inst; // Singleton object

    public GameObject[] SpawnPoints;
    public GameObject Switches;
    public GameObject SwitchLight;

    public GameObject lights;

    public AudioSource switchSource;
    public AudioSource offSource;

    public int numSwitches = 5;
    public float timeOn = 30;

    public bool on = true;

    //SphereCollider switchCollider;

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
		foreach(int i in spawnedSwitches)
			// Cast a ray from the spawn location in the direction of each other spawn point
			if(Physics.Raycast(SpawnPoints[spawnIndex].transform.position, SpawnPoints[i].transform.position - SpawnPoints[spawnIndex].transform.position, out hit, 20)){
				// If it hits a switch, then we are in line of sight
				if(hit.transform.tag == "Switch")
					return true;
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
        SpawnPoints[spawnIndex] = Instantiate(Switches, SpawnPoints[spawnIndex].transform.position, SpawnPoints[spawnIndex].transform.rotation, SpawnPoints[spawnIndex].transform);
        SpawnPoints[spawnIndex] = Instantiate(SwitchLight, SpawnPoints[spawnIndex].transform.position, SpawnPoints[spawnIndex].transform.rotation, SpawnPoints[spawnIndex].transform);
		// TODO: This isn't currently used... but it will probably be usefull with a better solution to the "is the player interacting with switch" problem.
		SphereCollider switchCollider = SpawnPoints[spawnIndex].AddComponent<SphereCollider>() as SphereCollider; // create appropriate collider
        switchCollider.radius = .2f; // make the collider larger so player can interact
		switchCollider.isTrigger = true; // make the collider not collide but just register that the player is inside of it
		switchCollider.tag = "Switch"; // mark the collider as a switch
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
