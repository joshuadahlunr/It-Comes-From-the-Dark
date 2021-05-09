using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    // Setup singleton
	public static DeathMenu inst;
	void Awake() {
		inst = this;
	}

    public Button returnMenuButton;
    public Button quitGameButton;
    public Text timeText;

	public bool pauseTimer = true;

	void Start() {
		// Disable menu at the start of the game
		gameObject.SetActive(false);
	}


	void updateTime() {
		float timePassed = GameplayManager.inst.getTime();
        //timePanel.text = timePassed.ToString();

        string hours = ((int)timePassed / 3600).ToString();
        string minutes = ((int)timePassed / 60).ToString();
        string seconds = ((int)timePassed % 60).ToString();
        string ms = ((int)(30 * (timePassed - (int)timePassed))).ToString();

        hours = (int.Parse(hours) < 10 ? "0" + hours : hours);
        minutes = (int.Parse(minutes) < 10 ? "0" + minutes : minutes);
        seconds = (int.Parse(seconds) < 10 ? "0" + seconds : seconds);
        ms = (int.Parse(ms) < 10 ? "0" + ms : ms);

        timeText.text = hours + ":" + minutes + ":" + seconds + ":" + ms;
	}


    void OnEnable()
    {
        returnMenuButton.onClick.AddListener(OnReturnClick);
        quitGameButton.onClick.AddListener(OnQuitClick);

        updateTime();
    }

    // Update is called once per frame
    void Update()
    {
		if(!pauseTimer) updateTime();

		if(!pauseTimer && Input.GetKeyDown(KeyCode.Escape))
			gameObject.SetActive(false);

    }

    void OnReturnClick()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    void OnQuitClick()
    {
        Application.Quit();
    }
}
