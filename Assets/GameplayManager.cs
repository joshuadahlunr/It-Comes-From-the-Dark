using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager inst; // Singleton object

    public bool gameOver = false;

    float dist;
    float timePassed = 0;

    public int deathDist;

    void Awake()
    {
        inst = this; // Setup the singleton
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        dist = Vector3.Distance(CharacterControl.inst.transform.position, MonsterMove.inst.transform.position);
        if(dist < deathDist)
        {
            DeathMenu.inst.gameObject.SetActive(true);
			DeathMenu.inst.pauseTimer = true;
			gameOver = true;
        }

		else if(Input.GetKeyDown(KeyCode.Escape)){
			DeathMenu.inst.gameObject.SetActive(true);
			DeathMenu.inst.pauseTimer = false;
		}
    }

    public float getDist() { return dist; }
    public float getTime() { return timePassed; }

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
