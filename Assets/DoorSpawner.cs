using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSpawner : MonoBehaviour
{
	public static DoorSpawner inst;
	void Awake() { inst = this; }

	public GameObject doorPrefab;

    // Start is called before the first frame update
    void Start()
    {
		Respawn();
    }

	// Randomly spawn doors
	public void Respawn(){
		foreach(Transform child in transform){
			// If the doorframes have any children (doors) delete them all
			if(child.childCount > 0)
				foreach(Transform childchild in child)
					Destroy(childchild.gameObject);

			Spawn(child);
		}
	}

	// Function which spawns a door with a percentage chance in each doorframe
	static Vector3[] positions = {new Vector3(1.4f, 0, -.42f), new Vector3(-1.4f, 0, -.06f)};
	void Spawn(Transform target){
		// 1 in 5 chance
		int randomChance = Random.Range(0, 10);
		if(randomChance < 2){
			// Doors can't spawn within 3 meters of switches (hopefull this means there will always be a path to each switch!)
			Collider[] thingsInBounds = Physics.OverlapSphere(target.position, 3f);
			foreach(Collider thing in thingsInBounds)
				if(thing.tag == "Switch")
					return; // Don't spawn doors close to switches (helps prevent blocking off switch rooms)

			// Instantiate a door as a child of this object, at the given position with a random rotation
			GameObject door = Instantiate(doorPrefab, target.position, Quaternion.LookRotation(target.forward * (randomChance * 2 - 1), Vector3.up), target);
			door.transform.localScale = new Vector3(1.1f, 1, 1);
			door.transform.localPosition += positions[randomChance];
		}
	}
}
