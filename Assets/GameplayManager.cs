using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager inst; // Singleton object

    public bool gameOver;

    float dist;
    float timePassed = 0;

    public GameObject player;
    public GameObject monster;
    public GameObject deathScreen;
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
        dist = Vector3.Distance(player.transform.position, monster.transform.position);
        if(dist < deathDist)
        {
            deathScreen.SetActive(true);
        }
    }

    public float getDist() { return dist; }
    public float getTime() { return timePassed; }
}
