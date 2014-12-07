using UnityEngine;
using System.Collections;

public class BreakableFloorScript : MonoBehaviour {

	void Start() 
	{
	
	}
	
	void Update() 
	{
	
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name.Equals("Weight"))
		{
			if (collision.relativeVelocity.magnitude > 8)
			{
				Destroy(gameObject);
			}
		}
	}
}
