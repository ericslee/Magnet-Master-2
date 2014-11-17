using UnityEngine;
using System.Collections;

public class Ball2Script : MonoBehaviour {

	private bool up;

	// Use this for initialization
	void Start () {
		up = true;
	}

	// Update is called once per frame
	void Update () {

		if (transform.position.y <= -2.4) {
			up = true;
		}

		if (transform.position.y >= 2.8) {
			up = false;
		}
		
		if (up) {
			transform.Translate(Vector2.up* 4f * Time.deltaTime);
		} else {
			transform.Translate(-Vector2.up * 4f * Time.deltaTime);
		}
	}
}
