using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public Button startGameButton;
    public Button instructionsButton;
    public Button creditsButton;
    public Button quitGameButton;

    public GameObject mainmenu;
    public GameObject instructions;
    public GameObject credits;

    void OnEnable()
    {
        startGameButton.onClick.AddListener(OnStartClick);
        instructionsButton.onClick.AddListener(OnInstClick);
        creditsButton.onClick.AddListener(OnCredClick);
        quitGameButton.onClick.AddListener(OnQuitClick);
    }

    void OnStartClick()
    {
        SceneManager.LoadScene("HospitalLevel", LoadSceneMode.Single);
    }

    void OnInstClick()
    {
        instructions.SetActive(true);
        mainmenu.SetActive(false);
    }

    void OnCredClick()
    {
        credits.SetActive(true);
        mainmenu.SetActive(false);
    }

    void OnQuitClick()
    {
        Utility.quit();
    }
}
