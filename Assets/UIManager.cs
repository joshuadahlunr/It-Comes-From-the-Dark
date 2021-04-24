using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject recPanel;
    public Text timePanel;
    float timePassed = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        timePanel.text = timePassed.ToString();

        string hours = ((int)timePassed / 3600).ToString();
        string minutes = ((int)timePassed / 60).ToString();
        string seconds = ((int)timePassed % 60).ToString();
        string ms = ((int)(30 * (timePassed - (int)timePassed))).ToString();

        hours = (int.Parse(hours) < 10 ? "0" + hours : hours);
        minutes = (int.Parse(minutes) < 10 ? "0" + minutes : minutes);
        seconds = (int.Parse(seconds) < 10 ? "0" + seconds : seconds);
        ms = (int.Parse(ms) < 10 ? "0" + ms : ms);

        timePanel.text = hours + ":" + minutes + ":" + seconds + ":" + ms;

        recPanel.SetActive(int.Parse(ms) > 15);
    }
}
