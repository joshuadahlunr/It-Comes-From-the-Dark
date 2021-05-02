using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public static CharacterControl inst; // Singleton object

    public Rigidbody collision;
    public float speed = 0.5f;
    private float translation;
    private float strafe;

	// Variable tracking the players's current velocity
	public Vector3 velocity;
	// Variable tracking the player's position last frame for velocity calculations
    public Vector3 positionLastFrame;

	// Time that power ups draw from (thrice this number is the number of seconds of night vision the player has)
	public float batteryCharge = 2;

	void Awake(){
		inst = this; // Setup singleton
	}

    // Start is called before the first frame update
    void Start()
    {
        collision = GetComponent<Rigidbody>();
		// Spawn the player in a random position (on the navmesh) around the center of the map
		collision.MovePosition(Utility.randomNavmeshLocation(new Vector3(20, 0, 20), 20));
		// The player spawns facing in a random direction
		MouseCam.inst.mouseLook.x = Random.Range(0f, 360f);

    }

    // Update is called once per frame
    void Update()
    {
		Vector3 move = (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")).normalized * speed * Time.deltaTime;
        collision.MovePosition(transform.position + move);

		// Update velocity
		velocity = (transform.position - positionLastFrame) / Time.deltaTime;
		// Save character's position
		positionLastFrame = transform.position;
    }
}
