using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    CharacterController characterController;
    public float speed = 0.5f;
    private float translation;
    private float strafe;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        strafe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(strafe, 0, translation);
    }
}
