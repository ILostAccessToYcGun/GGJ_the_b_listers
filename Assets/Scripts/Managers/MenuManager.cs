using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;  //used for loading/unloading scenes

public class MenuManager : MonoBehaviour
{
    //reference to the entire pause menu panel GameObject
    [SerializeField] private GameObject pauseMenuUI;
    private bool isGamePaused = false;

    void Start()
    {
        //ensures the menu is hidden when the game starts
        pauseMenuUI.SetActive(false);
    }

   void Update()
    {
        //checks if the escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    //load into the first scene once the player presses "Start Game"
    public void PlayButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); //hides the menu
        Time.timeScale = 1f;         //resumes time
        isGamePaused = false;
        //optional - ensure cursor is locked/hidden for gameplay
        //Cursor.lockState = CursorLockMode.Locked; 
    }

    void Pause() //private as it's only called internally on key press
    {
        pauseMenuUI.SetActive(true); //show the menu
        Time.timeScale = 0f;         //stop time
        isGamePaused = true;
        //optional - makes the cursor visible for menu interaction
        Cursor.lockState = CursorLockMode.None; 
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; //ensures time is normal before loading new scene
        //replace "MaruDev" with the actual name of your main menu scene
        SceneManager.LoadScene("MaruDev");
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
