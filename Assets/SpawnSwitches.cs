using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSwitches : MonoBehaviour
{
	public static SpawnSwitches inst; // Singleton object

    public GameObject[] SpawnPoints;
    public GameObject Switches;

    public GameObject lights;

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

    List<int> spawnedSwitches = new List<int>();

    // Update is called once per frame
    void Update()
    {
		// Only do light logic whil the lights are on
		if(on) LightLogic();

		// For each switch check if the player is interacting with it!
		if (Input.GetKey(KeyCode.E)){
	        for (int i = 0; i < numSwitches; i++){
				// TODO: This isn't a great solution... but I'm tired (it can be made a lot better!)
				Collider[] thingsInBounds = Physics.OverlapSphere(SpawnPoints[spawnedSwitches[i]].transform.position, 1);
				foreach(Collider thing in thingsInBounds){
					if(thing.tag == "Player"){
						on = true;
						lights.SetActive(true);
					}
				}
			}
		}
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
			lights.SetActive(false);
			timer = 0;
		}
    }
}
