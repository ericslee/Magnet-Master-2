using UnityEngine;
using System.Collections;

public class ConveyorScript : MonoBehaviour {

	void Start() 
	{
	
	}
	
	void Update() 
	{
		transform.Rotate(Vector3.back * 10f * Time.deltaTime);
	}
}
