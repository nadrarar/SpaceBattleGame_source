using UnityEngine;
using System.Collections;

public class StoreMenu : MonoBehaviour {

	public GUISkin upSkin;
	public GUISkin downSkin;
	public Texture2D backdrop;
	private bool isLoading = false;
	public bool hasMissiles = false;
	public bool hasScatter = false;
    public bool hasBurst = false;
	public bool hasSonic = false;
    public Texture2D missileIcon;
    public Texture2D scatterShotIcon;
    public Texture2D burstGunIcon;
	public Texture2D sonicWaveIcon;
	public int points = 0;
	
	void Start(){
		PlayerPrefs.SetInt("hasMissiles",10);
		hasMissiles = PlayerPrefs.HasKey("hasMissiles");
		hasScatter = PlayerPrefs.HasKey("hasScatter");
        hasBurst = PlayerPrefs.HasKey("hasBurst");
		hasSonic = PlayerPrefs.HasKey("hasSonic");
		points = PlayerPrefs.GetInt("Points");
	}
	
	void  OnGUI ()
	{
        if (upSkin && downSkin)
			GUI.skin = upSkin;
		else
			Debug.Log ("StartMenuGUI: GUI Skin object missing!");

		GUIStyle backgroundStyle = new GUIStyle ();
		backgroundStyle.normal.background = backdrop;
		GUI.Label (new Rect ((Screen.width - (Screen.height * 2)) * 0.75f, 0, Screen.height * 2,
        Screen.height), "", backgroundStyle);

        GUI.Label(new Rect((Screen.width / 2) - 350, (Screen.height / 3) - 120, 160, 70), "Store",
       "LabelLayer");
		GUI.Label (new Rect ((Screen.width / 2) - 350, (Screen.height / 3) - 120, 160, 70), "Store", 
       "label");
        
		
		GUI.Label (new Rect ((Screen.width / 2) + 200, (Screen.height / 3) - 80, 50, 25), "Points:", 
       "box");
		GUI.Label (new Rect ((Screen.width / 2) + 250, (Screen.height / 3) - 80, 50, 25), points.ToString(), 
       "box");

        // Weapon icons
        GUI.Label(new Rect((Screen.width / 2) - 360, (Screen.height / 2), 35, 35),
            new GUIContent("", missileIcon));
        GUI.Label(new Rect((Screen.width / 2) - 360, (Screen.height / 2) + 35, 35, 35),
            new GUIContent("", scatterShotIcon));
        GUI.Label(new Rect((Screen.width / 2) - 360, (Screen.height / 2) + 70, 35, 35),
            new GUIContent("", burstGunIcon));
		GUI.Label(new Rect((Screen.width / 2) - 360, (Screen.height / 2) + 105, 35, 35),
            new GUIContent("", sonicWaveIcon));
		
        // Weapon descriptions
        GUI.Label(new Rect((Screen.width / 2) - 360, (Screen.height / 2), 235, 35),
            new GUIContent("", "The Missile Launcher fires guided missiles\n" +
                "that seek enemy ships.\n\n" +
                "Damage: 30\n" +
                "Velocity: 60\n" +
                "Ammo Capacity: 20\n" +
                "Reload Time: 0.6s"));
        GUI.Label(new Rect((Screen.width / 2), (Screen.height / 2), 300, 160),
            GUI.tooltip, "button");
		
        GUI.Label(new Rect((Screen.width / 2) - 360, (Screen.height / 2) + 35, 235, 35),
            new GUIContent("", "The Scatter Shot fires a cone of lasers\n" +
                "in a Shotgun pattern.\n\n" +
                "Damage: 30 x 5\n" +
                "Velocity: 140\n" +
                "Ammo Capacity: 30\n" +
                "Reload Time: 0.4s"));
        GUI.Label(new Rect((Screen.width / 2), (Screen.height / 2), 300, 160),
            GUI.tooltip, "button");
		
        GUI.Label(new Rect((Screen.width / 2) - 360, (Screen.height / 2) + 70, 235, 35),
            new GUIContent("", "The Burst Gun fires 3 round bursts\n" +
                "at high velocity.\n\n" +
                "Damage: 30\n" +
                "Velocity: 180\n" +
                "Ammo Capacity: 60 (3 per burst)\n" +
                "Reload Time: 0.6s (0.3s between bursts)"));
        GUI.Label(new Rect((Screen.width / 2), (Screen.height / 2), 300, 160),
            GUI.tooltip, "button");
		
		 GUI.Label(new Rect((Screen.width / 2) - 360, (Screen.height / 2) + 105, 235, 35),
            new GUIContent("", "The Sonic Wave damages enemies\n" +
                "in a 10 m radius.\n\n" +
                "Damage: 100\n" +
                "Velocity: Instantaneous\n" +
                "Ammo Capacity: 5 \n" +
                "Reload Time: 5s"));
        GUI.Label(new Rect((Screen.width / 2), (Screen.height / 2), 300, 160),
            GUI.tooltip, "button");
		
		if (hasMissiles)
			GUI.skin = downSkin;
		else 
			GUI.skin = upSkin;
		if (GUI.Button (new Rect ((Screen.width / 2) - 325, (Screen.height / 2), 200, 35), "100 Pts     Missile Launcher", "button")) {
            if(!hasMissiles)
				buyMissiles();
		}
		if (hasScatter)
			GUI.skin = downSkin;
		else 
			GUI.skin = upSkin;
		if (GUI.Button (new Rect ((Screen.width / 2) - 325, (Screen.height / 2) + 35, 200, 35), "100 Pts     Scatter Shot", "button")) {
            if(!hasScatter)
				buyScatter();
		}
        if (hasBurst)
            GUI.skin = downSkin;
        else
            GUI.skin = upSkin;
        if (GUI.Button(new Rect((Screen.width / 2) - 325, (Screen.height / 2) + 70, 200, 35), "100 Pts     Burst Gun", "button"))
        {
            if (!hasBurst)
                buyBurst();
        }
		
		if (hasSonic)
            GUI.skin = downSkin;
        else
            GUI.skin = upSkin;
        if (GUI.Button(new Rect((Screen.width / 2) - 325, (Screen.height / 2) + 105, 200, 35), "100 Pts     Sonic Wave", "button"))
        {
            if (!hasSonic)
                buySonic();
        }
		
		GUI.skin = upSkin;

		if (GUI.Button (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 160, 235, 35), "Undo changes", "button")){
			hasMissiles = PlayerPrefs.HasKey("hasMissiles");
			hasScatter = PlayerPrefs.HasKey("hasScatter");
            hasBurst = PlayerPrefs.HasKey("hasBurst");
			hasSonic = PlayerPrefs.HasKey("hasSonic");
			points = PlayerPrefs.GetInt("Points");
		}
		if (GUI.Button (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 195, 235, 35), "Reset Game", "button")){
			isLoading = false;
			hasMissiles = true;
			hasScatter = false;
            hasBurst = false;
            hasSonic = false;
			points = 0;
			PlayerPrefs.DeleteKey("hasMissiles");
			PlayerPrefs.SetInt("hasMissiles",10);
			PlayerPrefs.DeleteKey("hasScatter");
            PlayerPrefs.DeleteKey("hasBurst");
			PlayerPrefs.DeleteKey("hasSonic");
			PlayerPrefs.DeleteKey("Points");
		}
		if (GUI.Button (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 230, 235, 35), "Save and Return", "button")){
			isLoading = true;
			PlayerPrefs.SetInt("Points", points);
			if(hasMissiles){
				PlayerPrefs.SetInt ("hasMissiles", 10);
			}
			if(hasScatter){
				PlayerPrefs.SetInt ("hasScatter", 10);
			}
            if (hasBurst)
            {
                PlayerPrefs.SetInt("hasBurst", 10);
            }
			if (hasSonic)
            {
                PlayerPrefs.SetInt("hasSonic", 10);
            }
			PlayerPrefs.Save();
			Application.LoadLevel ("MainMenu"); // load the game level.
		}

		if (isLoading)
			GUI.Label (new Rect ((Screen.width / 3) * 2, (Screen.height / 2) + 175, 100, 35),
        "Loading...", "box");

        // Hit "P" to add points
        if (Input.GetKeyUp(KeyCode.P))
        {
            points += 100;

        }
	}
	void  buyMissiles (){
		if(points >= 100){
			points -= 100;
			hasMissiles = true;
		}	
	}
	void buyScatter (){
		if(points >= 100){
			points -= 100;
			hasScatter = true;
		}
	}
    void buyBurst()
    {
        if (points >= 100)
        {
            points -= 100;
            hasBurst = true;
        }
    }
	void buySonic()
    {
        if (points >= 100)
        {
            points -= 100;
            hasSonic = true;
        }
    }
} 
