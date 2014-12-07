using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	const int MAX_LIVES = 5;
	const int START_LEVEL = 1;
	const float CAM_Y_POS_PLUS_LEVEL_1 = 2;
	const float CAM_Y_POS_PLUS_LEVEL_2 = 0;
	const float CAM_Y_POS_PLUS_LEVEL_3 = 0;
	const float CAM_Z_POS_LEVEL_1 = -10.0f;
	const float CAM_Z_POS_LEVEL_2 = -20.0f;
	const float CAM_Z_POS_LEVEL_3 = -20.0f;
	const float RETICLE_Z_POS_LEVEL_1 = -2.0f;
	const float RETICLE_Z_POS_LEVEL_2 = -2.0f;
	const float RETICLE_Z_POS_LEVEL_3 = -2.0f;

	public int currentLevel = START_LEVEL;
	int totalLives = MAX_LIVES;

	// Game state
	bool hasLost = false;
	bool hasWon = false;

	// Power state
	bool hasLevitation = false;
	bool hasGravity = false;
	bool hasElectricity = false;

	// Checkpoints
	Vector3 initialSpawnPoint = new Vector3(-15.03494f, 3.715919f, 0.374071f);

	// Sound
	AudioSource deathSound;
	AudioSource level1Music;
	AudioSource level2Music;
	AudioSource level3Music;
	AudioSource level3StartSound;

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
		// set up sounds
		deathSound = GetComponents<AudioSource>()[0];
		level3Music = GetComponents<AudioSource>()[1];
		level1Music = GetComponents<AudioSource>()[3];
		level3StartSound = GetComponents<AudioSource>()[2];
		level2Music = GetComponents<AudioSource>()[4];

		SetUpForNewLevel();
		StartLevel(currentLevel);
	}

	void SetUpForNewLevel()
	{
		// cache references
		player = GameObject.Find("Lucina");
		playerScript = player.GetComponent<PlayerScript>();

		// do not allow certain objects to be sucked in by gravity
		Physics.IgnoreLayerCollision(13, 9, true); // player
		Physics.IgnoreLayerCollision(13, 8, true); // reticle
		Physics.IgnoreLayerCollision(13, 11, true); // gravity centers

		// Create game hud
		GameObject gameHUD = new GameObject("GameHUD");
		gameHUD.AddComponent<GameHUD>();
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
		SetUpForNewLevel();

		if (level == 2)
		{
			playerScript.GetComponent<Level1Script>().enabled = false;
			playerScript.GetComponent<Level3Script>().enabled = false;
			playerScript.GetComponent<Level2Script>().enabled = true;
			playerScript.SetCameraYPlus(CAM_Y_POS_PLUS_LEVEL_2);
			playerScript.SetCameraZPosition(CAM_Z_POS_LEVEL_2);
			playerScript.SetReticleZPosition(RETICLE_Z_POS_LEVEL_2);
			level1Music.Stop();
			level3Music.Stop();
			level2Music.Play();
		}
		else if (level == 3)
		{
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
		}
	}

	public void LoseLife() 
	{
		totalLives--;
		deathSound.Play();

		// respawn player
		// RespawnPlayer();

		if (totalLives <= 0) 
		{
			Die();
		}
	}

	public void Die()
	{
		hasLost = true;
		Application.LoadLevel("GameOver2"); 
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
		Application.LoadLevel("WinScene"); 
	}

	// Getters
	public int GetTotalLives() { return totalLives; }
	public int GetMaxLives() { return MAX_LIVES; }
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
