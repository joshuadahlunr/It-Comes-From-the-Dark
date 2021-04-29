using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
	public static MonsterAI inst; // Singleton object

	// Enum and variable defining the current state of the monster
	enum State {
		disabled,
		chasing,
		patroling
	};
	State state = State.chasing;

	// Reference to the object which holds all the nav mesh links (monster uses them to decide where to spawn.)
	public GameObject navMeshLinks;
	// Reference to the particle system for the monster
	public GameObject particles;


	// Range of values randomly determining how long it takes for the monster to update the player's predicted position
	public Vector2 playerLocationUpdateIntervalRange = new Vector2(4, 7);
	// How long it should be between player location updates (when the monster doesn't have line of sight)
	float playerLocationUpdateInterval;
	// How long it should be between checks for if the player is in line of sight
	public float physicsCheckUpdateInterval = .25f;

	// When the object is created get a reference to the movement script
	void Awake() {
		inst = this; // Setup singleton

		// movement = GetComponent<MonsterMove>();
		playerLocationUpdateInterval = Random.Range(playerLocationUpdateIntervalRange.x, playerLocationUpdateIntervalRange.y);
	}

	// Timers tracking when locations and line of sight checks should be updated
	float timeSinceLocationUpdate = float.MaxValue, timeSinceLastPhysicsCheck = float.MaxValue;
	// Raycast catcher.
	RaycastHit hit;
	// The location where we predict the player will be
	Vector3 predictedIntercept;
	// The path the monster will patrol.
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
			if (Physics.Raycast (transform.position, (CharacterControl.inst.transform.position + .5f * Vector3.up) - transform.position, out hit))
				// Check if what we hit was the player.
				playerInLineofSight = hit.transform.name == CharacterControl.inst.transform.name; // TODO: why do we have to check the transform's names, why can't we just check the transforms?

			if(state != State.disabled) {
				// If the player is in line of sight start fading in whispers
				if(playerInLineofSight) MonsterAudioManager.inst.fadeInWhispers();
				// Otherwise start fading out whispers
				else MonsterAudioManager.inst.fadeOutWhispers();
			}

			State lastState = state; // Keep track of the state before we transition, logic needs to hapen if we transition out of disabled.
			// If the lights are on, the monster is disabled
			if(SpawnSwitches.inst.on){
				state = State.disabled; // TODO: need to move monster outside playable area!
				MonsterMove.inst.agent.enabled = false;
				particles.SetActive(false);
				transform.position -= new Vector3(0, 20, 0);
			// If we can reach the player, chase the player
			} else if(MonsterMove.inst.CanReachDestination(CharacterControl.inst.transform.position)){
				state = State.chasing;
				timeSinceLocationUpdate = float.MaxValue; // Make sure that we update the player's position right away
			// If we can't reach the player, patrol tha area around the player
			} else if(!MonsterMove.inst.CanReachDestination(predictedIntercept) && state != State.patroling){
				state = State.patroling;
				patrolRadius = initialPatrolRadius; // Reset patrol radius
				createPatrolPath(); // Create path to patrol
			}

			// If we are transitioning from disabled to one of the other states, we need to move the monster back into the playable area
			if(lastState == State.disabled && state != lastState){
				// Find the nav-mesh link furthest away from the player
				float maxDistSqr = 0;
				Transform newSpawn = null;
				foreach (Transform child in navMeshLinks.transform){
					float distSqr = (child.position - CharacterControl.inst.transform.position).sqrMagnitude;
					if(distSqr > maxDistSqr){
						maxDistSqr = distSqr;
						newSpawn = child;
					}
				}

				// Move the monster to the furthest link from the player
				transform.position = newSpawn.position;
				MonsterMove.inst.positionLastFrame = transform.position;
				// Renable navigation
				MonsterMove.inst.agent.enabled = true;
				particles.SetActive(true);
			}

			// Visulize the current path
			MonsterMove.inst.VisualizePath(MonsterMove.inst.destination, .25f);
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
		float initialAngle = Vector3.Angle(transform.forward, CharacterControl.inst.transform.position - transform.position);
		// Figure out the distance to the player (nav-mesh considered)
		float distance2player = MonsterMove.inst.DistanceAlongPath(CharacterControl.inst.transform.position);

		// Play an audio cue to alert the player that they have been lost.
		MonsterAudioManager.inst.playSigh();

		// Make sure the path doesn't have any points
		patrolPoints.Clear();
		// Create a circle of points starting with the inital angle around the player.
		for(float angle = initialAngle; angle < initialAngle + 360; angle += 360.0f/patrolPointCount){
			Vector3 point = CharacterControl.inst.transform.position + -patrolRadius * new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, -Mathf.Sin(angle * Mathf.Deg2Rad));

			// Visualize each point
			Debug.DrawLine(point, point + new Vector3(.1f, .1f, .1f), Color.red, 3);

			if(!MonsterMove.inst.CanReachDestination(point)) continue; // Remove unreachable points
			if(MonsterMove.inst.DistanceAlongPath(point) > distance2player + patrolRadius * patrolRadiusPathLengthCutoffRatio) continue; // Remove points in other rooms (or that are at least take a while to travel to)
			patrolPoints.Add(point);
		}

		// Visualize the chosen points
		foreach (Vector3 point in patrolPoints)
			Debug.DrawLine(CharacterControl.inst.transform.position, point, Color.yellow, 3);
	}

	// Update method called when we are chasing the player
	void chasingUpdate(bool playerInLineofSight){
		// If the player is in line of sight or if enouph time has passed, update the player's predicted position
		if(playerInLineofSight || timeSinceLocationUpdate >= playerLocationUpdateInterval){
			timeSinceLocationUpdate = 0;
			// Randomly determine how long it will take for this check to run again
			playerLocationUpdateInterval = Random.Range(playerLocationUpdateIntervalRange.x, playerLocationUpdateIntervalRange.y);

			// Play an audio cue to alert the player that their location has been tracked.
			MonsterAudioManager.inst.playSigh();

			// Calculate the relative velocity.
			Vector3 relativeVelocity = CharacterControl.inst.velocity - /*monsterVelocity*/MonsterMove.inst.velocity;
			// Calculate the time it will take for the monster to catch up to the player (ignoring obstacles)
			float relativeTime = /*relativeDistance*/(CharacterControl.inst.transform.position - transform.position).magnitude / relativeVelocity.magnitude;
			// Calculate the point where the monster will intercept the player (ignoring obstacles)
			predictedIntercept = CharacterControl.inst.transform.position + CharacterControl.inst.velocity * relativeTime;

			// Update the debug point
			//debugPlayerPredictedPositionIndicator.transform.position = new Vector3(predictedIntercept.x, 0, predictedIntercept.z);
			// Update the position the monster is moving towards
			MonsterMove.inst.destination = new Vector3(predictedIntercept.x, CharacterControl.inst.transform.position.y, predictedIntercept.z);
		}
	}

	// Update method called when we are patroling around the player
	void patrolingUpdate(bool playerInLineofSight){
		// If the path is empty, create a new path
		if(patrolPoints.Count == 0) createPatrolPath();

		// If we have reached the current patrol point, remove it from the list (creating new path and making the radius larger if it is empty)
		if(Vector3.Distance(transform.position, MonsterMove.inst.destination) < .1f){
		//if(MonsterMove.inst.agent.isStopped){
			patrolPoints.RemoveAt(0);
			if(patrolPoints.Count == 0){
				patrolRadius += .5f;
				createPatrolPath();
			}
		}

		// Set the monster's destination to the first point in the path (if it exists)
		try{
			MonsterMove.inst.destination = patrolPoints[0];
		} catch (System.ArgumentOutOfRangeException) {
			// If points don't exist yet, that is because we are too far away from the player to have patrol points not culled for distance, just make sure we are routing towards the player
			MonsterMove.inst.destination = CharacterControl.inst.transform.position;
	 	}


	}
}
