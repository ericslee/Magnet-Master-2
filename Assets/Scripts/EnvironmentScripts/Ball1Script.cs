using UnityEngine;
using System.Collections;

public class Ball1Script : MonoBehaviour {

	private bool right;

	// Use this for initialization
	void Start () {
		right = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x <= 20) {
			right = true;
		}

		if (transform.position.x >= 31) {
			right = false;
		}

		if (right) {
			transform.Translate(Vector2.right * 2f * Time.deltaTime);
		} else {
			transform.Translate(-Vector2.right * 2f * Time.deltaTime);
		}
	}
}
