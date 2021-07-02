using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour
{
	[Header("Game Characters")]
	[SerializeField] GameObject pacman;
	[SerializeField] GameObject mapCoins; // all coins on the map

	[Header("Values")]
	[SerializeField] float score;
	[SerializeField] float highScore;
	[SerializeField] int lives;

	[Header("User Interface")]
	[SerializeField] Text scoreText;
	[SerializeField] Text highScoreText;

	[Header("Sound Effects")]
	[SerializeField] AudioSource backgroundMusic;

	[Header("Main Menu")]
	[SerializeField] bool mainMenu;

    // Start is called before the first frame update
    void Start()
    {
    	backgroundMusic.Play(); // start game soundtrack
        lives = PlayerPrefs.GetInt("Lives", lives); // get back how many lives we have
    }

    // Update is called once per frame
    void Update()
    {
    	if(!mainMenu) // if we're not in the main menu (we're in game)
    	{
    		GetValues(); // get score..etc from pacman
	        UpdateValues(); // sync values with UI and saved data
	        if(pacman.gameObject.GetComponent<PacmanScript> ().died) // if pacman is dead 
	        {
	        	RestartAndReset(); // completeley reset the score and everything & restart 
	        }

	        if(mapCoins.transform.childCount <= 0) // if pacman eats all coins & all coins are destroyed
	        {
	        	Restart(); // restart but don't reset anything
	        }
    	}
    	
    }


    // get score..etc from pacman
    void GetValues()
    {
    	score = pacman.gameObject.GetComponent<PacmanScript> ().score; // getting the score value from pacman's game object
    }

    // sync values with UI and saved data
    void UpdateValues()
    {
    	scoreText.text = score.ToString(); // update the score's UI
    	highScoreText.text = PlayerPrefs.GetFloat("HighScore", highScore).ToString(); // update the high score's UI
    	if(score > PlayerPrefs.GetFloat("HighScore", highScore)) // if the current score is higher than our high score
    	{
    		highScore = score; // update high score
    		PlayerPrefs.SetFloat("HighScore", highScore); 
    	}	
    }


    // completeley reset the score and everything & restart
    void RestartAndReset()
    {
    	lives = 7; // reset lives back to 7
    	PlayerPrefs.SetInt("Lives", lives); // save it to saved data
    	score = 0; // reset score back to 0
    	SceneManager.LoadScene("Main Menu"); // go to main menu
    }

    // restart but don't reset anything
    void Restart()
    {
    	SceneManager.LoadScene("Game"); // start the game
    }

    public void StartGame()
    {
    	SceneManager.LoadScene("Game"); // start the game
    }

    // resetting high score
    public void ResetHighScore()
    {
    	highScore = 0;
    	PlayerPrefs.SetFloat("HighScore", highScore); // saving it to saved data
    }

    // quitting the game
    public void ExitGame()
    {
    	Application.Quit(); // closing the window
    }
}
