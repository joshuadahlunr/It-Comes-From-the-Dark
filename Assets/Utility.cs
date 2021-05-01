using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Utility : MonoBehaviour
{
	// Function which generates a random point on the navmesh, a set distance from a point
    static public Vector3 randomNavmeshLocation(Vector3 origin, float radius) {
		while(true){ // Keep generating points until we succede.
			Vector3 randomPosition = Random.insideUnitSphere * radius + origin;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(randomPosition, out hit, radius, 1))
				return hit.position;
		}
	}
}
