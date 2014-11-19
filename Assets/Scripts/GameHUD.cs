using UnityEngine;
using System.Collections;

public class GameHUD : MonoBehaviour {

	GameManager gameManager;
	PlayerScript playerScript;

	Texture2D healthBar;
	Texture2D healthBarFill;
	Texture2D gainsBar;
	Texture2D gainsBarFill;

	void Start () 
	{
		// cache references
		gameManager = GetComponent<GameManager>();
		playerScript = GameObject.Find("Lucina").GetComponent<PlayerScript>();

		healthBar = Resources.Load("Textures/health-bar") as Texture2D;
		healthBarFill = CreateHealthBarTexture();
		gainsBar = Resources.Load("Textures/gains-bar") as Texture2D;
		gainsBarFill = CreateGainsBarTexture();
	}
	
	void Update () {
	
	}

	private Color GetColorFrom256Scale(int r, int g, int b) {
		return new Color(r/255f, g/255f, b/255f);
	}
	
	private Color GetColorFrom256Scale(int r, int g, int b, float a) {
		return new Color(r/255f, g/255f, b/255f, a);
	}

	private Texture2D CreateHealthBarTexture() {
		int width = healthBar.width;
		int height = healthBar.height;
		
		Texture2D hBarFill = new Texture2D(width, height);
		Color[] colors = hBarFill.GetPixels();
		for (int i = 0; i < colors.Length; i++) {
			colors[i] = GetColorFrom256Scale(0, 190, 134);
		}
		hBarFill.SetPixels(colors);
		hBarFill.Apply();

		return hBarFill;
	}

	private Texture2D CreateGainsBarTexture() {
		int width = gainsBar.width;
		int height = gainsBar.height;
		
		Texture2D gBarFill = new Texture2D(width, height);
		Color[] colors = gBarFill.GetPixels();
		for (int i = 0; i < colors.Length; i++) {
			colors[i] = GetColorFrom256Scale(177, 6, 57, 0.5f);
		}
		gBarFill.SetPixels(colors);
		gBarFill.Apply();
		
		return gBarFill;
	}

	// Right now this is hooked to num lives, will have to change later
	private void DrawHealthBar() {
		float healthRatio = (float)gameManager.GetTotalLives() / gameManager.GetMaxLives();

		// Fraction of the screen width we want the health bar to take up
		float mult = 0.35f;

		// New width and height of health bar on screen w.r.t our Screen size
		int w = (int)(Screen.width * mult);
		int h = (int)(w * healthBar.height / healthBar.width);

		// x,y where the health bar fill starts (top left corner of it). 7 comes from the full size border being 7 pixels.
		// +1 because it usually looks better with that buffer.
		int x = (int)((float)h / healthBar.height * 7) + 1;
		int y = (int)((float)w /healthBar.width * 7) + 1;

		GUI.BeginGroup(new Rect(10, (int)(Screen.height * 0.03f), w, h));
			GUI.DrawTexture(new Rect(0, 0, w, h), healthBar);
			GUI.DrawTexture(new Rect(x, y, (int)((w - x * 2) * healthRatio), h - y * 2), healthBarFill); 
		GUI.EndGroup();
	}

	private void DrawGainsBar() {
		float gainsRatio = 0.5f; //HOOK TO GAIN
		
		// Fraction of the screen width we want the health bar to take up
		float mult = 0.65f;
		
		// New width and height of health bar on screen w.r.t our Screen size
		int h = (int)(Screen.height * mult);
		int w = (int)(h * gainsBar.width / gainsBar.height);
		
		// x,y where the health bar fill starts (top left corner of it). 7 comes from the full size border being 7 pixels.
		// +1 because it usually looks better with that buffer.
		int x = (int)((float)h / gainsBar.height * 7) + 1;
		int y = (int)((float)w /gainsBar.width * 7) + 1;
		
		GUI.BeginGroup(new Rect(10, (int)(Screen.height * 0.2f), w, h));
		GUI.DrawTexture(new Rect(0, 0, w, h), gainsBar);
		GUI.DrawTexture(new Rect(x, y, w - x * 2, (int)((h - y * 2) * gainsRatio)), gainsBarFill); 
		GUI.EndGroup();
	}

	void OnGUI() 
	{
		GUI.Label(new Rect(50, 15, Screen.width / 5, Screen.height / 10), "Lives: " + gameManager.GetTotalLives());

		// Loss state
		if (gameManager.GetHasLost()) 
		{
			GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 2, Screen.height / 2), "LUCINA IS DEAD. GAME OVER");
		}
		if (gameManager.GetHasWon())
		{
			GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 2, Screen.height / 2), "MAGNET MASTER HAS BEEN AVENGED. YOU WIN");
		}

		// Powers
		GUI.Label(new Rect(50, 45, Screen.width / 5, Screen.height / 10), playerScript.GetCurrentPower().ToString());
		GUI.Label(new Rect(50, 75, Screen.width / 5, Screen.height / 10), "GAIN: " + playerScript.GetCurrentPowerGain());

		// Health
		DrawHealthBar();
		DrawGainsBar();
	}
}
