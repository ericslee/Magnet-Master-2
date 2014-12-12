using UnityEngine;
using System.Collections;

public class RestartButton : MonoBehaviour {

	private GameObject restartButton;
	private Camera guiCamera;

	void Start() {
		restartButton = GameObject.FindGameObjectWithTag("RestartButton");
		guiCamera = GameObject.Find("GUI Camera").camera;
	}

	void Update() {
		if (Time.time % 1 > 0 && Time.time % 1 < 0.5f) {
			restartButton.renderer.enabled = false;
		} else {
			restartButton.renderer.enabled = true;
		}

		Ray ray = guiCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray) && Input.GetMouseButtonDown(0))
			Application.LoadLevel("IntroScene");
	}
}
