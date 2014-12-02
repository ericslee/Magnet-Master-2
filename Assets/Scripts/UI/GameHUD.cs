using UnityEngine;
using System.Collections;

public class GameHUD : MonoBehaviour {

	GameManager gameManager;
	PlayerScript playerScript;

	Texture2D healthBar;
	Texture2D healthBarFill;
	Texture2D gainsBar;
	Texture2D gainsBarFill;
	Texture2D gainsBarFillGravity;
	Texture2D gainsBarFillElectricity;
	Texture2D gainsBarFillLevitation;
	Texture2D powerIconGravity;
	Texture2D powerIconElectricity;
	Texture2D powerIconLevitation;

	void Start () 
	{
		// cache references
		gameManager = GetComponent<GameManager>();
		playerScript = GameObject.Find("Lucina").GetComponent<PlayerScript>();

		healthBar = Resources.Load("Materials/Textures/health-bar-with-highlights") as Texture2D;
		healthBarFill = CreateHealthBarTexture();
		gainsBar = Resources.Load("Materials/Textures/gains-bar-with-highlights") as Texture2D;
		gainsBarFillGravity = CreateGainsBarTexture(PowerType.Gravity);
		gainsBarFillElectricity = CreateGainsBarTexture(PowerType.Electricity);
		gainsBarFillLevitation = CreateGainsBarTexture(PowerType.Levitation);
		gainsBarFill = gainsBarFillLevitation;
		powerIconGravity = Resources.Load("Materials/Textures/power-icon-gravity") as Texture2D;
		powerIconElectricity = Resources.Load("Materials/Textures/power-icon-electricity") as Texture2D;
		powerIconLevitation = Resources.Load("Materials/Textures/power-icon-levitation") as Texture2D;
	}
	
	void Update () {

	}

	private Color GetColorFrom256Scale(int r, int g, int b) {
		return new Color(r/255f, g/255f, b/255f);
	}
	
	private Color GetColorFrom256Scale(int r, int g, int b, float a) {
		return new Color(r/255f, g/255f, b/255f, a);
	}

	private Color GetColorFrom256Scale(Vector3 rgb, float a) {
		return new Color((int)rgb.x/255f, (int)rgb.y/255f, (int)rgb.z/255f, a);
	}

	private Texture2D CreateHealthBarTexture() {
		int width = healthBar.width;
		int height = healthBar.height;
		
		Texture2D hBarFill = new Texture2D(width, height);
		Color[] colors = hBarFill.GetPixels();
		for (int i = 0; i < hBarFill.height; i++) {
			for (int j = 0; j < hBarFill.width; j++) {
				if (hBarFill.height-1-12 <= i && i <= hBarFill.height-1-10) {
					colors[hBarFill.width * i + j] = GetColorFrom256Scale(93, 243, 199);
				} else if (hBarFill.height-1-33 <= i && i <= hBarFill.height-1-5) {
					colors[hBarFill.width * i + j] = GetColorFrom256Scale(24, 182, 146);
				} else {
					colors[hBarFill.width * i + j] = GetColorFrom256Scale(12, 156, 114);
				}
			}
		}
		hBarFill.SetPixels(colors);
		hBarFill.Apply();

		return hBarFill;
	}

	private Texture2D CreateGainsBarTexture(PowerType power) {
		int width = gainsBar.width;
		int height = gainsBar.height;

		Texture2D gBarFill = new Texture2D(width, height);
		Color[] colors = gBarFill.GetPixels();
		float alpha = 0.65f;

		// Levitation
		Vector3 lHigh = new Vector3(253, 185, 205);
		Vector3 lMid = new Vector3(210, 44, 94);
		Vector3 lLow = new Vector3(177, 6, 57);

		// Gravity
		Vector3 gHigh = new Vector3(178, 167, 222);
		Vector3 gMid = new Vector3(112, 94, 184);
		Vector3 gLow = new Vector3(80, 61, 154);

		// Electricity
		Vector3 eHigh = new Vector3(165, 218, 218);
		Vector3 eMid = new Vector3(82, 172, 172);
		Vector3 eLow = new Vector3(45, 137, 138);

		Vector3 high;
		Vector3 mid;
		Vector3 low;

		switch (power) {
			case PowerType.Levitation:
				high = lHigh;
				mid = lMid;
				low = lLow;
				break;
			case PowerType.Gravity:
				high = gHigh;
				mid = gMid;
				low = gLow;
				break;
			default:
				high = eHigh;
				mid = eMid;
				low = eLow;
				break;
		}


		for (int i = 0; i < gBarFill.height; i++) {
			for (int j = 0; j < gBarFill.width; j++) {
				if (10 <= j && j <= 12) {
					colors[gBarFill.width * i + j] = GetColorFrom256Scale(high, alpha);
				} else if (5 <= j && j <= 33) {
					colors[gBarFill.width * i + j] = GetColorFrom256Scale(mid, alpha);
				} else {
					colors[gBarFill.width * i + j] = GetColorFrom256Scale(low, alpha);
				}
			}
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

		// Bar border thickness in vert and horiz directions. Need to calculate due to resizing from original size.
		// 7 comes from the full size border being 7 pixels. +1 because it usually looks better with that buffer.
		int bw = (int)((float)h / healthBar.height * 7) + 1;
		int bh = (int)((float)w /healthBar.width * 7) + 1;

		GUI.BeginGroup(new Rect(10, (int)(Screen.height * 0.03f), w, h));
			GUI.DrawTexture(new Rect(0, 0, w, h), healthBar);
			if (healthRatio > 0) GUI.DrawTexture(new Rect(bw, bh, (int)((w - bw * 2) * healthRatio), h - bh * 2), healthBarFill);
		GUI.EndGroup();
	}

	private void DrawGainsBar() {
		float gainsRatio = (float)playerScript.GetCurrentPowerGain() / playerScript.GetCurrentPowerMaxGain();
		
		// Fraction of the screen width we want the gains bar to take up
		float mult = 0.65f;
		
		// New width and height of gains bar on screen w.r.t our Screen size
		int h = (int)(Screen.height * mult);
		int w = (int)(h * gainsBar.width / gainsBar.height);
		
		// Bar border thickness in vert and horiz directions. Need to calculate due to resizing from original size.
		// 7 comes from the full size border being 7 pixels. +1 because it usually looks better with that buffer.
		int bw = (int)((float)h / gainsBar.height * 7) + 1;
		int bh = (int)((float)w /gainsBar.width * 7) + 1;

		switch (playerScript.GetCurrentPower()) {
			case PowerType.Levitation:
				gainsBarFill = gainsBarFillLevitation;
				break;
			case PowerType.Gravity:
				gainsBarFill = gainsBarFillGravity;
				break;
			default:
				gainsBarFill = gainsBarFillElectricity;
				break;
		}
		int fillHeight = (int)((h - bh * 2) * gainsRatio);
		GUI.BeginGroup(new Rect(10, (int)(Screen.height * 0.2f), w, h));
			GUI.DrawTexture(new Rect(0, 0, w, h), gainsBar);
			GUI.DrawTexture(new Rect(bw, h - bh - fillHeight, w - bw * 2, fillHeight), gainsBarFill); 
		GUI.EndGroup();
	}

	// Draws icon with opacity
	private void DrawIconWithOpacity(int x, int y, int width, int height, Texture2D texture, float alpha) {
		Color orig = GUI.color;
		GUI.color = new Color(orig.r, orig.g, orig.b, alpha);
		GUI.DrawTexture(new Rect(x, y, width, height), texture);
		GUI.color = orig;
	}

	private void DrawPowerIcons() {
		// We want these to be about this scale w.r.t to screen height
		float scale = 0.081f;
		int dim = (int) (scale * Screen.height);
		int padding = 20;
		int totalWidth = dim * 3 + padding * 2;

		float faded = 0.2f;
		GUI.BeginGroup(new Rect(Screen.width - totalWidth - 10, 10, totalWidth, dim));
			DrawIconWithOpacity(0, 0, dim, dim, powerIconLevitation, (playerScript.GetCurrentPower() == PowerType.Levitation) ? 1 : faded);
			DrawIconWithOpacity(dim + padding, 0, dim, dim, powerIconGravity, (playerScript.GetCurrentPower() == PowerType.Gravity) ? 1 : faded);
			DrawIconWithOpacity(dim * 2 + padding * 2, 0, dim, dim, powerIconElectricity, (playerScript.GetCurrentPower() == PowerType.Electricity) ? 1 : faded); 
		GUI.EndGroup();
	}

	void OnGUI() 
	{
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
//		GUI.Label(new Rect(50, 75, Screen.width / 5, Screen.height / 10), "GAIN: " + playerScript.GetCurrentPowerGain());
		DrawPowerIcons();

		// Health
		DrawHealthBar();
		DrawGainsBar();
	}
}
