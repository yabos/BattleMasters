using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_Control : MonoBehaviour
{
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution((Screen.width * 16) / 9, Screen.width, true);
    }

    public void OnNextLevel()
    {
        SceneManager.LoadScene("Battle");
    }	
}
