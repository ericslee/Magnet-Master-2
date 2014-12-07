using UnityEngine;
using System.Collections;

public class TutorialTextScript : MonoBehaviour {

	Vector3 tutorialPosition;
	float timeIn;

	void Start() 
	{

	}
	
	void Update() 
	{
		
	}

	public void EnterText()
	{
		iTween.MoveTo(gameObject, tutorialPosition,  timeIn);
	}

	public void SetTutorialPosition(Vector3 pos) { tutorialPosition = pos; }
	public void SetTimeIn(float time) { timeIn = time; }
}
