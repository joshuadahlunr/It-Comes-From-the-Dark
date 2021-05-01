using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public Button startGameButton;
    public Button quitGameButton;


    void Start()
    {
        startGameButton.onClick.AddListener(OnStartClick);
        quitGameButton.onClick.AddListener(OnQuitClick);
    }

    // Update is called once per frame
    void Update()
    {


    }

    void OnStartClick()
    {
        SceneManager.LoadScene("HospitalLevel", LoadSceneMode.Single);
    }

    void OnQuitClick()
    {
        Application.Quit();
    }
}
