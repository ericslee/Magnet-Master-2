using UnityEngine;
using System.Collections;

public class ConveyorScript : MonoBehaviour {
	Vector3 rotation;

	void Start() 
	{
		rotation = new Vector3(-1.0f,0,0);
	}
	
	void Update() 
	{
		transform.Rotate(rotation * 10f * Time.deltaTime);
	}
}
