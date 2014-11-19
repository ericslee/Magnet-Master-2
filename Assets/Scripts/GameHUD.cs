using UnityEngine;
using System.Collections;

public class GameHUD : MonoBehaviour {

	GameManager gameManager;
	PlayerScript playerScript;

	void Start () 
	{
		// cache references
		gameManager = GetComponent<GameManager>();
		playerScript = GameObject.Find("Lucina").GetComponent<PlayerScript>();
	}
	
	void Update () {
	
	}

	void OnGUI() 
	{
		GUI.Label(new Rect(50, 15, Screen.width / 5, Screen.height / 10), "Lives: " + gameManager.GetTotalLives());

		// Loss state
		if (gameManager.GetHasLost()) 
		{
			GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 2, Screen.height / 2), "LUCINA IS DEAD. GAME OVER");
		}
		if (gameManager.GetHasWon())
		{
			GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 2, Screen.height / 2), "MAGNET MASTER HAS BEEN AVENGED. YOU WIN");
		}

		// Powers
		GUI.Label(new Rect(50, 45, Screen.width / 5, Screen.height / 10), playerScript.GetCurrentPower().ToString());
		GUI.Label(new Rect(50, 75, Screen.width / 5, Screen.height / 10), "GAIN: " + playerScript.GetCurrentPowerGain());
	}
}
