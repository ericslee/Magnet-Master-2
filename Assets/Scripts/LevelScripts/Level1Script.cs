using UnityEngine;
using System.Collections;

public class Level1Script : MonoBehaviour {

	GameManager gameManager;
	GameObject player;
	PlayerScript playerScript;
	bool keyCollected = false;

	// Sounds
	AudioSource keyCollectSFX;
	AudioSource doorRattleSFX;
	AudioSource powerCollectSFX;

	// Tutorial
	TutorialTextScript movementText;
	TutorialTextScript levitationText;
	TutorialTextScript gravityText;
	TutorialTextScript reticleText;
	Vector3 movementTextPosition;
	Vector3 levitationTextPosition;
	Vector3 gravityTextPosition;
	Vector3 reticleTextPosition;

	void Start() 
	{
		// cache references
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		player = GameObject.Find("Lucina");
		playerScript = player.GetComponent<PlayerScript>();
		movementText = GameObject.Find("MovementText").GetComponent<TutorialTextScript>();
		levitationText = GameObject.Find("LevitationText").GetComponent<TutorialTextScript>();
		gravityText = GameObject.Find("GravityText").GetComponent<TutorialTextScript>();
		reticleText = GameObject.Find("ReticleText").GetComponent<TutorialTextScript>();

		keyCollectSFX = GetComponents<AudioSource>()[11];
		doorRattleSFX = GetComponents<AudioSource>()[12];
		powerCollectSFX = GetComponents<AudioSource>()[13];

		movementTextPosition = new Vector3(464.1634f, -0.582355f, 0.6604137f);
		levitationTextPosition = new Vector3(481.7638f, -0.582355f, 0.660414f);
		reticleTextPosition = new Vector3(492.0935f, -0.070f, 0.660414f);
		gravityTextPosition = new Vector3(506.1514f, -0.582355f, 0.660414f);

		movementText.SetTutorialPosition(movementTextPosition);
		movementText.SetTimeIn(3.0f);
		movementText.EnterText();
	}
	
	void Update() 
	{
	
	}

	void OnTriggerEnter(Collider c) 
	{
		if (c.gameObject.name.Equals("LevitationPower"))
		{
			gameManager.SetHasLevitation(true);
			Destroy(c.gameObject);
			if (powerCollectSFX) powerCollectSFX.Play();

			levitationText.SetTutorialPosition(levitationTextPosition);
			levitationText.SetTimeIn(1.0f);
			levitationText.EnterText();
		}
		else if (c.gameObject.name.Equals("GravityPower"))
		{
			gameManager.SetHasGravity(true);
			Destroy(c.gameObject);
			player.GetComponent<GravityScript>().gravityTargets.Clear();
			if (powerCollectSFX) powerCollectSFX.Play();

			gravityText.SetTutorialPosition(gravityTextPosition);
			gravityText.SetTimeIn(1.0f);
			gravityText.EnterText();
		}
		else if (c.tag.Equals("Door"))
		{
			if (keyCollected)
			{
				gameManager.StartLevel(2);
			}
			else 
			{
				if (doorRattleSFX) doorRattleSFX.Play();
			}
		}
		else if (c.tag.Equals("ReticleTip"))
		{
			reticleText.SetTutorialPosition(reticleTextPosition);
			reticleText.SetTimeIn(1.0f);
			reticleText.EnterText();
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name.Equals("Key"))
		{
			keyCollected = true;
			Destroy(collision.gameObject);
			if (keyCollectSFX) keyCollectSFX.Play();
		}
	}
}
