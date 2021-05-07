using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
	[SerializeField]
	// Light references
    Light lightPoint, lightSpot;
	// Variable tracking the (when enabled) intensity of the lights
	float intensity;
	// Random offset to make sure lights aren't all flickering in sync
	float timeOffset;

	public void Start(){
		intensity = lightSpot.intensity;
		timeOffset = Random.Range(0f, 100f);
	}

	// Sets the intensity of both lights
	public void setIntensity(float value){
		intensity = value;
		if(lightPoint.intensity > 0){ // If the lights are enabled
			lightPoint.intensity = value / 2; // The point light is always a lot less bright than the spot light
			lightSpot.intensity = value;
		}
	}

	// Gets the intensity of both lights
	public float getIntensity() {
		return intensity;
	}

	// Dis/enables the lights
	public void setEnabled(bool value){
		if(value) {
			lightPoint.intensity = intensity;
			lightSpot.intensity = intensity;
		} else {
			lightPoint.intensity = 0;
			lightSpot.intensity = 0;
		}
	}

	// Function which will quickly flicker the lights off and on with time
	public void flicker(){
		if(Mathf.PingPong(2 * Time.time + timeOffset, 1) > .5f * (Mathf.Sin(Time.time) + .5f * Mathf.Sin(3 * Time.time)) + .8f)
		 	setEnabled(true);
		else
			setEnabled(false);
	}

	// Function called after flickering to make sure lights are in the correct state
	public void stopFlicker() {
		setEnabled(SpawnSwitches.inst.on);
	}
}
