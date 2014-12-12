using UnityEngine;
using System.Collections;

public class LucinaAnimations : MonoBehaviour {
	
	static int idleState = Animator.StringToHash("Base Layer.idle");
	static int walkState = Animator.StringToHash("Base Layer.walk");
	static int jumpState = Animator.StringToHash("Base Layer.jump");
	static int pantsuState = Animator.StringToHash("Base Layer.pantsu");
	static int winState = Animator.StringToHash("Base Layer.win");

//	private AnimatorStateInfo currentBaseState;
	protected Animator animator;

	// Use this for initialization
	void Start() {
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		if (animator) { 
			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				animator.SetBool("Walking", true);
			} else if (Input.GetKeyUp(KeyCode.RightArrow)) {
				animator.SetBool("Walking", false);
			}
			if (Input.GetKeyUp(KeyCode.UpArrow)) {
				animator.SetTrigger("Jumping");
			} else if (Input.GetKeyUp(KeyCode.UpArrow)) {
			}

			if (animator.GetCurrentAnimatorStateInfo(0).nameHash == jumpState) {

			}

//			if (Input.GetKeyUp(KeyCode.UpArrow)) {
//				animator.SetTrigger("EndJumping");
//			}
//			if (Input.GetKeyDown(KeyCode.RightArrow)) {
//				animator.SetBool("Running", true);
//			}
//			if (Input.GetKeyUp(KeyCode.RightArrow)) {
//				animator.SetBool("Running", false);
//			}
		}
	}
}
