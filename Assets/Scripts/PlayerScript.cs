using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PowerType {Levitation, Gravity, Electricity};

public class PlayerScript : MonoBehaviour
{
	// Wall Trigger
	bool wallDrop = false;
	bool wallHide = false;
	bool leverTurn = true;

	// Fire Walls Trigger
	bool fireWall = false;

	// Enemy Shooting Fire Balls Trigger
	bool fireBall = false;

	// Key Drop Trigger
	bool keyDrop = false;

	GameManager gameManager;

	public float jumpHeight;
	int health;

	// Controls
	float distToGround;
	bool collidingWall; // used for disabling left, right controls when colliding with a wall
	float jumpValue = 0;

	Quaternion frontRotation;
	Quaternion leftRotation;
	Quaternion rightRotation;

	// Powers
	GameObject targetingReticle;
	Object targetingReticlePrefab;
	LevitationScript levScript;
	ElectricityScript elecScript;
	PowerType currentActivePower;

	public LayerMask levitationLayerMask;
	public LayerMask gravityLayerMask;
	public LayerMask electricyLayerMask;

	// Use this for initialization
	void Start()
	{
		// load resources
		targetingReticlePrefab = Resources.Load("Prefabs/Reticle");

		// cache references
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		levScript = GetComponent<LevitationScript>();
		elecScript = GetComponent<ElectricityScript>();

		// get distance to ground
		distToGround = collider.bounds.extents.y;
		jumpHeight = 100.0f;
		
		frontRotation = Quaternion.Euler(0,180,0);
		leftRotation = Quaternion.Euler(0,-90,0);
		rightRotation = Quaternion.Euler(0,90,0);

		// instantiate recticle
		Vector3 reticlePosition = new Vector3(transform.position.x + 4f, transform.position.y + 3, transform.position.z);   
		targetingReticle = (GameObject)Instantiate(targetingReticlePrefab, reticlePosition, Quaternion.Euler(90, 0, -2));	

		// setup powers
		currentActivePower = PowerType.Levitation;
	}
	
	void Update()
	{
		HandleInput();
		HandleObstacles();
	}
	
	void HandleInput()
	{
		HandleMovement();
		HandlePowersInput();

		// follow player with camera
		Vector3 playerPosition = transform.position;
		playerPosition.z = -20f;
		Camera.main.transform.position = playerPosition;
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

		Jump();

		// FOR DEBUGGING, multi jump
		if (Input.GetKey(KeyCode.U))
		{
			rigidbody.velocity = new Vector3(0, 8, 0);
		}

		// FOR DEBUGGING, trapping walls go back
		if (Input.GetKey (KeyCode.H))
		{
			wallHide = true;
			wallDrop = false;
		}
	}

	void HandlePowersInput() 
	{
		if (targetingReticle) 
		{
			// Move reticle with mouse
			Vector3 pos = Input.mousePosition;
			pos.z = transform.position.z - Camera.main.transform.position.z;
			Vector3 newReticlePos = Camera.main.ScreenToWorldPoint(pos);
			newReticlePos.z = -2;

			targetingReticle.transform.position = newReticlePos;

			if (Input.GetMouseButtonDown(0)) 
			{
				// check to see if reticle is targeting anything

				Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
				RaycastHit hit;

				LayerMask currentMask = levitationLayerMask;

				if (currentActivePower.Equals(PowerType.Gravity)) 
				{
					currentMask = gravityLayerMask;
				}
				else if (currentActivePower.Equals(PowerType.Electricity))
				{
					currentMask = electricyLayerMask;
				}

				if(Physics.Raycast(ray, out hit, 100, currentMask))
				{
					ActivatePower(hit.transform.gameObject, Input.mousePosition.y); 
				}

			}
			if (Input.GetMouseButton(0)) 
			{
			}
			if (Input.GetMouseButtonDown(1)) 
			{
				switch (GetCurrentPower())
				{
					case PowerType.Levitation:
						currentActivePower = PowerType.Gravity;
						break;
					case PowerType.Gravity:
						currentActivePower = PowerType.Electricity;
						break;
					case PowerType.Electricity:
						currentActivePower = PowerType.Levitation;
						break;
				}
			}
		}
	}

	void ActivatePower(GameObject target, float mouseYPos)
	{
		switch (GetCurrentPower())
		{
		case PowerType.Levitation:
			levScript.SetCurrentlyLevitatingObj(target, mouseYPos);
			break;
		case PowerType.Gravity:
			Debug.Log("gravity power");
			break;
		case PowerType.Electricity:
			Debug.Log("electricity power");
			elecScript.SetElectricityTarget(target, mouseYPos);
			break;
		}
	}

	void Jump()
	{
		if (IsGrounded())
		{
			Quaternion rotation = transform.rotation;
			transform.rotation = Quaternion.identity;
			collidingWall = false;
			
			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) 
			{
				rigidbody.AddForce(Vector3.up * 7, ForceMode.VelocityChange);
			}

			transform.rotation = rotation;
		}
		/*
		if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) 
		{
			if (jumpValue < jumpHeight) 
			{
				rigidbody.AddForce(Vector3.up * 5, ForceMode.Acceleration);
				jumpValue++;
			}
			else 
			{
				jumpValue = 0;
			}
		}
		*/
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
		// if collision with hazardous object, lose life
		if (collision.gameObject.tag.Equals("Lava"))
		{
			Debug.Log ("lose life");
			gameManager.LoseLife();
		}
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

	void OnTriggerEnter(Collider c) {
		if (c.tag.Equals("WallDrop")) {
			Debug.Log ("WallDrop");
			wallDrop = true;
		} else if (c.tag.Equals("FireWall")) {
			Debug.Log ("FireWall");
			fireWall = true;
		} else if (c.tag.Equals("FireBall")) {
			Debug.Log ("FireBall");
			fireBall = true;
		} else if (c.tag.Equals("KeyDrop")) {
			Debug.Log ("KeyDrop");	
			keyDrop = true;
		} else if (c.tag.Equals ("Door")) {
			//Level over
			gameManager.Win();
			Debug.Log("LEVEL COMPLETE");
		}
	}

	void HandleObstacles() {
		if (wallHide || wallDrop) {
			GameObject wall1 = GameObject.FindWithTag("Wall1");
			GameObject wall2 = GameObject.FindWithTag("Wall2");
			GameObject wall3 = GameObject.FindWithTag("Wall3");
			if (wallHide) {
				if (leverTurn) {
					GameObject lever = GameObject.FindWithTag("Lever");
					lever.transform.Rotate (Vector3.forward * 90f);
					leverTurn = false;
				}
				if (wall1.transform.position.y > -20) {
					wall1.transform.Translate(-Vector2.up * 12f * Time.deltaTime);
				}
				/*if (wall2.transform.position.y < 20) {
					wall2.transform.Translate(Vector2.up * 4f * Time.deltaTime);
				}*/
				if (wall3.transform.position.y < 20) {
					wall3.transform.Translate(Vector2.up * 12f * Time.deltaTime);
				}
			}
			if (wallDrop) {
				if (wall1.transform.position.y < 1) {
					wall1.transform.Translate(Vector2.up * 12f * Time.deltaTime);
				}
				/*if (wall2.transform.position.y > 2) {
					wall2.transform.Translate(-Vector2.up * 4f * Time.deltaTime);
				}*/
				if (wall3.transform.position.y > 1) {
					wall3.transform.Translate(-Vector2.up * 12f * Time.deltaTime);
				}
			}
		}
		if (fireWall) {
			//Handle particle system fire here
		}
		if (fireBall) {
			//Handle fire ball throwing here
		}
		if (keyDrop) {
			GameObject key = GameObject.FindWithTag("Key");
			key.transform.Translate(-Vector2.up * 4f * Time.deltaTime);
		}
	}

	// Getters
	public float GetCurrentPowerGain()
	{
		if (currentActivePower.Equals(PowerType.Levitation))
		{
			return levScript.GetGain();
		}
		else if (currentActivePower.Equals(PowerType.Electricity))
		{
			return elecScript.GetGain();
		}
		else 
		{
			return 0;
		}
	}

	public PowerType GetCurrentPower() { return currentActivePower; }
	public int GetHealth() { return health; }
	public void SetHealth(int newHealth) { health = newHealth; }
}
