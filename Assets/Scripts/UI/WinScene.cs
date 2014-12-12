using UnityEngine;
using System.Collections;

public class WinScene : MonoBehaviour {

	static int idleState = Animator.StringToHash("Base Layer.idle");
	static int walkState = Animator.StringToHash("Base Layer.walk");
	static int jumpState = Animator.StringToHash("Base Layer.jump");
	static int pantsuState = Animator.StringToHash("Base Layer.pantsu");
	static int winState = Animator.StringToHash("Base Layer.win");
	
	//	private AnimatorStateInfo currentBaseState;
	protected Animator animator;
	GameObject lucina;
	GameObject lucinaMesh;
	GameObject newGameButton;
	GameObject endCredit;
	bool lucinaIsDisabled;

	const float threshold = 9.25568f;
	float creditsYTranslate = 0.02f;

	//0.9568

	void Start() {
		lucina = GameObject.Find("Yehua");
		lucinaMesh = lucina.transform.GetChild(1).gameObject;
		newGameButton = GameObject.FindWithTag("RestartButton");

		animator = lucina.GetComponent<Animator>();

		endCredit = GameObject.Find("End Credit");
		lucinaIsDisabled = true;
		lucinaMesh.renderer.enabled = false;
		newGameButton.SetActive(false);
	}

	private void ShowNewGameButton() {
		newGameButton.SetActive(true);
	}

	void Update() {
		if (Input.GetKey(KeyCode.Alpha1))
		{
			creditsYTranslate = 1.0f;
		}
		if (Input.GetKeyUp(KeyCode.Alpha1))
		{
			creditsYTranslate = 0.02f;
		}

		Vector3 oldPos = endCredit.transform.position;

		if (oldPos.y < threshold) endCredit.transform.Translate(0, creditsYTranslate, 0);
		if (oldPos.y > threshold && lucinaIsDisabled) {
			lucinaIsDisabled = false;
			lucinaMesh.renderer.enabled = true;
			if (animator) {
				animator.SetTrigger("Win");
			}
			Invoke("ShowNewGameButton", 2.5f);
		}
	}
}
