using UnityEngine;
using System.Collections;

public class Level1Script : MonoBehaviour {

	GameManager gameManager;

	void Start() 
	{
		// cache references
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
			gameManager.StartLevel(3);
		}
	}
}
