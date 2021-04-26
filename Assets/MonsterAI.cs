﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
	enum State {
		disabled,
		chasing,
		patroling
	};
	State state = State.disabled;

	// Variable referencing the player
	public Rigidbody player;
	// Reference to the monster's movement script.
	MonsterMove movement;
	// Reference to the monster's audio manager
	public MonsterAudioManager audioManager;
	// Debug marker representing the player's predicted position
	//public GameObject debugPlayerPredictedPositionIndicator;

	public SpawnSwitches switchMgr;


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

	Vector3 predictedIntercept;

	List<Vector3> patrolPoints = new List<Vector3>();

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

			if(state != State.disabled) {
				// If the player is in line of sight start fading in whispers
				if(playerInLineofSight) audioManager.fadeInWhispers();
				// Otherwise start fading out whispers
				else audioManager.fadeOutWhispers();
			}

			if(switchMgr.on){
				state = State.disabled; // TODO: need to move monster outside playable area!
				movement.agent.enabled = false;
			// If we can reach the player, chase the player
			} else if(movement.CanReachDestination(player.transform.position)){
				movement.agent.enabled = true;
				state = State.chasing;
				timeSinceLocationUpdate = float.MaxValue; // Make sure that we update the player's position right away
			// If we can't reach the player, patrol tha area around the player
			} else if(!movement.CanReachDestination(predictedIntercept) && state != State.patroling){
				movement.agent.enabled = true;
				state = State.patroling;
				patrolRadius = initialPatrolRadius; // Reset patrol radius
				createPatrolPath(); // Create path to patrol
			}

			// Visulize the current path
			movement.VisualizePath(movement.destination, .25f);
		}

		// Depending on our state preform a different set of actions.
		switch(state){
		case State.disabled:
			break;
		case State.chasing:
			chasingUpdate(playerInLineofSight);
			break;
		case State.patroling:
			patrolingUpdate(playerInLineofSight);
			break;
		}



		// Update the variables used to calculate velocity
		playerPositionLastFrame = player.transform.position;
		monsterPositionLastFrame = transform.position;
    }

	// Variable which defines the patrol radius the monster always starts with
	public float initialPatrolRadius = 3;
	// Variable which defines the current patrol radius
	float patrolRadius = 3;
	// Variable defining how many points to generate with the patrol radius
	public int patrolPointCount = 12;
	// Determine how much further than the distance to the player a patrol point can be before it is ignored
	public float patrolRadiusPathLengthCutoffRatio = 1.3f;

	// Creates a path around the player for the monster to patrol
	void createPatrolPath(){
		// Calculate the angle between the player and the monster
		float initialAngle = Vector3.Angle(transform.forward, player.transform.position - transform.position);
		// Figure out the distance to the player (nav-mesh considered)
		float distance2player = movement.DistanceAlongPath(player.transform.position);

		// Play an audio cue to alert the player that they have been lost.
		audioManager.playSigh();

		// Make sure the path doesn't have any points
		patrolPoints.Clear();
		// Create a circle of points starting with the inital angle around the player.
		for(float angle = initialAngle; angle < initialAngle + 360; angle += 360.0f/patrolPointCount){
			Vector3 point = player.transform.position + -patrolRadius * new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, -Mathf.Sin(angle * Mathf.Deg2Rad));

			// Visualize each point
			Debug.DrawLine(point, point + new Vector3(.1f, .1f, .1f), Color.red, 3);

			if(!movement.CanReachDestination(point)) continue; // Remove unreachable points
			if(movement.DistanceAlongPath(point) > distance2player + patrolRadius * patrolRadiusPathLengthCutoffRatio) continue; // Remove points in other rooms (or that are at least take a while to travel to)
			patrolPoints.Add(point);
		}

		// Visualize the chosen points
		foreach (Vector3 point in patrolPoints)
			Debug.DrawLine(player.transform.position, point, Color.yellow, 3);
	}

	// Update method called when we are chasing the player
	void chasingUpdate(bool playerInLineofSight){
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
			predictedIntercept = player.transform.position + playerVelocity * relativeTime;

			// Update the debug point
			//debugPlayerPredictedPositionIndicator.transform.position = new Vector3(predictedIntercept.x, 0, predictedIntercept.z);
			// Update the position the monster is moving towards
			movement.destination = new Vector3(predictedIntercept.x, player.transform.position.y, predictedIntercept.z);
		}
	}

	// Update method called when we are patroling around the player
	void patrolingUpdate(bool playerInLineofSight){
		// If the path is empty, create a new path
		if(patrolPoints.Count == 0) createPatrolPath();

		// If we have reached the current patrol point, remove it from the list (creating new path and making the radius larger if it is empty)
		if(Vector3.Distance(transform.position, movement.destination) < .1f){
		//if(movement.agent.isStopped){
			patrolPoints.RemoveAt(0);
			if(patrolPoints.Count == 0){
				patrolRadius += .5f;
				createPatrolPath();
			}
		}

		// Set the monster's destination to the first point in the path (if it exists)
		try{
			movement.destination = patrolPoints[0];
		} catch (System.ArgumentOutOfRangeException) {
			// If points don't exist yet, that is because we are too far away from the play to have patrol points not culled for distance, just make sure we are routing towards the player
			movement.destination = player.transform.position;
	 	}


	}
}
