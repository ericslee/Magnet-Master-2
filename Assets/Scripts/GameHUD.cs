using UnityEngine;
using System.Collections;

public class GameHUD : MonoBehaviour {

	GameManager gameManager;

	void Start () 
	{
		// cache references
		gameManager = GetComponent<GameManager>();
	}
	
	void Update () {
	
	}

	void OnGUI() 
	{
		GUI.Label(new Rect(50, 15, Screen.width / 5, Screen.height / 25), "Lives: " + gameManager.GetTotalLives());

		// Loss state
		if (gameManager.GetTotalLives() <= 0) 
		{
			GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 2, Screen.height / 2), "LUCINA IS DEAD. GAME OVER");
		}
	}
}
