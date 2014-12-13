using UnityEngine;
using System.Collections;

public class MovingEnemyScript : MonoBehaviour {

	const float MAX_HEALTH = 100.0f;
	const float FIREBALL_SPEED = 600;
	const float FIREBALL_RAND = 1.5f;

	public float health;

	// animation
	protected Animator animator;

	// attacks
	Object fireballPrefab;
	GameObject currentFireball;
	Vector3 attackSpawnPoint;
	bool inMidAttack = false;

	bool isChasing = false;
	bool isGoingLeft = true;
	float gravity = 1.0F;
	Vector3 moveDirection = Vector3.zero;

	// Sounds
	AudioSource initialRoarSFX;
	AudioSource attackRoarSFX;
	AudioSource fireballSFX;

	// references
	public GameObject player;
	PlayerScript playerScript;
	CharacterController controller;

	void Start() 
	{
		health = MAX_HEALTH;
		attackSpawnPoint = transform.position;
		attackSpawnPoint.x -= 1;
		attackSpawnPoint.y += 0.5f;

		// cache references
		animator = GetComponent<Animator>();
		fireballPrefab = Resources.Load("Prefabs/Flame");
		player = GameObject.Find("Lucina");
		playerScript = player.GetComponent<PlayerScript>();
		initialRoarSFX = GetComponents<AudioSource>()[0];
		attackRoarSFX = GetComponents<AudioSource>()[1];
		fireballSFX = GetComponents<AudioSource>()[2];
		controller = GetComponent<CharacterController>();

		animator.SetBool("run", true);
	}
	
	void Update() 
	{
		// move
		if (isChasing)
		{
			if (transform.position.x >= player.transform.position.x && !isGoingLeft)
			{
				isGoingLeft = true;
				transform.Rotate(new Vector3(0, 180, 0));
			}
			else if (transform.position.x < player.transform.position.x && isGoingLeft)
			{
				isGoingLeft = false;
				transform.Rotate(new Vector3(0, 180, 0));
			}

			if (isGoingLeft)
			{
				moveDirection = Vector3.left * 5.0f;
			}
			else 
			{
				moveDirection = Vector3.right * 5.0f;
			}

			controller.Move(moveDirection * Time.deltaTime);
		}
	}

	public void StartAttacking()
	{
		isChasing = true;

		// continuously shoot fireballs
		InvokeRepeating("AttackWrapper", 0, 1.0f);

		initialRoarSFX.Play();
	}

	void AttackWrapper()
	{
		if (!inMidAttack)
		{
			StartCoroutine(Attack());
		}
	}

	IEnumerator Attack()
	{
		inMidAttack = true;
		//animator.SetTrigger("attack");
		attackRoarSFX.Play();
		yield return new WaitForSeconds(0.65f);
		fireballSFX.Play();
		currentFireball = (GameObject)Instantiate(fireballPrefab, transform.position, Quaternion.identity);
		Vector3 directionToPlayer = player.transform.position - transform.position;
			
		// add rand element in y
		directionToPlayer.y = directionToPlayer.y + Random.Range(-FIREBALL_RAND, FIREBALL_RAND);
			
		directionToPlayer.Normalize();
		currentFireball.rigidbody.AddForce(directionToPlayer * FIREBALL_SPEED);
		Destroy(currentFireball, 2);

		yield return new WaitForSeconds(2.0f);
		inMidAttack = false;

		yield return null;
	}
}
