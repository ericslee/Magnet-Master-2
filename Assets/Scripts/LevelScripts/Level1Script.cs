using UnityEngine;
using System.Collections;

public class Level1Script : MonoBehaviour {

	GameManager gameManager;
	bool keyCollected = false;

	// Sounds
	AudioSource keyCollectSFX;
	AudioSource doorRattleSFX;

	void Start() 
	{
		// cache references
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		keyCollectSFX = GetComponents<AudioSource>()[11];
		doorRattleSFX = GetComponents<AudioSource>()[12];
	}
	
	void Update() 
	{
	
	}

	void OnTriggerEnter(Collider c) 
	{
		if (c.gameObject.name.Equals("LevitationPower"))
		{
			Debug.Log("levitation power obtained");
		}
		else if (c.gameObject.name.Equals("GravityPower"))
		{
			Debug.Log("Gravity power obtained");
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
