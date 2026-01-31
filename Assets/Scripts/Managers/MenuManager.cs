using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;  //used for loading/unloading scenes

public class MenuManager : MonoBehaviour
{
    void Start()
    {
    }

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
