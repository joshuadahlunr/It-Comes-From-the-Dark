using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager inst; // Singleton object

    public GameObject recPanel;
    public Text timePanel;
    public List<Image> noisePanel;
    public GameObject player;
    public GameObject monster;
    public SpawnSwitches spawnSwitches;
    public int noiseAmount;
    float timePassed = 0;

    public GameplayManager gpManager;

    void Awake() {
		inst = this; // Setup the singleton
	}

    // Update is called once per frame
    void Update()
    {

        // Time Panel

        timePassed = gpManager.getTime();
        //timePanel.text = timePassed.ToString();

        string hours = ((int)timePassed / 3600).ToString();
        string minutes = ((int)timePassed / 60).ToString();
        string seconds = ((int)timePassed % 60).ToString();
        string ms = ((int)(30 * (timePassed - (int)timePassed))).ToString();

        hours = (int.Parse(hours) < 10 ? "0" + hours : hours);
        minutes = (int.Parse(minutes) < 10 ? "0" + minutes : minutes);
        seconds = (int.Parse(seconds) < 10 ? "0" + seconds : seconds);
        ms = (int.Parse(ms) < 10 ? "0" + ms : ms);

        timePanel.text = hours + ":" + minutes + ":" + seconds + ":" + ms;

        // Record Panel

        recPanel.SetActive(int.Parse(ms) > 15);

        // Noise Panel
        if (int.Parse(ms) % 1 == 0)
        {
            for (int i = 0; i < noisePanel.Count; i++)
            {
                noisePanel[i].color = new Color32(255, 255, 255, 0);
            }

            float dist = 
            noiseAmount = 100;
            if (!spawnSwitches.on)
            {
                noiseAmount = (int)(255.0f - 10.0f * dist);
                noiseAmount = Mathf.Max(100, noiseAmount);
            }
            int panelforfun = (int)((Random.value * 8) % 8);
            noisePanel[panelforfun].color = new Color32(64, 64, 64, (byte)noiseAmount);
            noisePanel[panelforfun].transform.localRotation = Quaternion.Euler(0, (Random.value > 0.5 ? 180 : 0), 0);
            noisePanel[panelforfun].transform.localRotation = Quaternion.Euler((Random.value > 0.5 ? 180 : 0), 0, 0);
        }
    }
}
