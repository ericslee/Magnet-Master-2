using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

	const float MAX_HEALTH = 100.0f;
	const float FIREBALL_SPEED = 300;
	const float FIREBALL_RAND = 1.5f;

	public float health;

	// animation
	protected Animator animator;

	// attacks
	Object fireballPrefab;
	GameObject currentFireball;
	Vector3 attackSpawnPoint;
	bool inMidAttack = false;

	// references
	public GameObject player;
	PlayerScript playerScript;

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
	}
	
	void Update() 
	{

	}

	public void StartAttacking()
	{
		// continuously shoot fireballs
		InvokeRepeating("AttackWrapper", 0, 1.0f);
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
		animator.SetTrigger("attack");
		yield return new WaitForSeconds(0.65f);
		currentFireball = (GameObject)Instantiate(fireballPrefab, attackSpawnPoint, Quaternion.identity);
		Vector3 directionToPlayer = player.transform.position - attackSpawnPoint;
			
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
