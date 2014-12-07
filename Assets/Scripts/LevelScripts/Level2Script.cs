using UnityEngine;
using System.Collections;

public class Level2Script : MonoBehaviour {

	GameManager gameManager;
	GameObject player;
	PlayerScript playerScript;

	void Start() 
	{
		// cache references
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		player = GameObject.Find("Lucina");
		playerScript = player.GetComponent<PlayerScript>();
	}
	
	void Update () 
	{
	
	}

	void OnTriggerEnter(Collider c) 
	{
		if (c.tag.Equals("Level2Door"))
		{
			gameManager.StartLevel(3);
		}
	}
}
