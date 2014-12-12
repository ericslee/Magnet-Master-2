using UnityEngine;
using System.Collections;

public class GameHUD : MonoBehaviour {

	GameManager gameManager;
	PlayerScript playerScript;
	ElectricityScript elecScript;

	Texture2D healthBar;
	Texture2D healthBarFill;
	Texture2D gainsBar;
	Texture2D gainsBarFill;
	Texture2D gainsBarFillGravity;
	Texture2D gainsBarFillElectricityHigh;
	Texture2D gainsBarFillElectricityMid;
	Texture2D gainsBarFillElectricityLow;
	Texture2D gainsBarFillLevitation;
	Texture2D powerIconGravity;
	Texture2D powerIconElectricity;
	Texture2D powerIconLevitation;
	Texture2D livesHeartIcon;

	Vector2 viewRectBottomLeft;
	Vector2 viewRectTopRight;

	Vector2 elecThresh;

	float windowaspect;
	float targetaspect;

	bool powerInUse;
	Vector3 gainsBarLocation;

	void Start () 
	{
		// cache references
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		playerScript = GameObject.Find("Lucina").GetComponent<PlayerScript>();
		elecScript = GameObject.Find("Lucina").GetComponent<ElectricityScript>();

		healthBar = Resources.Load("Materials/Textures/health-bar-with-highlights") as Texture2D;
		healthBarFill = CreateHealthBarTexture();
		gainsBar = Resources.Load("Materials/Textures/gains-bar-with-highlights") as Texture2D;
		gainsBarFillGravity = CreateGainsBarTexture(PowerType.Gravity, 0);
		gainsBarFillElectricityHigh = CreateGainsBarTexture(PowerType.Electricity, 0);
		gainsBarFillElectricityMid = CreateGainsBarTexture(PowerType.Electricity, 1);
		gainsBarFillElectricityLow = CreateGainsBarTexture(PowerType.Electricity, 2);
		gainsBarFillLevitation = CreateGainsBarTexture(PowerType.Levitation, 0);
		gainsBarFill = gainsBarFillLevitation;
		powerIconGravity = Resources.Load("Materials/Textures/power-icon-gravity") as Texture2D;
		powerIconElectricity = Resources.Load("Materials/Textures/power-icon-electricity") as Texture2D;
		powerIconLevitation = Resources.Load("Materials/Textures/power-icon-levitation") as Texture2D;
		livesHeartIcon = Resources.Load("Materials/Textures/lives-heart") as Texture2D;

		elecThresh = Vector2.zero;

		viewRectBottomLeft = Camera.main.ViewportToScreenPoint(new Vector3(Camera.main.rect.x, Camera.main.rect.y, 0));
		viewRectTopRight = Camera.main.ViewportToScreenPoint(new Vector3(Camera.main.rect.x + Camera.main.rect.width, Camera.main.rect.y + Camera.main.rect.height, 0));

		windowaspect = (float)Screen.width / (float)Screen.height;
		targetaspect = 16.0f / 9.0f;

		powerInUse = false;
		gainsBarLocation = Vector3.zero;
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

	// electricityTier - 0 for high, 1 for mid, 2 for low
	private Texture2D CreateGainsBarTexture(PowerType power, int electricityTier) {
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

		// Electricity High
		Vector3 eHHigh = new Vector3(231, 76, 76);
		Vector3 eHMid = new Vector3(182, 30, 24);
		Vector3 eHLow = new Vector3(156, 12, 15);

		// Electricity Mid
		Vector3 eMHigh = new Vector3(72, 225, 142);
		Vector3 eMMid = new Vector3(24, 182, 104);
		Vector3 eMLow = new Vector3(12, 156, 76);

		// Electricity Low
		Vector3 eLHigh = new Vector3(250, 223, 93);
		Vector3 eLMid = new Vector3(239, 209, 11);
		Vector3 eLLow = new Vector3(213, 172, 5);

		Vector3 high;
		Vector3 mid;
		Vector3 low;

		switch (power) {
			case PowerType.Levitation:
				high = lHigh;
				mid = lMid;
				low = lLow;
				break;
			case PowerType.Electricity:
				if (electricityTier == 0) {
					// High (red)
					high = eHHigh;
					mid = eHMid;
					low = eHLow;
				} else if (electricityTier == 1) {
					// Mid (green)
					high = eMHigh;
					mid = eMMid;
					low = eMLow;
				} else {
					// Low (yellow)
					high = eLHigh;
					mid = eLMid;
					low = eLLow;
				}
				break;
			default:
				// Gravity
				high = gHigh;
				mid = gMid;
				low = gLow;
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
		float healthRatio = (float)gameManager.GetTotalHealth() / gameManager.GetMaxHealth();

		// Fraction of the screen width we want the health bar to take up
		float mult = 0.35f;

		// New width and height of health bar on screen w.r.t our Screen size
		int w = (int)(Screen.width * mult);
		int h = (int)(w * healthBar.height / healthBar.width);

		// Bar border thickness in vert and horiz directions. Need to calculate due to resizing from original size.
		// 7 comes from the full size border being 7 pixels. +1 because it usually looks better with that buffer.
		int bw = (int)((float)h / healthBar.height * 7) + 1;
		int bh = (int)((float)w /healthBar.width * 7) + 1;

		float x = (int)(Screen.height * 0.025f);
		float y = (Mathf.Abs(windowaspect - targetaspect) < 0.001f) ? (int)(Screen.height * 0.025f) : viewRectBottomLeft.y - Screen.height / 10f + 14f;

		GUI.BeginGroup(new Rect(x, y, w, h));
			GUI.DrawTexture(new Rect(0, 0, w, h), healthBar);
			if (healthRatio > 0) GUI.DrawTexture(new Rect(bw, bh, (int)((w - bw * 2) * healthRatio), h - bh * 2), healthBarFill);
		GUI.EndGroup();
	}

	private void DrawGainsBar() {
		// If we're not applying power anymore, flip the bool and don't draw the gains bar
		if (!playerScript.PowerIsActive()) {
			if (powerInUse) powerInUse = false;
			return;
		}

		// Else if power is active, flip the bool and set the location of the gains bar using
		// the INITIAL MOUSE CLICK position and some offset
		if (!powerInUse) {
			powerInUse = true;
			// Mouse pos in pixel coordinates
			Vector3 pos = Input.mousePosition;
			// GUI takes coordinates where y is flipped from screen/pixel coordinates given from Unity (stupid)
			gainsBarLocation = new Vector3(pos.x, Screen.height - pos.y, pos.z);
		}

		float gainsRatio = (float)playerScript.GetCurrentPowerGain() / playerScript.GetCurrentPowerMaxGain();
		
		// Fraction of the screen width we want the gains bar to take up
		float mult = 0.4f;
		
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
			case PowerType.Electricity:
				if (gainsRatio < elecThresh.x/100f) {
					gainsBarFill = gainsBarFillElectricityLow;
				} else if (gainsRatio >= elecThresh.x/100f && gainsRatio <= elecThresh.y/100f) {
					gainsBarFill = gainsBarFillElectricityMid;
				} else if (gainsRatio > elecThresh.y/100f) {
					gainsBarFill = gainsBarFillElectricityHigh;
				}
				break;
			default:
				gainsBarFill = gainsBarFillGravity;
				break;
		}

		int fillHeight = (int)((h - bh * 2) * gainsRatio);

		// Calculate position of gains bar so it doesn't go off the screen too
		float posx = gainsBarLocation.x + (Screen.width * 0.05f);
		float posy = gainsBarLocation.y - h/2f;
		if (posx + w > Screen.width - 10) posx = Screen.width - 10 - w;
		if (posy < 10) posy = 10;
		else if (posy + h > Screen.height - 10) posy = Screen.height - 10 - h;

		GUI.BeginGroup(new Rect(posx, posy, w, h));
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

		float x = (Mathf.Abs(windowaspect - targetaspect) < 0.001f) ? Screen.width - totalWidth - 10 : viewRectTopRight.x - totalWidth - 10;
		float y = (Mathf.Abs(windowaspect - targetaspect) < 0.001f) ? 10 : viewRectBottomLeft.y - Screen.height / 10f + 12f;

		// THE GROUP RECT Y COORDINATE IS SOMEWHAT ARBITRARY BECAUSE I SWEAR TO GOD UNITY'S STUPID COORDINATE SYSTEM ALWAYS GIVES ME SOMETHING WRONG
		GUI.BeginGroup(new Rect(x, y, totalWidth, dim));
			if (gameManager.GetHasLevitation()) DrawIconWithOpacity(0, 0, dim, dim, powerIconLevitation, (playerScript.GetCurrentPower() == PowerType.Levitation) ? 1 : faded);
			if (gameManager.GetHasGravity())DrawIconWithOpacity(dim + padding, 0, dim, dim, powerIconGravity, (playerScript.GetCurrentPower() == PowerType.Gravity) ? 1 : faded);
			if (gameManager.GetHasElectricity())DrawIconWithOpacity(dim * 2 + padding * 2, 0, dim, dim, powerIconElectricity, (playerScript.GetCurrentPower() == PowerType.Electricity) ? 1 : faded); 
		GUI.EndGroup();
	}

	private void DrawLivesIcons() {
		// TODO: Temp var, hook up to real
		int numLivesLeft = gameManager.GetTotalLives();
		float scale = 0.050f;
		int dimH = (int)(scale * Screen.height);
		int dimW = (int)(dimH * 461f/415f);
		int padding = 20;

		float x = (int)(Screen.height * 0.025f);

		// Height of health bar... just copied and pasted.
		float mult = 0.35f;
		int w = (int)(Screen.width * mult);
		int h = (int)(w * healthBar.height / healthBar.width);
		float y = (Mathf.Abs(windowaspect - targetaspect) < 0.001f) ? (int)(Screen.height * 0.025f) : viewRectBottomLeft.y - Screen.height / 10f + 14f;

		GUI.BeginGroup(new Rect(x, y + dimH + (int)(0.016f * Screen.height), (dimW + padding) * numLivesLeft, dimH));
		for (int i = 0; i < numLivesLeft; i++) {
			GUI.DrawTexture(new Rect(10 + (dimW + padding) * i, 0, dimW, dimH), livesHeartIcon);
		}
        GUI.EndGroup();
    }

	public void SetElectricityThresholds(Vector2 thresh) {
		elecThresh = thresh;
	}

	void OnGUI() 
	{
		// Powers
		DrawPowerIcons();
		DrawLivesIcons();

		// Health
		DrawHealthBar();
		DrawGainsBar();
	}
}
