using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySpawn : MonoBehaviour
{
	// Singleton object
	public static BatterySpawn inst;

	public GameObject batteryPrefab;
	public AudioSource pickupSource;

	// Variable tracking the number of batteries that should be put in the level in each spawn sequence.
	public int targetBatteryCount = 30;

	void Awake(){
		inst = this; // Setup the singleton object
	}

    // Start is called before the first frame update
    void Start()
    {
		// Respawn all of the lights
		Respawn();
    }

	void Update(){
		// For each switch check if the player is interacting with it!
		if (Input.GetKeyUp(KeyCode.E)){
			foreach (Transform child in transform){
				// TODO: This isn't a great solution... but I'm tired (it can be made a lot better!)
				Collider[] thingsInBounds = Physics.OverlapSphere(child.position, 1);
				foreach(Collider thing in thingsInBounds){
					// If the player is close to the battery and pressing E
					if(thing.tag == "Player"){
						// If we aren't full on battery charge, destroy the battery so we can't pick it up again
						if(CharacterControl.inst.batteryCharge < 99.99999f){
							Destroy(child.gameObject);
							pickupSource.Play(); // Also play a sound to indicate that the battery was picked up
						}
						// Add one to the battery charge (making sure its not greater than the maximum)
						CharacterControl.inst.batteryCharge = Mathf.Clamp(CharacterControl.inst.batteryCharge + 1, 0, 99.99999f);
					}
				}
			}
		}
	}

	// Makes sure that there are at least 30 batteries on the map.
	public void Respawn(){
		int need2spawn = targetBatteryCount - transform.childCount;
		for(int i = 0; i < need2spawn; i++){
			Vector3 spawnPoint = Utility.randomNavmeshLocation(new Vector3(20, 0, 20), 40);
			// Make sure the spawn point isn't near any other battery
			while(inLineofSightofAny(spawnPoint)) spawnPoint = Utility.randomNavmeshLocation(new Vector3(20, 0, 20), 40);
			spawnPoint.y = 2; // Lift up the spawn point so that the battery will fall down due to gravity

			// Instantiate a battery as a child of this object, at the given position with a random rotation
			Instantiate(batteryPrefab, spawnPoint, Quaternion.Euler(Random.Range(0, 360), 0, -30), transform);
		}
	}

	// Function which determines if the provided switch spawn point has line of sight to any other spawned switch
	bool inLineofSightofAny(Vector3 position){
		RaycastHit hit;
		foreach (Transform child in transform)
			// Cast a ray from the spawn location in the direction of each other spawn point
			if(Physics.Raycast(position, child.position - position, out hit, 8)){
				// If it hits a switch, then we are in line of sight
				if(hit.transform.tag == "Battery")
					return true;
			}
		// If we didn't hit any swicthes then we are not in line of sight
		return false;
	}
}
