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
}
