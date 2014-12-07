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

	void Start() 
	{
		// cache references
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		enemyOne = GameObject.Find("Enemies").transform.GetChild(0).transform.GetChild(0).gameObject;
		enemyTwo = GameObject.Find("Enemies").transform.GetChild(1).transform.GetChild(0).gameObject;
		playerScript = GetComponent<PlayerScript>();

		wall1 = GameObject.FindWithTag("Wall1");
		wall2 = GameObject.FindWithTag("Wall2");
		wall3 = GameObject.FindWithTag("Wall3");
		key = GameObject.FindWithTag("Key");
	}

	void Update () 
	{
		HandleObstacles();
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag.Equals("WallDrop") && !crushingWallDrop) {
			wallDrop = true;
			crushingWallDrop = true;
			playerScript.panicVoice.PlayDelayed(1.0f);
		} else if (c.tag.Equals("FireWall")) {
			fireWall = true;
			enemyOne.GetComponent<EnemyScript>().StartAttacking();
		} else if (c.tag.Equals("FireBall")) {
			enemyTwo.GetComponent<EnemyScript>().StartAttacking();
		} else if (c.tag.Equals("KeyDrop")) {
			keyDrop = true;
		} else if (c.tag.Equals ("FinalDoor")) {
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
			if (wall2.transform.position.y > 10) {
				wall2.transform.Translate(-Vector2.up * 1f * Time.deltaTime);
			} 
		}
		if (fireWall) {
			//Handle particle system fire here
		}
		if (keyDrop) {
			key.transform.Translate(-Vector2.up * 4f * Time.deltaTime);
		}
	}
}
