using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    CharacterController characterController; // Why is this needed?
    public Rigidbody collision;
    public float speed = 0.5f;
    private float translation;
    private float strafe;

	// Variable tracking the players's current velocity
	public Vector3 velocity;
	// Variable tracking the player's position last frame for velocity calculations
    public Vector3 positionLastFrame;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>(); // Why is this needed?
        collision = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
		Vector3 move = (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")).normalized * speed * Time.deltaTime;

        //transform.Translate(strafe, 0, translation);
        collision.MovePosition(transform.position + move);

		// Update velocity
		velocity = (transform.position - positionLastFrame) / Time.deltaTime;
		// Save character's position
		positionLastFrame = transform.position;
    }
}
