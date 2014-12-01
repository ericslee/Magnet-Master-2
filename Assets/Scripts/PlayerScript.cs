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
	int invincibilityFrames;

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
	GravityScript gravScript;
	PowerType currentActivePower;

	Texture reticleHoverNormalTexture;
	Texture reticleHoverGlowTexture;
	Texture reticleHoverGlowRedTexture;
	bool powerIsActive;

	public LayerMask levitationLayerMask;
	public LayerMask gravityLayerMask;
	public LayerMask electricyLayerMask;

	// animation
	protected Animator animator;
	static int idleState = Animator.StringToHash("Base Layer.idle");
	static int walkState = Animator.StringToHash("Base Layer.walk");
	static int jumpState = Animator.StringToHash("Base Layer.jump");
	static int pantsuState = Animator.StringToHash("Base Layer.pantsu");
	SkinnedMeshRenderer lucinaRenderer;

	// Level 2 enemies
	GameObject enemyOne;
	GameObject enemyTwo;

	// Use this for initialization
	void Start()
	{
		// load resources
		targetingReticlePrefab = Resources.Load("Prefabs/Reticle");

		// cache references
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		levScript = GetComponent<LevitationScript>();
		elecScript = GetComponent<ElectricityScript>();
		gravScript = GetComponent<GravityScript>();
		animator = GetComponent<Animator>();
		lucinaRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

		enemyOne = GameObject.Find("Enemy1").transform.GetChild(0).gameObject;
		enemyTwo = GameObject.Find("Enemy2").transform.GetChild(0).gameObject;

		// get distance to ground
		//distToGround = collider.bounds.extents.y;
		distToGround = 1.0f;
		jumpHeight = 100.0f;
		invincibilityFrames = 100;
		
		frontRotation = Quaternion.Euler(0,180,0);
		leftRotation = Quaternion.Euler(0,-90,0);
		rightRotation = Quaternion.Euler(0,90,0);

		// instantiate recticle
		Vector3 reticlePosition = new Vector3(transform.position.x + 4f, transform.position.y + 3, transform.position.z);   
		targetingReticle = (GameObject)Instantiate(targetingReticlePrefab, reticlePosition, Quaternion.Euler(90, 0, -2));	

		// setup powers
		currentActivePower = PowerType.Levitation;

		// for reticle glow
		reticleHoverGlowTexture = Resources.Load("Materials/Textures/reticle-glow") as Texture;
		reticleHoverNormalTexture = Resources.Load("Materials/Textures/reticle-normal") as Texture;
		reticleHoverGlowRedTexture = Resources.Load("Materials/Textures/reticle-glow-red") as Texture;
	}

	void Update()
	{
		HandleInput();
		HandleObstacles();
	}
	
	void HandleInput()
	{
		HandleMovement();
		HandlePowersHover();
		HandlePowersInput();
		HandleDamage();

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

				if (animator.GetCurrentAnimatorStateInfo(0).nameHash != jumpState) 
				{
					animator.SetBool("Walking", true);
				}
			}
			else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
			{
				transform.rotation = Quaternion.identity;
				transform.Translate(-Vector2.right * 4f * Time.deltaTime);
				transform.rotation = leftRotation;

				animator.SetBool("Walking", true);
			}
		}

		if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D) 
		    || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
		{
			animator.SetBool("Walking", false);
		}

		Jump();

		// FOR DEBUGGING, multi jump
		if (Input.GetKey(KeyCode.U))
		{
			rigidbody.velocity = new Vector3(0, 8, 0);
		}

		// FOR DEBUGGING, trapping walls go back
		if (Input.GetKey(KeyCode.H))
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
					powerIsActive = true;
				}

			}
			if (Input.GetMouseButtonUp(0)) {
				powerIsActive = false;
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

	void HandlePowersHover() {
		// check to see if reticle is hovering over anything the current power can be applied to
		
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
    
		Texture currTex = targetingReticle.renderer.material.GetTexture("_MainTex");
		Debug.Log (currTex);
	    if (Physics.Raycast(ray, out hit, 100, currentMask) && !powerIsActive)
	    {
			if (currTex.name != "reticle-glow")
			{
				targetingReticle.renderer.material.SetTexture("_MainTex", reticleHoverGlowTexture);
			}
	    }
		else if (powerIsActive)
		{
			if (currTex.name != "reticle-glow-red")
			{
				targetingReticle.renderer.material.SetTexture("_MainTex", reticleHoverGlowRedTexture);
            }
		}
		else
		{
			if (currTex.name != "reticle-normal")
            {
			targetingReticle.renderer.material.SetTexture("_MainTex", reticleHoverNormalTexture);
			}
		}
    }

	void HandleDamage()
	{
		invincibilityFrames++;

		// flash character during invincibility
		if (invincibilityFrames < 100)
		{
			if(lucinaRenderer.enabled)
				lucinaRenderer.enabled = false;
			else
				lucinaRenderer.enabled = true;
		}
		else if (!lucinaRenderer.enabled) 
		{
			lucinaRenderer.enabled = true;
		}
	}

	void ActivatePower(GameObject target, float mouseYPos)
	{
		switch (GetCurrentPower())
		{
		case PowerType.Levitation:
			StartCoroutine(levScript.SetCurrentlyLevitatingObj(target, mouseYPos));
			break;
		case PowerType.Gravity:
			gravScript.SetGravityCenter(target, mouseYPos);
			break;
		case PowerType.Electricity:
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
				//rigidbody.AddForce(Vector3.up * 7, ForceMode.VelocityChange);
				rigidbody.velocity = new Vector3(0, 8, 0);
				animator.SetBool("Walking", false);
				animator.SetTrigger("Jumping");
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
		Vector3 transformPositionWithOffset = transform.position;
		transformPositionWithOffset.y = transformPositionWithOffset.y + 1.1f;

		Vector3 leftBound = new Vector3(transform.position.x - (GetComponent<BoxCollider>().size.x / 2), transformPositionWithOffset.y, transform.position.z);
		Vector3 rightBound = new Vector3(transform.position.x + (GetComponent<BoxCollider>().size.x / 2), transformPositionWithOffset.y, transform.position.z);

		return (Physics.Raycast(transformPositionWithOffset, -Vector3.up, distToGround + 0.1f)
		        || (Physics.Raycast(leftBound, -Vector3.up, distToGround + 0.1f))
		        || (Physics.Raycast(rightBound, -Vector3.up, distToGround + 0.1f)));
	}
	
	void OnCollisionEnter(Collision collision)
	{ 
		// if collision with hazardous object, lose life
		if (collision.gameObject.tag.Equals("Hazard") && invincibilityFrames > 100)
		{
			TakeDamage();
		}
		// environment tag needed in case we want to be able to control 
		if (collision.gameObject.tag.Equals("Environment") && !IsGrounded())
		{
			collidingWall = true;
		}

		// platform
		if (collision.gameObject.name.Equals("Platform") || collision.gameObject.name.Equals("Moving Sphere"))
		{
			transform.parent = collision.gameObject.transform ; 
		}
	}

	void OnCollisionStay(Collision collisionInfo) 
	{
		if (collisionInfo.collider.gameObject.tag.Equals("Hazard") && invincibilityFrames > 100)
		{
			TakeDamage();
		}
	}
	
	void OnCollisionExit()
	{
		collidingWall = false;

		transform.parent = null;
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag.Equals("WallDrop")) {
			Debug.Log ("WallDrop");
			wallDrop = true;
		} else if (c.tag.Equals("FireWall")) {
			Debug.Log ("FireWall");
			fireWall = true;
			enemyOne.GetComponent<EnemyScript>().StartAttacking();
		} else if (c.tag.Equals("FireBall")) {
			Debug.Log ("FireBall");
			fireBall = true;
			enemyTwo.GetComponent<EnemyScript>().StartAttacking();
		} else if (c.tag.Equals("KeyDrop")) {
			Debug.Log ("KeyDrop");	
			keyDrop = true;
		} else if (c.tag.Equals ("Door")) {
			//Level over
			gameManager.Win();
			Debug.Log("LEVEL COMPLETE");
		}
		else if (c.tag.Equals("Hazard") && invincibilityFrames > 100)
		{
			TakeDamage ();
		}
	}

	void HandleObstacles() {
		if (wallDrop) {
			GameObject wall1 = GameObject.FindWithTag("Wall1");
			GameObject wall2 = GameObject.FindWithTag("Wall2");
			GameObject wall3 = GameObject.FindWithTag("Wall3");
			if (wall1.transform.position.y < 1) {
				wall1.transform.Translate(Vector2.up * 12f * Time.deltaTime);
			} else { 
				wallDrop = false;
			}
	
			/*if (wall2.transform.position.y > 2) {
				wall2.transform.Translate(-Vector2.up * 4f * Time.deltaTime);}*/
			if (wall3.transform.position.y > 1) {
				wall3.transform.Translate(-Vector2.up * 12f * Time.deltaTime);
			} else {
				wallDrop = false;
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

	void TakeDamage()
	{
		gameManager.LoseLife();
		
		// knock back
		Vector3 knockbackForce;
		if (gameObject.GetComponent<Rigidbody>().velocity.x < 0)
			knockbackForce = new Vector3(-1000, 350, 0);
		else
			knockbackForce = new Vector3(1000, 350, 0);
		gameObject.GetComponent<Rigidbody>().AddForce(knockbackForce);
		
		invincibilityFrames = 0;
	}

	// Getters
	public float GetCurrentPowerGain()
	{
		if (currentActivePower.Equals(PowerType.Levitation))
		{
			return levScript.GetGain();
		}
		else if (currentActivePower.Equals(PowerType.Gravity))
		{
			return gravScript.GetGain();
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

	public float GetCurrentPowerMaxGain()
	{
		if (currentActivePower.Equals(PowerType.Levitation))
		{
			return levScript.GetMaxGain();
		}
		else if (currentActivePower.Equals(PowerType.Gravity))
		{
			return gravScript.GetMaxGain();
		}
		else if (currentActivePower.Equals(PowerType.Electricity))
		{
			return elecScript.GetMaxGain();
		}
		else 
		{
			return 1;
		}
	}

	public PowerType GetCurrentPower() { return currentActivePower; }
	public int GetHealth() { return health; }
	public int GetInvincibilityFrames() { return invincibilityFrames; }
	public void SetHealth(int newHealth) { health = newHealth; }
}
