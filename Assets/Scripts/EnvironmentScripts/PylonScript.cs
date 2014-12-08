using UnityEngine;
using System.Collections;

public class PylonScript : MonoBehaviour {

	GameObject player;
	Level2Script level2Script;
	public GameObject door;

	// Sound
	AudioSource airlockSFX;

	void Start() 
	{
		player = GameObject.Find("Lucina");
		level2Script = player.GetComponent<Level2Script>();
		door = GameObject.Find("door");

		// open door
		level2Script.OpenDoor();
		door.GetComponent<Animation>().Play("open");

		// play sound
		airlockSFX = GetComponents<AudioSource>()[0];
		if (airlockSFX) airlockSFX.Play();
	}
	
	void Update() 
	{
	
	}
}
