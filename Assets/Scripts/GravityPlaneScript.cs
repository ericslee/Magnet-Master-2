using UnityEngine;
using System.Collections;

public class GravityPlaneScript : MonoBehaviour {

	public GravityScript gravScript;
	public GameObject player;

	void Start() 
	{
		player = GameObject.Find("Player");
		gravScript = player.GetComponent<GravityScript>();
	}
	
	void Update() 
	{
	
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject) 
		{
			gravScript.AddToGravityList(other.gameObject);
		}
	}
}
