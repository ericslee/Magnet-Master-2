using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PowerType {Levitation, Gravity, Electricity};

public class PlayerScript : MonoBehaviour
{
	public float jumpHeight;
	int health;

	//Sound
	AudioSource jumpSource;
	AudioSource takeDamageSource;
	AudioSource platformGunFireSource;
	AudioSource gravityGunFireSource;
	
	// Controls
	float distToGround;
	bool collidingWall; // used for disabling left, right controls when colliding with a wall

	Quaternion frontRotation;
	Quaternion leftRotation;
	Quaternion rightRotation;

	// Use this for initialization
	void Start()
	{
		// get distance to ground
		distToGround = collider.bounds.extents.y;
		jumpHeight = 8.0f;
		
		frontRotation = Quaternion.Euler(0,180,0);
		leftRotation = Quaternion.Euler(0,-90,0);
		rightRotation = Quaternion.Euler(0,90,0);
	}
	
	void Update()
	{
		HandleInput();
	}
	
	void HandleInput()
	{
		HandleMovement();
	}
	
	void HandleMovement()
	{
		// change character direction if necessary
		Vector3 pos = Input.mousePosition;
		pos.z = transform.position.z - Camera.main.transform.position.z;
		if (transform.rotation != rightRotation && Camera.main.ScreenToWorldPoint(pos).x > transform.position.x) 
		{
			transform.rotation = rightRotation;
		}
		else if (transform.rotation != leftRotation && Camera.main.ScreenToWorldPoint(pos).x < transform.position.x)
		{
			transform.rotation = leftRotation;
		}
		
		if (!collidingWall)
		{
			if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
			{
				transform.rotation = Quaternion.identity;
				transform.Translate(Vector2.right * 4f * Time.deltaTime);
				transform.rotation = rightRotation;
			}
			else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
			{
				transform.rotation = Quaternion.identity;
				transform.Translate(-Vector2.right * 4f * Time.deltaTime);
				transform.rotation = leftRotation;
			}
		}
		
		// Jump
		if (IsGrounded())
		{
			Quaternion rotation = transform.rotation;
			transform.rotation = Quaternion.identity;
			collidingWall = false;
			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
			{
				rigidbody.velocity = new Vector3(0, jumpHeight, 0);
			}
			transform.rotation = rotation;
		}

		// FOR DEBUGGING, multi jump
		if (Input.GetKey(KeyCode.U))
		{
			rigidbody.velocity = new Vector3(0, 8, 0);
		}
	}
	

	
	// checks if player is grounded with three tiny raycasts from the left bound, center, and right bound of the collider
	bool IsGrounded()
	{
		Vector3 leftBound = new Vector3(transform.position.x - (GetComponent<BoxCollider>().size.x / 2), transform.position.y, transform.position.z);
		Vector3 rightBound = new Vector3(transform.position.x + (GetComponent<BoxCollider>().size.x / 2), transform.position.y, transform.position.z);
		
		return (Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f)
		        || (Physics.Raycast(leftBound, -Vector3.up, distToGround + 0.1f))
		        || (Physics.Raycast(rightBound, -Vector3.up, distToGround + 0.1f)));
	}
	
	void OnCollisionEnter(Collision collision)
	{ 
		// environment tag needed in case we want to be able to control 
		if (collision.gameObject.tag.Equals("Environment") && !IsGrounded())
		{
			collidingWall = true;
		}
	}
	
	void OnCollisionExit()
	{
		collidingWall = false;
	}

	public int getHealth(){
		return health;
	}
	
	public void setHealth(int newHealth){
		health = newHealth;
	}
}
