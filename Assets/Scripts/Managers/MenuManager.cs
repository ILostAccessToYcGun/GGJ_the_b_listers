using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //load into the first scene once the player presses "Start Game"
    public void PlayButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    //Quit button
    //  If in the editor and its pressed, it will stop running there
    //  else if its not running in the editor (so like in the actual game) it works as well to end the game and close
    public void onQuit()
    {
#if !UNITY_EDITOR
Application.Quit();
#else 
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
