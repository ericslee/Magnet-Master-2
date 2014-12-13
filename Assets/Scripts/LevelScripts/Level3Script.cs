using UnityEngine;
using System.Collections;

public class Level3Script : MonoBehaviour {

	GameManager gameManager;
	PlayerScript playerScript;

	// Wall Trigger
	bool wallDrop = false;
	bool crushingWallDrop = false;

	// Fire Walls Trigger
	bool fireWall = false;
	
	// Enemy Shooting Fire Balls Trigger
	bool fireBall = false;
	
	// Key Drop Trigger
	bool keyDrop = false;

	// Level 3 enemies
	GameObject enemyOne;
	GameObject enemyTwo;

	// puzzle references
	GameObject key;
	GameObject wall1;
	GameObject wall2;
	GameObject wall3;

	// audio sound
	AudioSource powerCollectSFX;

	TutorialTextScript electricityText;
	Vector3 electricityTextPosition;

	void Start() 
	{
		// cache references
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		enemyOne = GameObject.Find("Enemies").transform.GetChild(0).transform.GetChild(0).gameObject;
		enemyTwo = GameObject.Find("Enemies").transform.GetChild(1).gameObject;
		powerCollectSFX = GetComponents<AudioSource>()[13];
		playerScript = GetComponent<PlayerScript>();
		electricityText = GameObject.Find("ElectricityText").GetComponent<TutorialTextScript>();;
		electricityTextPosition = new Vector3(464.1634f, -0.582355f, 0.6604137f);
		wall1 = GameObject.FindWithTag("Wall1");
		wall2 = GameObject.FindWithTag("Wall2");
		wall3 = GameObject.FindWithTag("Wall3");
	}

	void Update () 
	{
		HandleObstacles();
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag.Equals("WallDrop") && !crushingWallDrop) 
		{
			wallDrop = true;
			crushingWallDrop = true;
			playerScript.panicVoice.PlayDelayed(1.0f);
		} 
		else if (c.tag.Equals("FireWall") && !fireWall) 
		{
			fireWall = true;
			enemyOne.GetComponent<EnemyScript>().StartAttacking();
		} 
		else if (c.tag.Equals("FireBall") && !fireBall) 
		{
			fireBall = true;
			enemyTwo.GetComponent<MovingEnemyScript>().StartAttacking();
		} 
		else if (c.tag.Equals("KeyDrop")) 
		{
			keyDrop = true;
		} 
		else if (c.tag.Equals ("FinalDoor")) 
		{
			//Level over
			gameManager.Win();
			Debug.Log("LEVEL COMPLETE");
		}
		else if (c.tag.Equals("Hazard") && playerScript.GetInvincibilityFrames() > 100)
		{
			playerScript.TakeDamage(playerScript.normalKnockback);
		}
		else if (c.tag.Equals("Lava") && playerScript.GetInvincibilityFrames() > 100)
		{
			playerScript.TakeDamage(playerScript.lavaKnockback);
			playerScript.SetOnFire();
		}
		else if (c.gameObject.name.Equals("Checkpoint_1_Final_Level"))
		{
			gameManager.SetCurrentCheckpoint(1);
		}
		else if (c.gameObject.name.Equals("Checkpoint_2_Final_Level"))
		{
			gameManager.SetCurrentCheckpoint(2);
		}
	}

	void HandleObstacles() {
		if (wallDrop) {
			if (wall1.transform.position.y < 1) {
				wall1.transform.Translate(Vector2.up * 12f * Time.deltaTime);
			} else {
				wallDrop = false;
			}
			if (wall3.transform.position.y > 1) {
				wall3.transform.Translate(-Vector2.up * 12f * Time.deltaTime);
			} else {
				wallDrop = false;
			}
		}
		if (crushingWallDrop) {
			if (wall2.transform.position.y > -14) {
				wall2.transform.Translate(new Vector3(0, 0, -1f) * 1f * Time.deltaTime);
			} 
		}
		if (fireWall) {
			//Handle particle system fire here
		}
	}
}
