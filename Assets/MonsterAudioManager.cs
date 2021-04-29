using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MonsterAudioManager : MonoBehaviour
{
	public static MonsterAudioManager inst; // Singleton object

	// Audio Sources for the whispers and sighs
    public AudioSource whisperSource;
	public AudioSource sighSource;

	// List of sigh clips to randomly select from
	public List<AudioClip> sighList;

	// The ammount of time that it takes for whispers to completely fade in or out
	public float whisperVolumeFadeTime = 3f;
	// The max volume of the whispers
	public float maxWhisperVolume = .8f;
	// The desired volume of the whispers we are shifting towards
	float desiredWhisperVolume = 0;

	void Awake(){
		inst = this; // Setup singleton
	}

    // Update is called once per frame, updates the volume of the whispers towards the desired volum
    void Update()
    {
		// Shift towards the desired volume
		if(desiredWhisperVolume > whisperSource.volume)
			whisperSource.volume += Time.deltaTime / whisperVolumeFadeTime;
		else if (desiredWhisperVolume < whisperSource.volume)
			whisperSource.volume -= Time.deltaTime / whisperVolumeFadeTime;
    }

	/// Plays a randomly selected sigh to notify the player that their location has been updated.
    public void playSigh() {
		// If a clip isn't currently playing
		if (!sighSource.isPlaying){
			// Choose a random clip to play
			sighSource.clip = sighList[Random.Range(0, sighList.Count)];
			// And play it now
			sighSource.Play(0);
		}
	}

	/// Fades in the whisper loop
	public void fadeInWhispers() {
		// Set the desired volume to the max
		desiredWhisperVolume = maxWhisperVolume;

		// Play or unpause the clip as appropriate
		if (!whisperSource.isPlaying)
			whisperSource.Play();
		else
			whisperSource.UnPause();
	}

	/// Fades out the whisper loop
	public void fadeOutWhispers() {
		// Set the desired volume to 0
		desiredWhisperVolume = 0;

		// If the desired volume is already 0, pause the clip.
		if(whisperSource.volume == 0)
			whisperSource.Pause();
	}
}
