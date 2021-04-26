using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public Button startGameButton;


    void Start()
    {
        startGameButton.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {


    }

    void OnClick()
    {
        SceneManager.LoadScene("HospitalLevel", LoadSceneMode.Single);
    }
}
