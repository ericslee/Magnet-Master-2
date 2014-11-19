using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravityScript : MonoBehaviour {

	const float INITIAL_GRAVITY_GAIN = 1.0f;
	const float GRAVITY_RADIUS_RAW = 3.5f;
	const float MAX_GRAVITY_GAIN = 2.0f;
	const float MIN_GRAVITY_GAIN = 0.0f;

	GameObject currentGravityCenter;
	GameObject currentGravityPlane;
	Object gravityPlanePrefab;
	HashSet<GameObject> gravityTargets = new HashSet<GameObject>(); // set of objects to be affected by gravity

	// adjusting gain
	float gravityGain;
	float mouseClickYPos;

	void Start() 
	{
		gravityGain = INITIAL_GRAVITY_GAIN;
		gravityPlanePrefab = Resources.Load("Prefabs/GravityPlane");
	}
	
	void Update () 
	{
	
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
			Vector3 planeScale = obj.transform.localScale * 0.1f;
			currentGravityPlane.transform.localScale = planeScale;
			currentGravityCenter = obj;
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
				float timeToGravityCenter = Vector3.Distance(currentGravityCenter.transform.position, obj.transform.position) / 5.0f;
				iTween.MoveTo(obj, currentGravityCenter.transform.position, timeToGravityCenter);

				// parent to gravity center
				obj.transform.parent = currentGravityCenter.transform;
			}
		}
	}

	// Getters
	public float GetGain()
	{
		return gravityGain;
	}
}
