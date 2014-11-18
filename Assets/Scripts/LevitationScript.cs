using UnityEngine;
using System.Collections;

public class LevitationScript : MonoBehaviour {

	const float INITIAL_LEVITATION_GAIN = 1.0f;
	const float LEVITATION_RAW_AMOUNT = 3.5f;
	const float MAX_LEVITATION_GAIN = 2.0f;
	const float MIN_LEVITATION_GAIN = 0.0f;

	GameObject currentlyLevitatingObj;
	Vector3 objInitialPosition;

	// adjusting gain
	float levitationGain;
	float mouseClickYPos;

	void Start() 
	{
		levitationGain = INITIAL_LEVITATION_GAIN;
	}
	
	void Update() 
	{
		if (Input.GetMouseButton(0) && currentlyLevitatingObj) 
		{
			Vector3 levitatedPosition = currentlyLevitatingObj.transform.position;
			levitatedPosition.y = objInitialPosition.y + 
				(levitationGain * LEVITATION_RAW_AMOUNT);
			currentlyLevitatingObj.transform.position = levitatedPosition;

			// adjust gain if necessary
			if (Input.mousePosition.y != mouseClickYPos) 
			{
				float newLevitationGain = (float)(Input.mousePosition.y - mouseClickYPos) / 100.0f;
				if (newLevitationGain > MAX_LEVITATION_GAIN) newLevitationGain = MAX_LEVITATION_GAIN;
				else if (newLevitationGain < MIN_LEVITATION_GAIN) 
					newLevitationGain = MIN_LEVITATION_GAIN;

				levitationGain = newLevitationGain;
			}
		}
	}

	public void SetCurrentlyLevitatingObj(GameObject obj, float mouseYPos)
	{
		if (obj) 
		{
			currentlyLevitatingObj = obj;
			objInitialPosition = obj.transform.position;
			mouseClickYPos = mouseYPos;
		}
		Debug.Log(currentlyLevitatingObj.name);
	}

	// Getters
	public float GetGain()
	{
		return levitationGain;
	}
}
