using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PowerType {Levitation, Gravity, Electricity};

public class PlayerScript : MonoBehaviour
{
	const float MAX_SPEED = 8;
	const int MAX_INVINCIBILITY_FRAMES = 200;

	GameManager gameManager;

	public float jumpHeight;
	int health;
	int invincibilityFrames;

	// Controls
	float distToGround;
	bool collidingWall; // used for disabling left, right controls when colliding with a wall
	float jumpValue = 0;
	bool isJumping = false;

	Quaternion frontRotation;
	Quaternion leftRotation;
	Quaternion rightRotation;

	float camZPosition = -20;
	float camYPlus = 0;

	// Powers
	GameObject targetingReticle;
	Object targetingReticlePrefab;
	LevitationScript levScript;
	ElectricityScript elecScript;
	GravityScript gravScript;
	PowerType currentActivePower;

	// Reticle
	Texture reticleHoverNormalTexture;
	Texture reticleHoverGlowTexture;
	Texture reticleHoverGlowRedTexture;
	float reticleZPos = -108;
	bool powerIsActive;

	public LayerMask levitationLayerMask;
	public LayerMask gravityLayerMask;
	public LayerMask electricyLayerMask;

	// Damage
	public Vector3 normalKnockback = new Vector3(1000, 350, 0);
	public Vector3 lavaKnockback = new Vector3(1000, 750, 0);
	public Vector3 floorSpikesKnockback = new Vector3(1000, 1000, 0);
	Object onFirePrefab;
	GameObject currentOnFireObject;

	// animation
	protected Animator animator;
	static int idleState = Animator.StringToHash("Base Layer.idle");
	static int walkState = Animator.StringToHash("Base Layer.walk");
	static int jumpState = Animator.StringToHash("Base Layer.jump");
	static int pantsuState = Animator.StringToHash("Base Layer.pantsu");
	SkinnedMeshRenderer lucinaRenderer;

	// Sounds
	AudioSource jumpVoiceOne;
	AudioSource jumpVoiceTwo;
	AudioSource damageVoiceOne;
	AudioSource damageVoiceTwo;
	AudioSource damageVoiceThree;
	AudioSource damageVoiceFour;
	public AudioSource panicVoice;

	// Camera
	Camera guiCamera;
	bool camFollowPlayer = true;

	void Start()
	{
		// load resources
		targetingReticlePrefab = Resources.Load("Prefabs/Reticle");
		onFirePrefab = Resources.Load("Prefabs/OnFire");

		// cache references
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		levScript = GetComponent<LevitationScript>();
		elecScript = GetComponent<ElectricityScript>();
		gravScript = GetComponent<GravityScript>();
		animator = GetComponent<Animator>();
		lucinaRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

		jumpVoiceOne = GetComponents<AudioSource>()[4];
		jumpVoiceTwo = GetComponents<AudioSource>()[5];
		damageVoiceOne = GetComponents<AudioSource>()[6];
		damageVoiceTwo = GetComponents<AudioSource>()[7];
		damageVoiceThree = GetComponents<AudioSource>()[8];
		damageVoiceFour = GetComponents<AudioSource>()[9];
		panicVoice = GetComponents<AudioSource>()[10];

		// get distance to ground
		//distToGround = collider.bounds.extents.y;
		distToGround = 1.0f;
		jumpHeight = 100.0f;
		invincibilityFrames = MAX_INVINCIBILITY_FRAMES;
		
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

		GameObject guiCameraObject = GameObject.Find("GUI Camera");
		if (guiCameraObject) guiCamera = guiCameraObject.camera;
	}

	void Update()
	{
		HandleInput();
		HandleDamage();

		// follow player with camera
		Vector3 playerPosition = GetPlayerCamPosition();
		if (camFollowPlayer)
		{
			Camera.main.transform.position = playerPosition;
		}
		if (guiCamera) guiCamera.transform.position = playerPosition;
	}

	public Vector3 GetPlayerCamPosition()
	{
		Vector3 playerPosition = transform.position;
		playerPosition.y = playerPosition.y + camYPlus;
		playerPosition.z = camZPosition;
		return playerPosition;
	}

	void FixedUpdate()
	{
		// clamp velocity if necessary
		Vector3 v = rigidbody.velocity;
		if(v.magnitude > MAX_SPEED)
		{ 
			rigidbody.velocity = v.normalized * MAX_SPEED; 
		}
	}

	void HandleInput()
	{
		HandleMovement();
		HandlePowersHover();
		HandlePowersInput();
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

				if (animator.GetCurrentAnimatorStateInfo(0).nameHash != jumpState) 
				{
					animator.SetBool("Walking", true);
				}
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
	}

	void HandlePowersInput() 
	{
		if (targetingReticle) 
		{
			// Move reticle with mouse
			Vector3 pos = Input.mousePosition;
			pos.z = transform.position.z - guiCamera.transform.position.z;
			Vector3 newReticlePos = guiCamera.ScreenToWorldPoint(pos);
			newReticlePos.z = reticleZPos;

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
				if (GetCurrentPower().Equals(PowerType.Levitation))
				{
					if (gameManager.GetHasGravity()) currentActivePower = PowerType.Gravity;
					else if (gameManager.GetHasElectricity()) currentActivePower = PowerType.Electricity;
				}
				else if (GetCurrentPower().Equals(PowerType.Gravity))
				{
					if (gameManager.GetHasElectricity()) currentActivePower = PowerType.Electricity;
					else if (gameManager.GetHasLevitation()) currentActivePower = PowerType.Levitation;
				}
				else if (GetCurrentPower().Equals(PowerType.Electricity))
				{
					if (gameManager.GetHasLevitation()) currentActivePower = PowerType.Levitation;
					else if (gameManager.GetHasGravity()) currentActivePower = PowerType.Gravity;
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
    
		if (targetingReticle) 
		{
			Texture currTex = targetingReticle.renderer.material.GetTexture("_MainTex");
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
    }

	void HandleDamage()
	{
		invincibilityFrames++;

		// flash character during invincibility
		if (invincibilityFrames < MAX_INVINCIBILITY_FRAMES)
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
			
			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow)) 
			{
				//rigidbody.AddForce(Vector3.up * 7, ForceMode.VelocityChange);
				rigidbody.velocity = new Vector3(0, 8, 0);
				//animator.SetBool("Walking", false);
				animator.SetTrigger("Jumping");

				isJumping = true;

				// play sound
				int soundChoice = Random.Range(0, 2);
				if (soundChoice == 0)
					jumpVoiceOne.Play();
				else
					jumpVoiceTwo.Play();
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

		bool isGroundedBool = (Physics.Raycast(transformPositionWithOffset, -Vector3.up, distToGround + 0.1f)
		        || (Physics.Raycast(leftBound, -Vector3.up, distToGround + 0.1f))
		        || (Physics.Raycast(rightBound, -Vector3.up, distToGround + 0.1f)));

		if (isGroundedBool) isJumping = false;

		return isGroundedBool;
	}
	
	void OnCollisionEnter(Collision collision)
	{ 
		// if collision with hazardous object, lose life
		if (collision.gameObject.tag.Equals("Hazard") && invincibilityFrames > MAX_INVINCIBILITY_FRAMES)
		{
			TakeDamage(normalKnockback);
		}
		else if (collision.gameObject.tag.Equals("FloorSpikes") && invincibilityFrames > MAX_INVINCIBILITY_FRAMES) 
		{
			TakeDamage(floorSpikesKnockback);
		}
		else if (collision.gameObject.tag.Equals("Lava") && invincibilityFrames > MAX_INVINCIBILITY_FRAMES)
		{
			TakeDamage(lavaKnockback);
			SetOnFire();
		}
		else if (collision.gameObject.tag.Equals("InstantDeath") && invincibilityFrames > MAX_INVINCIBILITY_FRAMES) 
		{
			gameManager.Die();	
		}
		// environment tag needed in case we want to be able to control 
		if (collision.gameObject.tag.Equals("Environment") && !IsGrounded())
		{
			collidingWall = true;
		}

		// platform
		if (collision.gameObject.name.Equals("Platform") || collision.gameObject.name.Equals("Moving Sphere"))
		{
			transform.parent = collision.gameObject.transform; 
		}
	}

	void OnCollisionStay(Collision collisionInfo) 
	{
		if (collisionInfo.collider.gameObject.tag.Equals("Hazard") && invincibilityFrames > MAX_INVINCIBILITY_FRAMES)
		{
			TakeDamage(normalKnockback);
		}
		else if (collisionInfo.collider.gameObject.tag.Equals("FloorSpikes") && invincibilityFrames > MAX_INVINCIBILITY_FRAMES) 
		{
			TakeDamage(floorSpikesKnockback);
		}
		else if (collisionInfo.collider.gameObject.tag.Equals("Lava") && invincibilityFrames > MAX_INVINCIBILITY_FRAMES)
		{
			TakeDamage(lavaKnockback);
			SetOnFire();
		}

		// platform
		if (transform.parent == null 
		    && (collisionInfo.collider.gameObject.name.Equals("Platform") || collisionInfo.collider.gameObject.name.Equals("Moving Sphere")))
		{
			transform.parent = collisionInfo.collider.gameObject.transform; 
		}
	}
	
	void OnCollisionExit()
	{
		collidingWall = false;

		transform.parent = null;
	}

	public void TakeDamage(Vector3 knockback)
	{
		gameManager.LoseLife();

		// SFX
		int damageVoiceChoice = Random.Range(0, 4);
		switch (damageVoiceChoice)
		{
			case 0:
				damageVoiceOne.Play();
				break;
			case 1:
				damageVoiceTwo.Play();
				break;
			case 2:
				damageVoiceThree.Play();
				break;
			default:
				damageVoiceFour.Play();
				break;
		}
		
		// knock back
		Vector3 knockbackForce = knockback;
		if (gameObject.GetComponent<Rigidbody>().velocity.x < 0)
			knockbackForce.x = -knockback.x;
		gameObject.GetComponent<Rigidbody>().AddForce(knockbackForce);
		
		invincibilityFrames = 0;
	}

	public void SetOnFire()
	{
		if (!currentOnFireObject) 
		{
			currentOnFireObject = (GameObject)Instantiate(onFirePrefab, transform.position, Quaternion.identity);
			currentOnFireObject.transform.parent = transform;
			Destroy(currentOnFireObject, 2.0f);
		}
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
	public void SetCameraYPlus(float pos) { camYPlus = pos; }
	public void SetCameraZPosition(float pos) { camZPosition = pos; }
	public void SetReticleZPosition(float pos) { reticleZPos = pos; }
	public void SetCamFollowPlayer(bool b) { camFollowPlayer = b; }
}
