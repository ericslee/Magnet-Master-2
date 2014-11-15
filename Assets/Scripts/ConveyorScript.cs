using UnityEngine;
using System.Collections;

public class ConveyorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.back * 4f * Time.deltaTime);
	}
}
