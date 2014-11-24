using UnityEngine;
using System.Collections;

public class WallPathScript : MonoBehaviour {
	bool leverDown = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameObject wall1 = GameObject.FindWithTag("IntroWall");
		GameObject lever = GameObject.FindWithTag("Lever");
		if (!leverDown) {
			lever.transform.Rotate (Vector3.forward * 90f);
			leverDown = true;
		}
		if (wall1.transform.position.y > -2f) {
			wall1.transform.Translate(-Vector2.right * 12f * Time.deltaTime);
		}
	}
}
