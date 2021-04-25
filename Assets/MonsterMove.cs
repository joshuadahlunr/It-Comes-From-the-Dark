/*
 * MonsterMove.cs
 * This file is responsible for smoothly moving the monster to a requested point.
 * NOTE: Requires that there is a NavMeshAgent component attached to the same GameObject as this script.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent

public class MonsterMove : MonoBehaviour {
    // Reference to the NavigationAgent which handles the bulk of the monster's movement
    public NavMeshAgent agent;
    // Reference to the debugIndicator object which we position for visualization.
    //public GameObject debugIndicator;

    // Vector3 representing the target that the monster is moving towards.
    public Vector3 destination {
        get { return agent.destination; }
        set { SetDestination(value);  }
    }

	// Variable representing how quickly the monster should traverse doorways (this is a constant slightly slower than the player's speed)
	public float doorMovementSpeed = 1.0f;

    // Variable tracking whether or not we have started moving across a door link.
    bool moveAccrossLinkStarted = false;



    /// <summary>
    /// Function which runs when the object is created, and assigns some nessicary references.
    /// </summary>
    void Awake() {
        // Make sure that the reference to the NavMeshAgent is set
        agent = GetComponent<NavMeshAgent>();
        if(agent == null)
            Debug.LogError("This script must be attached to an object with a NavMeshAgent attached! Not found on: " + gameObject.name);
    }

    /// <summary>
    /// This function handles the calculations which need to be handled every frame.
    /// It manages a coroutine which handles moving the monster through doors.
    /// It also manages changing the debug target for the monster.
    /// </summary>
    void Update() {
        // If we need to go through a door link (Unity "Off Mesh Link" start a coroutine to handle moving the object accross)
        if (agent.isOnOffMeshLink && !moveAccrossLinkStarted)
            StartCoroutine(MoveAcrossDoorLink());
    }

    /// <summary>
    /// Updates the destination of the monster.
    /// Also positions a debugging indicator so that we can visualize the target of the monster.
    /// </summary>
    /// <param name="dest">The new location the monster should move toward</param>
    void SetDestination(Vector3 dest) {
        // Set the destination for the navigation agent
        agent.SetDestination(dest);

        // Move the debug indicator
        //debugIndicator.transform.position = dest;

    }

	/// <summary>
    /// Determines if we can reach the requested destination
    /// </summary>
    /// <param name="dest">The new location for the monster to check if we can reach.</param>
	/// <return>Whether or not we can reach the target desination.</return>
	public bool CanReachDestination(Vector3 dest){
		// Calculate the path to the destination and figure out if we can reach it.
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(dest, path);
        return path.status == NavMeshPathStatus.PathComplete;
	}

	/// <summary>
    /// Determines how much distance we have to travel to get to the target
    /// </summary>
    /// <param name="dest">The new location for the monster to check distance to.</param>
	/// <return>The distance along the path to the destination.</return>
	public float DistanceAlongPath(Vector3 dest){
		// Calculate the path
		NavMeshPath path = new NavMeshPath();
		agent.CalculatePath(dest, path);

		// Based on the points in the path add up the distance that will be traveled.
		float distance = 0;
		for (int i = 0; i < path.corners.Length - 1; i++)
			distance += Vector3.Distance(path.corners[i], path.corners[i+1]);
		return distance;
	}

	/// <summary>
    /// Displays the path to the destination point in the scene view.
    /// </summary>
    /// <param name="dest">The location for the monster to display the path to.</param>
	public void VisualizePath(Vector3 dest, float time = 0f){
		// Calculate the path
		NavMeshPath path = new NavMeshPath();
		agent.CalculatePath(dest, path);

		for (int i = 0; i < path.corners.Length - 1; i++)
			Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.white, time);
	}

    /// <summary>
    /// This coroutine manages the motion of the monster accross door links.
    /// </summary>
    /// <returns>Null</returns>
    IEnumerator MoveAcrossDoorLink() {
        // Mark that we have started moving across the link (prevents another copy of this coroutine from being started)
        moveAccrossLinkStarted = true;
        // Get the data from the navmesh about the door link
        OffMeshLinkData data = agent.currentOffMeshLinkData;

        // Calculate the start and ending position of the link
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos;// + Vector3.up * agent.baseOffset;
        // Using the start and end positions calculate how long it will take (the square magnitudes are used so this is an approximation)
		float duration = (endPos - startPos).sqrMagnitude / doorMovementSpeed / doorMovementSpeed; // Divide by doorMovementSpeed squared

        // Create a variable which tracks how long we have been squeezing through the door (time is scaled between 0 and 1 so that interpolations can work)
        float time = 0.0f;
        while (time < 1.0f) { // Continue squeezing until our calculated time has elapsed.
            // Each time this loop runs we interoplate the monster's position so that it slides a little closer to the end of the link.
            //transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.position = Vector3.Slerp(startPos, endPos, time);

            // Add the elapsed time to the current time (it has to be divided by the total duration so that the final total is 1)
            time += Time.deltaTime / duration;
            // Pause this coroutine and process everything else which needs to be processed
            yield return null;
        }

        // Make sure that the entity's position is the end of the link (we do use a few approximations)
        transform.position = endPos;
        // Mark that we have finished traversing the link
        agent.CompleteOffMeshLink();
        moveAccrossLinkStarted = false;

    }
}
