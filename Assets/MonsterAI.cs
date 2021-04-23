using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
	// Variable referencing the player
	public Rigidbody player;
	// Reference to the monster's movement script.
	MonsterMove movement;
	// Reference to the monster's audio manager
	public MonsterAudioManager audioManager;
	// Debug marker representing the player's predicted position
	//public GameObject debugPlayerPredictedPositionIndicator;


	// Range of values randomly determining how long it takes for the monster to update the player's predicted position
	public Vector2 playerLocationUpdateIntervalRange = new Vector2(4, 7);
	// How long it should be between player location updates (when the monster doesn't have line of sight)
	float playerLocationUpdateInterval;
	// How long it should be between checks for if the player is in line of sight
	public float physicsCheckUpdateInterval = .25f;

	// When the object is created get a reference to the movement script
	void Awake() {
		movement = GetComponent<MonsterMove>();
		playerLocationUpdateInterval = Random.Range(playerLocationUpdateIntervalRange.x, playerLocationUpdateIntervalRange.y);
	}


	// Variables tracking the player and monster's location between frames for velocity calculations. (Some of the custom movement code breaks the built in velocity calculations)
	Vector3 playerPositionLastFrame, monsterPositionLastFrame;
	// Timers tracking when locations and line of sight checks should be updated
	float timeSinceLocationUpdate = float.MaxValue, timeSinceLastPhysicsCheck = float.MaxValue;
	// Raycast catcher.
	RaycastHit hit;

    // Update is called once per frame
    void Update()
    {
		// Update timers
		timeSinceLocationUpdate += Time.deltaTime;
		timeSinceLastPhysicsCheck += Time.deltaTime;
		// Mark that the player is not in line of sight
		bool playerInLineofSight = false;

		// Check if enouph time has passed to check if the player is in line of sight
		if(timeSinceLastPhysicsCheck > physicsCheckUpdateInterval){
			timeSinceLastPhysicsCheck = 0;
			// Raycast to the player
			if (Physics.Raycast (transform.position, (player.transform.position + .5f * Vector3.up) - transform.position, out hit))
				// Check if what we hit was the player.
				playerInLineofSight = hit.transform.name == player.transform.name; // TODO: why do we have to check the transform's names, why can't we just check the transforms?

			// If the player is in line of sight start fading in whispers
			if(playerInLineofSight) audioManager.fadeInWhispers();
			// Otherwise start fading out whispers
			else audioManager.fadeOutWhispers();
		}

		// If the player is in line of sight or if enouph time has passed, update the player's predicted position
		if(playerInLineofSight || timeSinceLocationUpdate >= playerLocationUpdateInterval){
			timeSinceLocationUpdate = 0;
			// Randomly determine how long it will take for this check to run again
			playerLocationUpdateInterval = Random.Range(playerLocationUpdateIntervalRange.x, playerLocationUpdateIntervalRange.y);

			// Play an audio cue to alert the player that their location has been tracked.
			audioManager.playSigh();

			// Calculate the player and relative velocity.
			Vector3 playerVelocity = player.transform.position - playerPositionLastFrame;
			Vector3 relativeVelocity = playerVelocity - /*monsterVelocity*/(transform.position - monsterPositionLastFrame);

			// Calculate the time it will take for the monster to catch up to the player (ignoring obstacles)
			float relativeTime = /*relativeDistance*/(player.transform.position - transform.position).magnitude / relativeVelocity.magnitude;
			// Calculate the point where the monster will intercept the player (ignoring obstacles)
			Vector3 predictedIntercept = player.transform.position + playerVelocity * relativeTime;

			// Update the debug point
			//debugPlayerPredictedPositionIndicator.transform.position = new Vector3(predictedIntercept.x, 0, predictedIntercept.z);
			// Update the position the monster is moving towards
			movement.destination = new Vector3(predictedIntercept.x, player.transform.position.y, predictedIntercept.z);
		}



		// Update the variables used to calculate velocity
		playerPositionLastFrame = player.transform.position;
		monsterPositionLastFrame = transform.position;
    }
}
