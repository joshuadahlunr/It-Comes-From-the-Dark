using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCam : MonoBehaviour
{
    [SerializeField] // Why do the fields need to be seralized whent they are public intrinsic types?
    public float sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;
    public Rigidbody character;
    private Vector2 mouseLook;
    private Vector2 smoothV;

    // Start is called before the first frame update
    void Start()
    {
        character = this.transform.parent.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, mouseDelta.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, mouseDelta.y, 1f / smoothing);
        mouseLook += smoothV;

        // Lock the camera so that you can't look down too far, and you can't look up to far (fixes issue with being able to look up so far the camera becomes upside down!)
        // mouseLook.y = Mathf.Clamp(mouseLook.y, -50, 90);
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.MoveRotation(Quaternion.AngleAxis(mouseLook.x, character.transform.up));
        //character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
    }
}
