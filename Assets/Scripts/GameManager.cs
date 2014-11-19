using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	const int MAX_LIVES = 5;
	const int START_LEVEL = 1;

	int currentLevel;
	int totalLives;

	// Game state
	bool hasLost = false;
	bool hasWon = false;

	// Checkpoints
	Vector3 initialSpawnPoint = new Vector3(-15.03494f, 3.715919f, 0.374071f);

	// Sound
	AudioSource deathSound;

	// References
	GameObject player;

	// Use this for initialization
	void Start() 
	{
		currentLevel = START_LEVEL;
		totalLives = MAX_LIVES;

		// cache references
		player = GameObject.Find("Lucina");

		// set up sounds
		deathSound = GetComponents<AudioSource>()[0];

		// do not allow certain objects to be sucked in by gravity
		Physics.IgnoreLayerCollision(13, 9, true); // player
		Physics.IgnoreLayerCollision(13, 8, true); // reticle
		Physics.IgnoreLayerCollision(13, 11, true); // gravity centers
	}

	// Update is called once per frame
	void Update() 
	{
		HandleInput();
	}

	void HandleInput()
	{
		if (Input.GetKey(KeyCode.L))
		{
			// restore lives, respawn player
			totalLives = MAX_LIVES;
			hasLost = false;
			hasWon = false;
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
			hasLost = true;
			Debug.Log("GAME OVERRRRRRRRRRRRR");
		}
	}

	void RespawnPlayer()
	{
		player.transform.position = initialSpawnPoint;
	}

	public void Lose()
	{
		hasLost = true;
	}

	public void Win()
	{
		hasWon = true;
	}

	// Getters
	public int GetTotalLives() { return totalLives; }
	public int GetMaxLives() { return MAX_LIVES; }
	public bool GetHasLost() { return hasLost; }
	public bool GetHasWon() { return hasWon; }
}
