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

	// animation
	bool canControlLevitation;

	// sound
	AudioSource initialLevitationSound;

	void Start() 
	{
		levitationGain = INITIAL_LEVITATION_GAIN;
		canControlLevitation = false;

		// set up sounds
		initialLevitationSound = GetComponents<AudioSource>()[2];
	}
	
	void Update() 
	{
		if (Input.GetMouseButton(0) && currentlyLevitatingObj && canControlLevitation) 
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
		
		// drop object
		if (Input.GetMouseButtonUp(0))
		{
			// reactivate rigidbody physics
			if (currentlyLevitatingObj) {
				Rigidbody objRB = currentlyLevitatingObj.rigidbody;
				if (objRB)
				{
					objRB.isKinematic = false;
				}
				
				currentlyLevitatingObj = null;
				levitationGain = INITIAL_LEVITATION_GAIN;
				canControlLevitation = false;
			}
		}
	}

	public IEnumerator SetCurrentlyLevitatingObj(GameObject obj, float mouseYPos)
	{
		if (obj) 
		{
			currentlyLevitatingObj = obj;
			objInitialPosition = obj.transform.position;
			mouseClickYPos = mouseYPos;

			// set to isKinematic
			Rigidbody objRB = obj.rigidbody;
			if (objRB)
			{
				objRB.isKinematic = true;
			}

			// initial levitation
			Vector3 levitatedPosition = currentlyLevitatingObj.transform.position;
			levitatedPosition.y = objInitialPosition.y + 
				(levitationGain * LEVITATION_RAW_AMOUNT);
			iTween.MoveTo(currentlyLevitatingObj, levitatedPosition, 0.5f);

			if (initialLevitationSound) initialLevitationSound.Play();

			yield return new WaitForSeconds(0.5f);
			canControlLevitation = true;
			//currentlyLevitatingObj.transform.position = levitatedPosition;
		}

		yield return null;
	}

	// Getters
	public float GetGain() { return levitationGain; }
	public float GetMaxGain() { return MAX_LEVITATION_GAIN; }
}
