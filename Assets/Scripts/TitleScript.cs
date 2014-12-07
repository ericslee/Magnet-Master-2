using UnityEngine;
using System.Collections;

public class TitleScript : MonoBehaviour {
	
	private GUIStyle buttonStyle; 
	private Texture newGameButton; 
	// Use this for initialization 
	void Start () {
		newGameButton = (Texture)Resources.Load("Resources/Materials/Textures/ngb");
	} 
	
	// Update is called once per frame 
	void Update () { 
		if (Input.GetKey (KeyCode.Escape)) {
			Application.Quit();
			Debug.Log ("Application.Quit() only works in build, not in editor"); 
		}
	} 

	void OnGUI (){ 
		GUILayout.BeginArea(new Rect(Screen.width/2-300, Screen.height/2-300, 400,400));
		                             //175, 425)); 
		//GUILayout.TextField("Magnet Master 2");
		// Load the main scene 
		// The scene needs to be added into build setting to be loaded! 
		/*if (GUI.Button(new Rect(Screen.width/2, Screen.height/2, 200, 200), newGameButton)) {
			Application.LoadLevel("Mine"); 
		}*/

		if (GUILayout.Button("\n\nNew Game\n\n")) 
		{ 
			Application.LoadLevel("IntroScene"); 
		}
		
		if (GUILayout.Button("\n\nExit\n\n")) 
		{ 
			Application.Quit(); 
			Debug.Log ("Application.Quit() only works in build, not in editor"); 
		} 
		
		GUILayout.EndArea(); 
	} 
}
