using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	const int MAX_LIVES = 5;
	const int START_LEVEL = 1;

	int currentLevel;
	int totalLives;

	// Use this for initialization
	void Start () 
	{
		currentLevel = START_LEVEL;
		totalLives = MAX_LIVES;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
