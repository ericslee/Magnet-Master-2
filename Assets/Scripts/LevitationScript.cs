using UnityEngine;
using System.Collections;

public class LevitationScript : MonoBehaviour {

	const float INITIAL_LEVITATION_GAIN = 1.0f;
	const float LEVITATION_RAW_AMOUNT = 3.5f;

	GameObject currentlyLevitatingObj;
	Vector3 objInitialPosition;

	// adjusting gain
	float levitationGain;
	float mouseClickYPos;

	// Use this for initialization
	void Start () 
	{
		levitationGain = INITIAL_LEVITATION_GAIN;
	}
	
	// Update is called once per frame
	void Update () 
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
				if (newLevitationGain > 2.0f) newLevitationGain = 2.0f;
				else if (newLevitationGain < 0.0f) newLevitationGain = 0.0f;

				levitationGain = newLevitationGain;
				Debug.Log (levitationGain);
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
}
