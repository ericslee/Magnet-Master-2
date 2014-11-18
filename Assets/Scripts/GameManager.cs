using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	const int MAX_LIVES = 5;
	const int START_LEVEL = 1;

	int currentLevel;
	int totalLives;

	// Checkpoints
	Vector3 initialSpawnPoint = new Vector3(-15.03494f, 3.715919f, 0.374071f);

	// Sound
	AudioSource deathSound;

	// References
	GameObject player;

	// Use this for initialization
	void Start () 
	{
		currentLevel = START_LEVEL;
		totalLives = MAX_LIVES;

		// set up sounds
		deathSound = GetComponents<AudioSource>()[0];

		// cache references
		player = GameObject.Find("Player");
	}

	// Update is called once per frame
	void Update () 
	{
		HandleInput();
	}

	void HandleInput()
	{
		if (Input.GetKey(KeyCode.L))
		{
			// restore lives, respawn player
			totalLives = MAX_LIVES;
			RespawnPlayer();
		}
	}

	public void LoseLife() 
	{
		totalLives--;
		deathSound.Play();

		// respawn player
		RespawnPlayer();

		if (totalLives <= 0) 
		{
			Debug.Log("GAME OVERRRRRRRRRRRRR");
		}
	}

	void RespawnPlayer()
	{
		player.transform.position = initialSpawnPoint;
	}

	// Getters/setters
	public int GetTotalLives() { return totalLives; }
}
