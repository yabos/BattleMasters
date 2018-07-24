using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_Control : MonoBehaviour
{
    public void OnNextLevel()
    {
        SceneManager.LoadScene("Battle");
    }	
}
