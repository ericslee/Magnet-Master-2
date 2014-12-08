using UnityEngine;
using System.Collections;

public class SpikeFieldPlatformScript : MonoBehaviour {

	bool inMotion = false;
	float movementSpeed = 2.0f;

	void Start() 
	{
	
	}
	
	void Update() 
	{
		if (inMotion)
		{
			if (transform.position.x < 70)
			{
				transform.Translate(Vector2.right * movementSpeed * Time.deltaTime);
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag.Equals("Player"))
		{
			Vector3 hit = collision.contacts[0].normal;
			if (Vector3.Dot(hit,Vector3.up) < 0)
			{
				inMotion = true;
			}
		}
	}
}
