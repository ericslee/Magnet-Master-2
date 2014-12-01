using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravityScript : MonoBehaviour {

	const float INITIAL_GRAVITY_GAIN = 1.0f;
	const float GRAVITY_RADIUS_RAW = 0.1f;
	const float MAX_GRAVITY_GAIN = 1.5f;
	const float MIN_GRAVITY_GAIN = 0.0f;

	GameObject currentGravityCenter;
	GameObject currentGravityPlane;
	Object gravityPlanePrefab;
	HashSet<GameObject> gravityTargets = new HashSet<GameObject>(); // set of objects to be affected by gravity

	// adjusting gain
	float gravityGain;
	float mouseClickYPos;

	// Sounds
	AudioSource initialGravityPowerSound;

	void Start() 
	{
		gravityGain = INITIAL_GRAVITY_GAIN;
		gravityPlanePrefab = Resources.Load("Prefabs/GravityPlane");

		// set up sounds
		initialGravityPowerSound = GetComponents<AudioSource>()[3];
	}
	
	void Update () 
	{
		if (Input.GetMouseButton(0) && currentGravityPlane) 
		{
			// adjust gain if necessary
			if (Input.mousePosition.y != mouseClickYPos) 
			{
				float newGravityGain = (float)(Input.mousePosition.y - mouseClickYPos) / 500.0f;
				if (newGravityGain > MAX_GRAVITY_GAIN) newGravityGain = MAX_GRAVITY_GAIN;
				else if (newGravityGain < MIN_GRAVITY_GAIN) 
					newGravityGain = MIN_GRAVITY_GAIN;
				
				gravityGain = newGravityGain;
				
				// adjust scale of gravity plane 
				Vector3 planeScale = currentGravityCenter.transform.localScale * (GRAVITY_RADIUS_RAW * gravityGain);
				currentGravityPlane.transform.localScale = planeScale;

				// remove platforms from the gravity center's pull if necessary
			}
		}

		// destroy gravity plane
		if (Input.GetMouseButtonUp(0))
		{
			Destroy(currentGravityPlane);
			currentGravityPlane = null;
			// unparent everything from gravity targets list
			foreach (GameObject obj in gravityTargets)
			{
				obj.transform.parent = null;
			}

			gravityTargets.Clear();
		}
	}

	public void SetGravityCenter(GameObject obj, float mouseYPos)
	{
		// remove existing gravity plane
		if (currentGravityPlane) 
		{
			Destroy(currentGravityPlane);
			currentGravityPlane = null;
			gravityTargets.Clear();
		}
		if (obj)
		{
			currentGravityPlane = (GameObject)Instantiate(gravityPlanePrefab, obj.transform.position, Quaternion.Euler(-90, 0, 0));
			currentGravityPlane.transform.parent = obj.transform;
			Vector3 planeScale = obj.transform.localScale * GRAVITY_RADIUS_RAW;
			currentGravityPlane.transform.localScale = planeScale;
			currentGravityCenter = obj;

			initialGravityPowerSound.Play();
		}
	}

	public void AddToGravityList(GameObject obj)
	{
		if (!gravityTargets.Contains(obj)) 
		{
			gravityTargets.Add(obj);

			// move object over to gravity center
			if (currentGravityCenter)
			{
				float timeToGravityCenter = Vector3.Distance(currentGravityCenter.transform.position, obj.transform.position) / 10.0f;

				// attract to edge of object

				Vector3 attractionPos = currentGravityCenter.transform.position - 
					(GetColliderRadius() * (currentGravityCenter.transform.position - obj.transform.position));

				//Vector3 attractionPos = currentGravityCenter.transform.position;

				iTween.MoveTo(obj, attractionPos, timeToGravityCenter);

				// parent to gravity center
				obj.transform.parent = currentGravityCenter.transform;
			}
		}
	}

	float GetColliderRadius()
	{
		if (currentGravityCenter)
		{
			if (currentGravityCenter.GetComponent<SphereCollider>())
			{
				return currentGravityCenter.GetComponent<SphereCollider>().radius;
			}
			else if (currentGravityCenter.GetComponent<BoxCollider>())
			{
				return currentGravityCenter.GetComponent<BoxCollider>().size.x / 2.0f;
			}
			else if (currentGravityCenter.GetComponent<CapsuleCollider>())
			{
				return currentGravityCenter.GetComponent<CapsuleCollider>().radius;
			}
		}

		return 0;
	}

	// Getters
	public float GetGain() { return gravityGain; }
	public float GetMaxGain() { return MAX_GRAVITY_GAIN; }
}
