using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClutterSpawner : MonoBehaviour
{
	public int spawnCount = 50;
	public List<GameObject> prefabs;

    // Start is called before the first frame update
    void Start()
    {
        Respawn();
    }

    // Makes sure that there are at least 50 clutter pieces on the map.
  	void Respawn(){
  		for(int i = 0; i < spawnCount; i++){
  			Vector3 spawnPoint = Utility.randomNavmeshLocation(new Vector3(20, 0, 20), 40);
  			// Make sure the spawn point isn't near any other battery
  			while(inLineofSightofAny(spawnPoint)) spawnPoint = Utility.randomNavmeshLocation(new Vector3(20, 0, 20), 40);
  			spawnPoint.y = .3f; // Lift up the spawn point so that the battery will fall down due to gravity

			Debug.Log(spawnPoint);

			GameObject toSpawn = prefabs[Random.Range(0, prefabs.Count)];
			Debug.Log(toSpawn);

  			// Instantiate a battery as a child of this object, at the given position with a random rotation
  			Instantiate(toSpawn, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
  		}
  	}

  	// Function which determines if the provided switch spawn point has line of sight to any other spawned switch
  	bool inLineofSightofAny(Vector3 position){
  		RaycastHit hit;
  		foreach (Transform child in transform)
  			// Cast a ray from the spawn location in the direction of each other spawn point
  			if(Physics.Raycast(position, child.position - position, out hit, 8)){
  				// If it hits a switch, then we are in line of sight
  				if(hit.transform.tag == "Clutter")
  					return true;
  			}
  		// If we didn't hit any swicthes then we are not in line of sight
  		return false;
  	}
}
