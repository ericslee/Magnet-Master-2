using UnityEngine;
using System.Collections;

public class ClampVelocityScript : MonoBehaviour {

	public const float MAX_SPEED = 5;

	void Start() 
	{
	
	}
	
	void Update() 
	{
	
	}

	void FixedUpdate()
	{
		// clamp velocity if necessary
		if (!rigidbody.isKinematic)
		{
			if (rigidbody)
			{
				Vector3 v = rigidbody.velocity;
				if(v.magnitude > MAX_SPEED)
				{ 
					rigidbody.velocity = v.normalized * MAX_SPEED; 
				}
			}
		}
	}
}
