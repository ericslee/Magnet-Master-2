using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {

	bool leverDown = false;

	// references
	GameObject wall1;
	GameObject wall2;
	GameObject wall3;
	GameObject lever;

	// Use this for initialization
	void Start () {
		// cache references
		wall1 = GameObject.FindWithTag("Wall1");
		wall2 = GameObject.FindWithTag("Wall2");
		wall3 = GameObject.FindWithTag("Wall3");
		lever = GameObject.FindWithTag("Lever");
	}
	
	// Update is called once per frame
	void Update () {
		if (!leverDown) {
			lever.transform.Rotate (Vector3.forward * 90f);
			transform.Translate(-Vector2.right*1.25f);
			transform.Translate (Vector2.up);
			leverDown = true;
		}
		if (wall1.transform.position.y > -20) {
			wall1.transform.Translate(-Vector2.up * 12f * Time.deltaTime);
		}
		if (wall3.transform.position.y < 25) {
			wall3.transform.Translate(Vector2.up * 12f * Time.deltaTime);
		}
	}
}
