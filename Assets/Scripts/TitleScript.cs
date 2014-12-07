using UnityEngine;
using System.Collections;

public class TitleScript : MonoBehaviour {

	private GameObject startButton;
	private Camera guiCamera;

	void Start() {
		startButton = GameObject.Find("Start Button");
		guiCamera = GameObject.Find("GUI Camera").camera;
	}

	void Update() {
		if (Time.time % 1 > 0 && Time.time % 1 < 0.5f) {
			startButton.renderer.enabled = false;
		} else {
			startButton.renderer.enabled = true;
		}

		Ray ray = guiCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray) && Input.GetMouseButtonDown(0))
			Application.LoadLevel("IntroScene");
	}
}
