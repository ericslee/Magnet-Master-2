using UnityEngine;
using System.Collections;

public class ElectricityScript : MonoBehaviour {

	const float INITIAL_ELECTRICITY_GAIN = 2.5f;
	const float MAX_ELECTRICITY_GAIN = 6.0f;
	const float MIN_ELECTRICITY_GAIN = 1.8f;

	GameObject currentlyElectrifiedObject;

	// adjusting gain
	float electricityGain;
	float mouseClickYPos;
	GameObject lightningEmitter;
	LightningBolt lightningBoltScript;

	void Start() 
	{
		electricityGain = INITIAL_ELECTRICITY_GAIN;

		// cache references
		lightningEmitter = transform.GetChild(0).gameObject;
		lightningBoltScript = transform.GetChild(0).GetComponent<LightningBolt>();
	}
	
	void Update() 
	{
		if (Input.GetMouseButton(0) && currentlyElectrifiedObject) 
		{
			// adjust gain if necessary
			if (Input.mousePosition.y != mouseClickYPos) 
			{
				float newElectricityGain = (float)(Input.mousePosition.y - mouseClickYPos) / 50.0f;
				if (newElectricityGain > MAX_ELECTRICITY_GAIN) newElectricityGain = MAX_ELECTRICITY_GAIN;
				else if (newElectricityGain < MIN_ELECTRICITY_GAIN) 
					newElectricityGain = MIN_ELECTRICITY_GAIN;
				
				electricityGain = newElectricityGain;

				// adjust scale of electricity (visuals only)
				lightningBoltScript.scale = electricityGain;
			}
		}

		// stop applying electricity
		if (Input.GetMouseButtonUp(0))
		{
			currentlyElectrifiedObject = null;
			lightningBoltScript.target = null;
			lightningBoltScript.enabled = false;
			lightningEmitter.GetComponent<ParticleRenderer>().enabled = false;
		}
	}

	public void SetElectricityTarget(GameObject obj, float mouseYPos)
	{
		if (obj) 
		{
			currentlyElectrifiedObject = obj;
			mouseClickYPos = mouseYPos;
		}
		Debug.Log(currentlyElectrifiedObject.name);

		lightningBoltScript.target = obj.transform;
		if (lightningBoltScript.target)
		{
			lightningBoltScript.enabled = true;
			lightningEmitter.GetComponent<ParticleRenderer>().enabled = true;

			// activate object 
			//TODO: (only if gain is at certain threshold)
			MonoBehaviour[] objectScripts = obj.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour script in objectScripts)
			{
				script.enabled = true;
			}
		}
	}

	// Getters
	public float GetGain()
	{
		return electricityGain;
	}
}
