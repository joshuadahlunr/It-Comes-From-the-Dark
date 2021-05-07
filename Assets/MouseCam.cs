using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCam : MonoBehaviour
{
	public static MouseCam inst; // Singleton object

    [SerializeField] // Why do the fields need to be seralized whent they are public intrinsic types?
    public float sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;
	public AudioSource audioSource;
	DeferredNightVisionEffect nightVision;
    public Vector2 mouseLook;
    private Vector2 smoothV;

	// Variable defining the default camera height
	public float cameraHeight = 1.1f;
	// How much the camera bob is scaled by
	public float bobIntensity = .025f;
	// How often a footstep will occure
	public float footstepInterval = 2;

	void Awake(){
		inst = this; // Setup singleton
	}

	void Start(){
		nightVision = GetComponent<DeferredNightVisionEffect>() as DeferredNightVisionEffect;
	}


	// Variable storing the position last frame for displacement calculations
	Vector3 positionLastFrame;
	// Variable storing the total displacement
	float displacement = 0;
	// Variable which prevents footsteps from playing too frequently
	bool canPlayFootstepAgain = true;
	// Variable storing raycast hits
	RaycastHit hit;

    // Update is called once per frame
    void Update()
    {
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, mouseDelta.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, mouseDelta.y, 1f / smoothing);
        mouseLook += smoothV;

        // Lock the camera so that you can't look down too far, and you can't look up to far (fixes issue with being able to look up so far the camera becomes upside down!)
        mouseLook.y = Mathf.Clamp(mouseLook.y, -50, 90);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        CharacterControl.inst.collision.MoveRotation(Quaternion.AngleAxis(mouseLook.x, CharacterControl.inst.transform.up));

		// If the right mouse button is held down (and we have battery charge left), enable night vision
		if(Input.GetMouseButton(1) && CharacterControl.inst.batteryCharge > 0){
			nightVision.enabled = true;
			CharacterControl.inst.batteryCharge -= Time.deltaTime / 3; // Tick down the charge with time
		} else nightVision.enabled = false;

		// Cacluate total distance the player has traveled so far
		displacement += (transform.position - positionLastFrame).magnitude; // TODO: moving at a diagonal makes this go up quite a bit faster... not sure how to fix that
		// Calculate how much the view should bob, and then make it bob that much
		float heightModulation = bobIntensity * Mathf.Sin(Mathf.PI / footstepInterval * displacement);
		transform.localPosition = new Vector3(0, cameraHeight + heightModulation, 0);
		// If we are at a rising point in the footsteps, and a footstep isn't playing, and enouph of the cycle has passed that we can play another footstep...
		if(heightModulation > 0 && !audioSource.isPlaying && canPlayFootstepAgain){
			audioSource.Play(); // Play a footstep
			canPlayFootstepAgain = false; // And mark that we can't play another footstep for a while
		// Once we have dropped below 0, it is fine to play another footstep
		} else if (heightModulation < 0) canPlayFootstepAgain = true;

		// Preform a raycast in the direction the player is looking (while the E key is pressed)
		if (Input.GetKeyUp(KeyCode.E))
			if(Physics.Raycast(transform.position, transform.forward, out hit, 3)){
				// If we hit a switch, preform the logic for the turning on the lights
				if(hit.transform.tag == "Switch")
					SpawnSwitches.inst.Acivated(hit.transform);
				// If we hit a battery, preform the logic for picking it up
				else if(hit.transform.tag == "Battery")
					BatterySpawn.inst.Activated(hit.transform);

			}

		positionLastFrame = transform.position;
    }
}
