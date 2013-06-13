using UnityEngine;
using System.Collections;

public class GameHUD : MonoBehaviour {
	
	public Texture2D healthBar;
	public Texture2D shieldBar;
	public Texture2D emptyBar;
	public Texture2D missileIcon;
    public Texture2D scatterShotIcon;
    public Texture2D burstGunIcon;
	public Texture2D sonicGunIcon;
	public Texture2D crosshair;
	private Destructible destructibleScript;
	private FireSecondary secondaryWeaponScript;
	public int playerHealth = 100;
	public int playerShield = 100; 
	public int playerMaximumHealth;
	public int playerMaximumShield;
	public int currentWeapon = 0;
	public int secondaryAmmo = 0;
    public bool levelComplete = false;
    public bool displayWave = false;
    public string waveMessage;
    public string objectiveMessage;
	public bool displayObjective = false;
    private int currentWave = 0;
    private int currentObjective = 0;
	public void Start(){
		//I find the player and weapon instead of using find every frame to get current
		//health and shield because find is very costly 
		GameObject player = GameObject.Find("Player");
		destructibleScript = player.GetComponent<Destructible>();
		secondaryWeaponScript = player.GetComponentInChildren<FireSecondary>();
		//find max health in order to determine the currenthealth/maximumHealth ratio
		playerMaximumHealth = destructibleScript.maxHealth;
		playerMaximumShield = destructibleScript.maxShield;
		
	}
	
	public void StartObjectiveMessage(int objectiveNumber, string message, float messageTimeout = 4.0f){
		currentObjective = objectiveNumber;
		displayObjective = true;
		objectiveMessage = message;
		Invoke("StopObjectiveMessage",messageTimeout);
	}
	public void StopObjectiveMessage(){
		displayObjective = false;
	}

	public void StartWaveMessage(int waveNumber, string message){
		currentWave = waveNumber;
		displayWave = true;
		waveMessage = message;
		Invoke("StopWaveMessage",4.0f);
	}
	public void StopWaveMessage(){
		displayWave = false;
	}
	public void Update(){
		playerHealth = destructibleScript.curHealth;
		playerShield = destructibleScript.curShield;
		currentWeapon = secondaryWeaponScript.GetComponent<FireSecondary>().currentWeaponEquipped();
		secondaryAmmo = secondaryWeaponScript.GetComponent<FireSecondary>().ammo();
	}
	
	public void OnGUI(){
        if (!levelComplete)
        {
            if (displayWave)
            {
                GUIStyle style = GUI.skin.GetStyle ("Label");
				style.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect((Screen.width - 200) / 2, (Screen.height - 200) * 1/5, 200, 200), "Wave " + (1 + currentWave) + " \n" + waveMessage,style);
            }else if(displayObjective){
                GUIStyle style = GUI.skin.GetStyle ("Label");
				style.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect((Screen.width - 200) / 2, (Screen.height - 200) * 1/5, 200, 200), "Objective " + (1 + currentObjective) + " \n" + objectiveMessage,style);
			}
			/*
            GUI.BeginGroup(new Rect(48, 12, 128, 128));
            GUI.Label(new Rect(0, 0, 128, 128), emptyBar);
            GUI.BeginGroup(new Rect(0, 0, 128 * ((float)playerShield) / playerMaximumShield, 128));
            GUI.Label(new Rect(0, 0, 128, 128), shieldBar);
            GUI.EndGroup();
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(48, 42, 128, 128));
            GUI.Label(new Rect(0, 0, 128, 128), emptyBar);
            GUI.BeginGroup(new Rect(0, 0, 128 * ((float)playerHealth) / playerMaximumHealth, 128));
            GUI.Label(new Rect(0, 0, 128, 128), healthBar);
            GUI.EndGroup();
            GUI.EndGroup();
			 */
            if (currentWeapon != -1)
            {
                if (currentWeapon == 0)
                    GUI.Box(new Rect(48, Screen.height - 112, 64, 64), missileIcon);

                else if (currentWeapon == 1)
                    GUI.Box(new Rect(48, Screen.height - 112, 64, 64), scatterShotIcon);

                else if (currentWeapon == 2)
                    GUI.Box(new Rect(48, Screen.height - 112, 64, 64), burstGunIcon);
				
				else if (currentWeapon == 3)
                    GUI.Box(new Rect(48, Screen.height - 112, 64, 64), sonicGunIcon);

                GUI.Box(new Rect(116, Screen.height - 72, 24, 24), secondaryAmmo + "");
            }

            //GUI.Label(new Rect((Screen.width / 2) - 32, (Screen.height / 2), 64, 64), crosshair);
        }
    }

}