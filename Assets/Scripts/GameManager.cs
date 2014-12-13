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
	int currentCheckpoint = 0;
	List<Vector3> currentRespawnPoints;
	GameObject respawnEffect;
	string currentLevelString;
	bool onLevel1 = false;
	bool onLevel2 = false;
	bool onLevel3 = false;

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
	public SkinnedMeshRenderer lucinaRenderer;
	public GameObject lucinaMesh;

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
		level1RespawnPoints.Add(new Vector3(462.1642f, -0.390244f, 0));
		level2RespawnPoints.Add(new Vector3(-20.4133f, 6.748564f, 0));
		level2RespawnPoints.Add(new Vector3(19.67843f, 10.55312f, 0));
		level2RespawnPoints.Add(new Vector3(68.57932f, 7.761245f, 0));
		level3RespawnPoints.Add(new Vector3(-14.74031f, 2.606536f, 0));
		level3RespawnPoints.Add(new Vector3(48.50527f, 3.938205f, 0));
		level3RespawnPoints.Add(new Vector3(174.3493f, 1.867743f, 0));

		SetUpForNewLevel();
		StartLevel(currentLevel);
		currentLevelString = "IntroScene";
	}

	void SetUpForNewLevel()
	{
		// cache references
		player = GameObject.Find("Lucina");
		playerScript = player.GetComponent<PlayerScript>();
		playerScript.SetCamFollowPlayer(true);
		lucinaRenderer = player.GetComponentInChildren<SkinnedMeshRenderer>();
		lucinaMesh = player.transform.GetChild(1).gameObject;

		// do not allow certain objects to be sucked in by gravity
		Physics.IgnoreLayerCollision(13, 9, true); // player
		Physics.IgnoreLayerCollision(13, 8, true); // reticle
		Physics.IgnoreLayerCollision(13, 11, true); // gravity centers

		// do not allow spikes platform to be stopped
		Physics.IgnoreLayerCollision(17, 18, true);

		// Create game hud
		GameObject gameHUD = new GameObject("GameHUD");
		gameHUD.AddComponent<GameHUD>();
	}

	void ResetVariables()
	{
		// reset variables
		totalHealth = MAX_HEALTH;
		currentCheckpoint = 0;
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
		if (Input.GetKey(KeyCode.Escape)) 
		{
			Application.Quit();
			Debug.Log("Application.Quit() only works in build, not in editor"); 
		}
		if (Input.GetKey(KeyCode.Alpha4))
		{
			hasLevitation = true;
			hasGravity = true;
			hasElectricity = true;
			if (currentLevel == 1)
			{
				player.transform.position = new Vector3(490.9159f, -1.954842f, 0);
			}
			else if (currentLevel == 2)
			{
				player.transform.position = level2RespawnPoints[2];
			}
			else if (currentLevel == 3)
			{
				player.transform.position = level3RespawnPoints[2];
			}
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
			currentLevelString = "Level2";
			currentLevel = 2;
		}
		else if (levelNum == 3)
		{
			Application.LoadLevel("FinalLevel"); 
			currentLevelString = "FinalLevel";
			currentLevel = 3;
		}
	}

	void OnLevelWasLoaded(int level)
	{
		if (level == 1)
		{
			if (!onLevel1)
			{
				onLevel1 = true;
			}
			else
			{
				SetUpForNewLevel();

				// don't show Lucina until the respawn sequence occurs
				player.transform.position = currentRespawnPoints[currentCheckpoint];
				player.rigidbody.isKinematic = true;
				player.GetComponentInChildren<Renderer>().enabled = false;
				lucinaMesh.SetActive(false);
				StartCoroutine(RespawnPlayer());

				Destroy(gameObject);
			}
		}
		if (level == 2)
		{
			SetUpForNewLevel();
			playerScript.GetComponent<Level1Script>().enabled = false;
			playerScript.GetComponent<Level3Script>().enabled = false;
			playerScript.GetComponent<Level2Script>().enabled = true;
			playerScript.SetCameraYPlus(CAM_Y_POS_PLUS_LEVEL_2);
			playerScript.SetCameraZPosition(CAM_Z_POS_LEVEL_2);
			playerScript.SetReticleZPosition(RETICLE_Z_POS_LEVEL_2);
			if (!onLevel2)
			{
				ResetVariables();
				level1Music.Stop();
				level3Music.Stop();
				level2Music.Play();
				currentRespawnPoints = level2RespawnPoints;
				onLevel2 = true;
			}
			else
			{
				// don't show Lucina until the respawn sequence occurs
				player.transform.position = currentRespawnPoints[currentCheckpoint];
				player.rigidbody.isKinematic = true;
				player.GetComponentInChildren<Renderer>().enabled = false;
				lucinaMesh.SetActive(false);
				StartCoroutine(RespawnPlayer());
			}
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
			if (!onLevel3)
			{
				ResetVariables();
				SetUpForNewLevel();
				//level3StartSound.PlayDelayed(1);
				level1Music.Stop();
				level2Music.Stop();
				level3Music.Play();
				currentRespawnPoints = level3RespawnPoints;
				onLevel3 = true;
			}
			else 
			{
				// don't show Lucina until the respawn sequence occurs
				player.transform.position = currentRespawnPoints[currentCheckpoint];
				player.rigidbody.isKinematic = true;
				player.GetComponentInChildren<Renderer>().enabled = false;
				lucinaMesh.SetActive(false);
				StartCoroutine(RespawnPlayer());
			}
		}
		else if (level == 4)
		{
			Destroy(gameObject);
		}
		else if (level == 5)
		{
			Destroy(gameObject);
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

			// reload level so puzzles reset
			Application.LoadLevel(currentLevelString);
		}
	}

	public void Die()
	{
		hasLost = true;
		Application.LoadLevel("GameOver2"); 
	}

	IEnumerator RespawnPlayer()
	{
		yield return new WaitForSeconds(0.5f);
		player.transform.position = currentRespawnPoints[currentCheckpoint];
		GameObject respawnEffectGO = (GameObject)Instantiate(respawnEffect, player.transform.position, Quaternion.identity);
		Destroy(respawnEffectGO, 3.0f);
		respawnSound.Play();
		player.rigidbody.isKinematic = false;
		lucinaMesh.SetActive(true);
		lucinaRenderer.enabled = true;
		playerScript.SetLucinaRenderer();

		yield return null;
	}

	public void Lose()
	{
		hasLost = true;
	}

	public void Win()
	{
		hasWon = true;
		Application.LoadLevel("WinScene2"); 
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
	public void SetCurrentCheckpoint(int checkpoint) 
	{ 
		if (currentCheckpoint < checkpoint) currentCheckpoint = checkpoint; 
	}
}
