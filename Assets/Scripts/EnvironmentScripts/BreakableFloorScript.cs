using UnityEngine;
using System.Collections;

public class BreakableFloorScript : MonoBehaviour {

	GameObject player;
	Level2Script level2Script;

	void Start() 
	{
		player = GameObject.Find("Lucina");
		level2Script = player.GetComponent<Level2Script>();
	}
	
	void Update() 
	{
	
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name.Equals("Weight"))
		{
			if (collision.relativeVelocity.magnitude > 2)
			{
				level2Script.PlayThump();

				if (collision.relativeVelocity.magnitude > 7)
				{
					level2Script.BreakFloor(gameObject.transform.position);
					Destroy(gameObject);
				}
			}
		}
	}
}
