using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficulyIncrease : MonoBehaviour
{
	public static DifficulyIncrease inst; // Singleton object
	void Awake(){
		inst = this; // Setup singleton
	}

	// Variable tracking the number of times switches have been flipped
    public int switchFlipCount = 0;

	public void increaseDifficulty(){
		switchFlipCount++;

		// Decreases the ammount of time the lights will remain on for.
		SpawnSwitches.inst.timeOn = Mathf.Clamp(SpawnSwitches.inst.timeOn - 2.5f, SpawnSwitches.inst.minTimeOn, float.MaxValue);
		// Increases the number of switches you must find between uses (there will alwys be two switches you can use on the map)
		SpawnSwitches.inst.switchesBetweenUse = Mathf.Clamp(SpawnSwitches.inst.switchesBetweenUse + 1, 0, SpawnSwitches.inst.numSwitches - 3);

		// Monster gets faster (but not through doors);
		MonsterMove.inst.agent.speed += .0125f;
	}
}
