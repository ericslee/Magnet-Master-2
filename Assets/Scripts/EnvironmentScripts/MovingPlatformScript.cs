using UnityEngine;
using System.Collections;

public enum PlatformMovementPath {horizontal, vertical};

// Use for moving platforms/gravity centers/etc

public class MovingPlatformScript : MonoBehaviour {

	public float movementSpeed = 2.0f;
	public bool isMovingDefaultDirection;
	public float movementRadius;
	public PlatformMovementPath movementPath = PlatformMovementPath.horizontal;
	public Vector3 initialPos;

	float minBound;
	float maxBound;
	Vector2 translateVecNormalized;
	float currentPos;

	void Start() 
	{
		isMovingDefaultDirection = true;
		initialPos = transform.position;

		if (movementPath.Equals(PlatformMovementPath.horizontal)) 
		{
			translateVecNormalized = Vector2.right;
			minBound = initialPos.x - movementRadius;
			maxBound = initialPos.x + movementRadius;
		}
		else if (movementPath.Equals(PlatformMovementPath.vertical)) 
		{
			translateVecNormalized = Vector2.up;
			minBound = initialPos.y - movementRadius;
			maxBound = initialPos.y + movementRadius;
		}
	}
	
	void Update() 
	{
		if (movementPath.Equals(PlatformMovementPath.horizontal)) currentPos = transform.position.x;
		else if (movementPath.Equals(PlatformMovementPath.vertical)) currentPos = transform.position.y;

		Move();
	}

	void Move()
	{
		if (currentPos <= minBound) {
			isMovingDefaultDirection = true;
		}
		
		if (currentPos >= maxBound) {
			isMovingDefaultDirection = false;
		}
		
		if (isMovingDefaultDirection) {
			transform.Translate(translateVecNormalized * movementSpeed * Time.deltaTime);
		} else {
			transform.Translate(-translateVecNormalized * movementSpeed * Time.deltaTime);
		}
	}
}
