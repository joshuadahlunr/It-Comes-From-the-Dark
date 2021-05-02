using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public Button returnMenuButton;
    public Button quitGameButton;
    public Text timeText;

    public GameplayManager gpManager;


    void OnEnable()
    {
        returnMenuButton.onClick.AddListener(OnReturnClick);
        quitGameButton.onClick.AddListener(OnQuitClick);

        float timePassed = gpManager.getTime();
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

    // Update is called once per frame
    void Update()
    {


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
