using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {

	bool leverDown = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GameObject wall1 = GameObject.FindWithTag("Wall1");
		GameObject wall2 = GameObject.FindWithTag("Wall2");
		GameObject wall3 = GameObject.FindWithTag("Wall3");
		GameObject lever = GameObject.FindWithTag("Lever");
		if (!leverDown) {
			lever.transform.Rotate (Vector3.forward * 90f);
			leverDown = true;
		}
		if (wall1.transform.position.y > -20) {
			wall1.transform.Translate(-Vector2.up * 12f * Time.deltaTime);
		}
		if (wall3.transform.position.y < 20) {
			wall3.transform.Translate(Vector2.up * 12f * Time.deltaTime);
		}
	}
}
