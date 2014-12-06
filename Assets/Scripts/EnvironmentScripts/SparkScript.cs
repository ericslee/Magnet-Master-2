using UnityEngine;
using System.Collections;

public class SparkScript : MonoBehaviour {

	// sounds
	AudioSource sparkOneSFXSource;
	AudioSource sparkTwoSFXSource;
	AudioSource sparkThreeSFXSource;

	public GameObject player;
	bool playerInRange;

	void Start () 
	{
		sparkOneSFXSource = gameObject.AddComponent<AudioSource>();
		sparkTwoSFXSource = gameObject.AddComponent<AudioSource>();
		sparkThreeSFXSource = gameObject.AddComponent<AudioSource>();

		sparkOneSFXSource.clip = Resources.Load("Sounds/Spark1") as AudioClip;
		sparkOneSFXSource.playOnAwake = false;
		sparkOneSFXSource.maxDistance = 200.0f;
		sparkTwoSFXSource.clip = Resources.Load("Sounds/Spark2") as AudioClip;
		sparkTwoSFXSource.playOnAwake = false;
		sparkTwoSFXSource.maxDistance = 200.0f;
		sparkThreeSFXSource.clip = Resources.Load("Sounds/Spark3") as AudioClip;
		sparkThreeSFXSource.playOnAwake = false;
		sparkThreeSFXSource.maxDistance = 200.0f;

		player = GameObject.Find("Lucina");
		playerInRange = false;
	}

	void Update()
	{
		float distance = Mathf.Abs(Vector3.Distance(transform.position, player.transform.position));
		if (distance < 12.0f && !playerInRange)
		{
			Invoke("Spark", 0.5f);
			playerInRange = true;
		} 
		else if (distance > 13.0f && playerInRange)
		{
			playerInRange = false;
		}
	}

	void Spark()
	{
		// play sound
		int soundChoice = Random.Range(0, 3);
		if (soundChoice == 0)
			sparkOneSFXSource.Play();
		else if (soundChoice == 1)
			sparkTwoSFXSource.Play();
		else 
			sparkThreeSFXSource.Play();

		float randomTime = Random.Range(0.5f, 2.0f);
		if (playerInRange)
		{
			Invoke("Spark", randomTime);
		}
	}
}
