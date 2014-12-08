using UnityEngine;
using System.Collections;

public class Level2Script : MonoBehaviour {

	GameManager gameManager;
	GameObject player;
	PlayerScript playerScript;

	TutorialTextScript electricityText;
	Vector3 electricityTextPosition;

	// Sound
	AudioSource powerCollectSFX;
	AudioSource doorRattleSFX;

	// Camera
	Camera mainCamera;
	bool zoomOut = false;
	bool zoomIn = true; 
	Vector3 zoomOutPos = new Vector3(94.4f, 18, -52.2f);

	bool doorOpen = false;

	void Start() 
	{
		// cache references
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		player = GameObject.Find("Lucina");
		playerScript = player.GetComponent<PlayerScript>();
		mainCamera = Camera.main;

		electricityText = GameObject.Find("ElectricityText").GetComponent<TutorialTextScript>();
		electricityTextPosition = new Vector3(81.29794f, 9.026501f, 1f);

		powerCollectSFX = GetComponents<AudioSource>()[13];
		doorRattleSFX = GetComponents<AudioSource>()[12];
	}
	
	void Update() 
	{
		if (transform.position.x > 90 && !zoomOut)
		{
			// cam movement
			zoomOut = true;
			zoomIn = false;
			ZoomCameraOut();
		}
		else if (transform.position.x < 88 && !zoomIn)
		{	
			zoomIn = true;
			zoomOut = false;
			StartCoroutine(ZoomCameraIn());
		}
	}

	void ZoomCameraOut()
	{
		playerScript.SetCamFollowPlayer(false);
		iTween.MoveTo(mainCamera.gameObject, zoomOutPos, 3.0f);
	}

	IEnumerator ZoomCameraIn()
	{
		Vector3 playerPos = playerScript.GetPlayerCamPosition();
		playerPos.x = playerPos.x - 1.0f;
		iTween.MoveTo(mainCamera.gameObject, playerPos, 1.0f);
		yield return new WaitForSeconds(1.0f);
		playerScript.SetCamFollowPlayer(true);

		yield return null;
	}

	void OnTriggerEnter(Collider c) 
	{
		if (c.tag.Equals("Level2Door"))
		{
			if (doorOpen)
			{
				gameManager.StartLevel(3);
			}
			else 
			{
				if (doorRattleSFX) doorRattleSFX.Play();
			}
		}
		else if (c.gameObject.name.Equals("ElectricityPower"))
		{
			gameManager.SetHasElectricity(true);
			Destroy(c.gameObject);
			if (powerCollectSFX) powerCollectSFX.Play();
			
			electricityText.SetTutorialPosition(electricityTextPosition);
			electricityText.SetTimeIn(1.0f);
			electricityText.EnterText();
		}
	}

	public void OpenDoor()
	{
		doorOpen = true;
	}
}
