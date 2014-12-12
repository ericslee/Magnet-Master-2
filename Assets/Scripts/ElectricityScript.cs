using UnityEngine;
using System.Collections;

public class ElectricityScript : MonoBehaviour {

	const float INITIAL_ELECTRICITY_GAIN = 0f;
	const float MAX_ELECTRICITY_GAIN = 100f;
	const float MIN_ELECTRICITY_GAIN = 0f;

	GameObject currentlyElectrifiedObject;

	PlayerScript playerScript;

	// adjusting gain
	float electricityGain;
	bool increasingGain = true;
	float mouseClickYPos;
	GameObject lightningEmitter;
	public LightningBolt lightningBoltScript;
	float poweredThreshold;
	float overchargedThreshold;
	GameObject explosionGO;
	GameObject explosionPrefab;

	Vector2 pylonPoweringThresholds = new Vector2(30f, 60f);
	Vector2 conveyorPoweringThresholds = new Vector2(40f, 50f);
	Vector2 wallPoweringThresholds = new Vector2(60f, 80f);

	// Sounds
	AudioSource electricityPowerSound;
	AudioSource turnOnMachinarySound;
	AudioSource correctThresholdSound;
	AudioSource belowThresholdSound;
	AudioSource overchargeSound;
	
	void Start() 
	{
		electricityGain = INITIAL_ELECTRICITY_GAIN;

		// cache references
		lightningEmitter = transform.GetChild(2).gameObject;
		lightningBoltScript = transform.GetChild(2).GetComponent<LightningBolt>();
		playerScript = GetComponent<PlayerScript>();
		explosionPrefab = (GameObject)Resources.Load("Prefabs/Explosion");

		// set up sounds
		electricityPowerSound = GetComponents<AudioSource>()[0];
		turnOnMachinarySound = GetComponents<AudioSource>()[1];
		correctThresholdSound = GetComponents<AudioSource>()[14];
		belowThresholdSound = GetComponents<AudioSource>()[15];
		overchargeSound = GetComponents<AudioSource>()[16];
	}
	
	void Update() 
	{
		if (Input.GetMouseButton(0) && currentlyElectrifiedObject) 
		{
			// adjust gain automatically
			if (increasingGain)
			{
				electricityGain++;
			}
			else
			{
				electricityGain--;
			}
			if (electricityGain >= 100f) increasingGain = false;
			else if (electricityGain <= 0f) increasingGain = true;
		}

		// applying electricity
		if (Input.GetMouseButtonUp(0) && currentlyElectrifiedObject)
		{
			// if in range, activate
			if (electricityGain >= poweredThreshold && electricityGain <= overchargedThreshold)
			{
				// play zelda sound
				ActivateObject();
				correctThresholdSound.Play();
			}
			// if too low, play slow (wrong noise)
			else if (electricityGain < poweredThreshold)
			{
				belowThresholdSound.Play();
			}
			// if too high, explosion and damage player
			else if (electricityGain > overchargedThreshold)
			{
				overchargeSound.Play();
				playerScript.TakeDamage(playerScript.normalKnockback);
				explosionGO = (GameObject)Instantiate(explosionPrefab, currentlyElectrifiedObject.transform.position, Quaternion.identity);
				Destroy(explosionGO, 5.0f);
			}

			currentlyElectrifiedObject = null;
			lightningBoltScript.target = null;
			lightningBoltScript.enabled = false;
			lightningEmitter.GetComponent<ParticleRenderer>().enabled = false;
			electricityPowerSound.Stop();
		}
	}

	void ActivateObject()
	{
		// activate object 
		MonoBehaviour[] objectScripts = currentlyElectrifiedObject.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour script in objectScripts)
		{
			if (script.enabled == false)
			{
				script.enabled = true;
				turnOnMachinarySound.Play();
			}
		}
		
		// disable spark once activated
		foreach (Transform t in currentlyElectrifiedObject.transform)
		{
			if(t.name.Equals("Sparks"))	t.gameObject.SetActive(false);
		}
	}

	public void SetElectricityTarget(GameObject obj, float mouseYPos)
	{
		if (obj) 
		{
			currentlyElectrifiedObject = obj;
			mouseClickYPos = mouseYPos;
		}

		lightningBoltScript.target = obj.transform;
		if (lightningBoltScript.target)
		{
			lightningBoltScript.enabled = true;
			lightningEmitter.GetComponent<ParticleRenderer>().enabled = true;
			electricityPowerSound.Play();

			if (obj.name.Equals("PylonGO"))
			{
				poweredThreshold = pylonPoweringThresholds.x;
				overchargedThreshold = pylonPoweringThresholds.y;
			}
			else if (obj.name.Equals("Conveyor"))
			{
				poweredThreshold = conveyorPoweringThresholds.x;
				overchargedThreshold = conveyorPoweringThresholds.y;
			}
			else if (obj.name.Equals("Lever"))
			{
				poweredThreshold = wallPoweringThresholds.x;
				overchargedThreshold = wallPoweringThresholds.y;
			}
		}
	}

	// Getters
	public float GetGain() { return electricityGain; }
	public float GetMaxGain() { return MAX_ELECTRICITY_GAIN; }
}
