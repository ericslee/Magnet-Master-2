using UnityEngine;
using System.Collections;

public class GravityPlaneScript : MonoBehaviour {

	public GravityScript gravScript;
	public GameObject player;

	float rotationRate;

	void Start() 
	{
		player = GameObject.Find("Lucina");
		gravScript = player.GetComponent<GravityScript>();

		rotationRate = 40;
	}
	
	void Update() 
	{
		transform.Rotate(Vector3.up * Time.deltaTime * rotationRate);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject && other.gameObject.layer != 9) 
		{
			gravScript.AddToGravityList(other.gameObject);
		}
	}
}
