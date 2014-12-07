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

	void Start() 
	{
		// cache references
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		player = GameObject.Find("Lucina");
		playerScript = player.GetComponent<PlayerScript>();
		keyCollectSFX = GetComponents<AudioSource>()[11];
		doorRattleSFX = GetComponents<AudioSource>()[12];
		powerCollectSFX = GetComponents<AudioSource>()[13];
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
		}
		else if (c.gameObject.name.Equals("GravityPower"))
		{
			gameManager.SetHasGravity(true);
			Destroy(c.gameObject);
			if (powerCollectSFX) powerCollectSFX.Play();
		}
		else if (c.tag.Equals("Door"))
		{
			if (keyCollected)
			{
				gameManager.StartLevel(3);
			}
			else 
			{
				if (doorRattleSFX) doorRattleSFX.Play();
			}
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
