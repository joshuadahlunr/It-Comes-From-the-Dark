using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OtherMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public Button button;

    public GameObject mainmenu;
    public GameObject thismenu;

    void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        mainmenu.SetActive(true);
        thismenu.SetActive(false);
    }
}