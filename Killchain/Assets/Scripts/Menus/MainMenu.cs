using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public bool isStart;
    public bool isSettings;
    public bool isQuit;
    
    private TextMesh text;
    public GameObject settingsMenu;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMesh>();
        text.color = Color.white;
    }

    void OnMouseUp()
    {
        // Each if statement is a different button
        if (isStart)
        {
            // Begins the game
            SceneManager.LoadScene(1);
        }
        if (isSettings)
        {
            // Turns on the setting menu
            settingsMenu.SetActive(true);
            // Turns the main menu text off
            GameObject.Find("Menu").SetActive(false);
            // Turns the text back to white
            // Otherwise the text remains black when the settings menu is closed
            text.color = Color.white;
        }
        if (isQuit)
        {
            // Quits the game
            Application.Quit();
        }
    }

    void OnMouseEnter()
    {
        // When the mouse moves over the button
        // Turn the next black
        text.color = Color.black;
    }

    void OnMouseExit()
    {
        // When the mouse moves off the button
        // Turn the next back to white
        text.color = Color.white;
    }
}
