using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	const int MAX_LIVES = 3;
	const int MAX_HEALTH = 5;
	const int START_LEVEL = 1;
	const float CAM_Y_POS_PLUS_LEVEL_1 = 2;
	const float CAM_Y_POS_PLUS_LEVEL_2 = 2;
	const float CAM_Y_POS_PLUS_LEVEL_3 = 0;
	const float CAM_Z_POS_LEVEL_1 = -10.0f;
	const float CAM_Z_POS_LEVEL_2 = -20.0f;
	const float CAM_Z_POS_LEVEL_3 = -20.0f;
	const float RETICLE_Z_POS_LEVEL_1 = -2.0f;
	const float RETICLE_Z_POS_LEVEL_2 = -2.0f;
	const float RETICLE_Z_POS_LEVEL_3 = -2.0f;

	public int currentLevel = START_LEVEL;
	int totalLives = MAX_LIVES;
	int totalHealth = MAX_HEALTH;

	// Game state
	bool hasLost = false;
	bool hasWon = false;

	// Power state
	bool hasLevitation = false;
	bool hasGravity = false;
	bool hasElectricity = false;

	// Checkpoints
	List<Vector3> level1RespawnPoints = new List<Vector3>();
	List<Vector3> level2RespawnPoints = new List<Vector3>();
	List<Vector3> level3RespawnPoints = new List<Vector3>();
	int currentSpawnPoint = 0;
	List<Vector3> currentRespawnPoints;
	GameObject respawnEffect;

	// Sound
	AudioSource deathSound;
	AudioSource level1Music;
	AudioSource level2Music;
	AudioSource level3Music;
	AudioSource level3StartSound;
	AudioSource respawnSound;

	// References
	GameObject player;
	PlayerScript playerScript;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start() 
	{
		// cache/load references
		respawnEffect = (GameObject)Resources.Load("Prefabs/RespawnEffect");

		// set up sounds
		deathSound = GetComponents<AudioSource>()[0];
		level3Music = GetComponents<AudioSource>()[1];
		level1Music = GetComponents<AudioSource>()[3];
		level3StartSound = GetComponents<AudioSource>()[2];
		level2Music = GetComponents<AudioSource>()[4];
		respawnSound = GetComponents<AudioSource>()[5];

		// set up checkpoints
		level1RespawnPoints.Add(new Vector3(462.1642f, -0.390244f, 5.319216e-07f));
		level2RespawnPoints.Add(new Vector3(-20.4133f, 6.748564f, 0.01558677f));
		level3RespawnPoints.Add(new Vector3(-14.74031f, 2.606536f, 0.01558606f));

		SetUpForNewLevel();
		StartLevel(currentLevel);
	}

	void SetUpForNewLevel()
	{
		// cache references
		player = GameObject.Find("Lucina");
		playerScript = player.GetComponent<PlayerScript>();
		playerScript.SetCamFollowPlayer(true);

		// do not allow certain objects to be sucked in by gravity
		Physics.IgnoreLayerCollision(13, 9, true); // player
		Physics.IgnoreLayerCollision(13, 8, true); // reticle
		Physics.IgnoreLayerCollision(13, 11, true); // gravity centers

		// do not allow spikes platform to be stopped
		Physics.IgnoreLayerCollision(17, 18, true);

		// Create game hud
		GameObject gameHUD = new GameObject("GameHUD");
		gameHUD.AddComponent<GameHUD>();

		// reset variables
		totalHealth = MAX_HEALTH;
		currentSpawnPoint = 0;
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
			totalHealth = MAX_HEALTH;
			hasLost = false;
			hasWon = false;
			RespawnPlayer();
		}
		if (Input.GetKey (KeyCode.Escape)) {
			Application.Quit();
			Debug.Log ("Application.Quit() only works in build, not in editor"); 
		}
	}

	public void StartLevel(int levelNum) 
	{
		if (levelNum == 1)
		{
			playerScript.GetComponent<Level2Script>().enabled = false;
			playerScript.GetComponent<Level3Script>().enabled = false;
			playerScript.GetComponent<Level1Script>().enabled = true;
			playerScript.SetCameraYPlus(CAM_Y_POS_PLUS_LEVEL_1);
			playerScript.SetCameraZPosition(CAM_Z_POS_LEVEL_1);
			playerScript.SetReticleZPosition(RETICLE_Z_POS_LEVEL_1);
			level3Music.Stop();
			level1Music.Play();
			currentRespawnPoints = level1RespawnPoints;
		}
		else if (levelNum == 2)
		{
			Application.LoadLevel("Level2"); 
		}
		else if (levelNum == 3)
		{
			Application.LoadLevel("FinalLevel"); 
		}
	}

	void OnLevelWasLoaded(int level)
	{
		if (level == 2)
		{
			SetUpForNewLevel();
			playerScript.GetComponent<Level1Script>().enabled = false;
			playerScript.GetComponent<Level3Script>().enabled = false;
			playerScript.GetComponent<Level2Script>().enabled = true;
			playerScript.SetCameraYPlus(CAM_Y_POS_PLUS_LEVEL_2);
			playerScript.SetCameraZPosition(CAM_Z_POS_LEVEL_2);
			playerScript.SetReticleZPosition(RETICLE_Z_POS_LEVEL_2);
			level1Music.Stop();
			level3Music.Stop();
			level2Music.Play();
			currentRespawnPoints = level2RespawnPoints;
		}
		else if (level == 3)
		{
			SetUpForNewLevel();
			playerScript.GetComponent<Level1Script>().enabled = false;
			playerScript.GetComponent<Level2Script>().enabled = false;
			playerScript.GetComponent<Level3Script>().enabled = true;
			playerScript.SetCameraYPlus(CAM_Y_POS_PLUS_LEVEL_3);
			playerScript.SetCameraZPosition(CAM_Z_POS_LEVEL_3);
			playerScript.SetReticleZPosition(RETICLE_Z_POS_LEVEL_3);
			level3StartSound.Play();
			level1Music.Stop();
			level2Music.Stop();
			level3Music.Play();
			currentRespawnPoints = level3RespawnPoints;
		}
		else if (level == 4)
		{

		}
		else if (level == 5)
		{

		}
	}

	public void TakeDamage()
	{
		totalHealth--;

		if (totalHealth <= 0)
		{
			LoseLife();
		}
	}

	public void LoseLife() 
	{
		totalLives--;
		deathSound.Play();

		if (totalLives <= 0) 
		{
			Die();
		}
		else 
		{
			totalHealth = MAX_HEALTH;

			// respawn player
			RespawnPlayer();
		}
	}

	public void Die()
	{
		hasLost = true;
		Application.LoadLevel("GameOver2"); 
	}

	void RespawnPlayer()
	{
		player.transform.position = currentRespawnPoints[currentSpawnPoint];
		GameObject respawnEffectGO = (GameObject)Instantiate(respawnEffect, player.transform.position, Quaternion.identity);
		Destroy(respawnEffectGO, 3.0f);
		respawnSound.Play();
	}

	public void Lose()
	{
		hasLost = true;
	}

	public void Win()
	{
		hasWon = true;
		Application.LoadLevel("WinScene"); 
	}

	// Getters
	public int GetTotalLives() { return totalLives; }
	public int GetMaxLives() { return MAX_LIVES; }
	public int GetTotalHealth() { return totalHealth; }
	public int GetMaxHealth() { return MAX_HEALTH; }
	public bool GetHasLevitation() { return hasLevitation; }
	public bool GetHasGravity() { return hasGravity; }
	public bool GetHasElectricity() { return hasElectricity; }
	public bool GetHasLost() { return hasLost; }
	public bool GetHasWon() { return hasWon; }
	public void SetCurrentLevel(int level) { currentLevel = level; }
	public void SetHasLevitation(bool b) { hasLevitation = b; }
	public void SetHasGravity(bool b) { hasGravity = b; }
	public void SetHasElectricity(bool b) { hasElectricity = b; }
}
