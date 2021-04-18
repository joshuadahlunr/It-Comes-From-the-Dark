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

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>(); // Why is this needed?
        collision = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        strafe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        //transform.Translate(strafe, 0, translation);
        collision.MovePosition(transform.position + transform.forward * translation + transform.right * strafe);
    }
}
