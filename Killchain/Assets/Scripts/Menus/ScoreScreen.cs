using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreScreen : MonoBehaviour
{
    public Text scoreText;
    public Text highScoreText;
    public GameObject newHighScore;

    private int highScore;

    public void ActiveEndScreen(int score)
    {
        // Activates itself
        this.gameObject.SetActive(true);
        // Updates the scores on the end screen
        scoreText.text = score.ToString();
        highScore = PlayerPrefs.GetInt("High Score", 0);
        highScoreText.text = highScore.ToString();

        // If the new score for the run is a highscore
        if (score > highScore)
        {
            // Enable a congrats message
            newHighScore.SetActive(true);
            // Save the new highscore in player settings
            PlayerPrefs.SetInt("High Score", score);
        }
    }

    public void QuitToMenu()
    {
        // Loads the main menu
        SceneManager.LoadScene(0);
    }

    public void RestartRun()
    {
        // Reloads the current scene to begin a new run
        SceneManager.LoadScene(1);
    }
}
