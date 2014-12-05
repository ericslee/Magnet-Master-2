using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	// Use this for initialization
	void Start() 
	{
	
	}
	
	// Update is called once per frame
	void Update() 
	{
	
	}

	void OnGUI()
	{ 
		GUILayout.BeginArea(new Rect(Screen.width / 2 - (175 / 2), (Screen.height / 4) * 3, 
		                             175, 425)); 
		// Load the main scene 
		if (GUILayout.Button("New Game")) 
		{ 
			Application.LoadLevel("FinalScene"); 
		}
		
		if (GUILayout.Button("Quit")) 
		{ 
			Application.Quit(); 
			Debug.Log ("Application.Quit() only works in build, not in editor"); 
		} 
		
		GUILayout.EndArea(); 
	} 
}
