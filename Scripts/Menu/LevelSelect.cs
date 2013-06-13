using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LevelSelect : MonoBehaviour {
	
	public GUISkin gSkin;
	
	public String[] levelNames;
	public String[] levelScenes;
	public String[] levelDescriptions;
	public Texture2D[] levelTextures;
	public Material[] levelSkyBoxes;
	public int selectedLevel = 0;
    private bool isLoading = false;
	
	// this vector keeps track of how far down the level select list is scrolled
	Vector2 scrollValue;
	
	void OnGUI (){
		
		// indent is used to center the menu for varying resolutions
		int indent = (Screen.width - 560)/2;
		
		// Box to contain menu
		GUI.Box (new Rect(indent - 10, 10, 580, Screen.height - 20), "");
		
		if (gSkin)
			GUI.skin = gSkin;
		else
			Debug.Log ("StartMenuGUI: GUI Skin object missing!");
		
		// Title label
		GUI.Label (new Rect(indent + 30, 30, 0, 0), "Level Select", "text");
		
		// selectionGrid height is used to determine how big the box for the level select list should be
		int selectionGridHeight = 20 * levelNames.Length;
		
		// Draw level select list
		scrollValue = GUI.BeginScrollView(new Rect (indent + 30, 55, 200, Screen.height - 130), new Vector2 (0, scrollValue.y), new Rect(0, 0, 180, selectionGridHeight));
			GUI.Box (new Rect(0, 0, 180, selectionGridHeight + 30), "");
			selectedLevel = GUI.SelectionGrid(new Rect(15, 15, 150, selectionGridHeight), selectedLevel, levelNames, 1);
		GUI.EndScrollView();
		
		// Description text
		GUI.TextArea (new Rect(indent + 235, 365, 300, Screen.height - 385), levelDescriptions[selectedLevel]);
		
		// Level preview
		GUI.Box (new Rect(indent + 235, 55, 300, 300), levelTextures[selectedLevel]);
		
		// Background image (skybox)
		GetComponent<Skybox>().material = levelSkyBoxes[selectedLevel];
		
		// Begin and Back buttons
		if (GUI.Button (new Rect (indent + 30, Screen.height - 65, 180, 20), "Begin")) {
            isLoading = true;
			Application.LoadLevel (levelScenes[selectedLevel]);
		}
		if (GUI.Button (new Rect (indent + 30, Screen.height - 40, 180, 20), "Back")) {
            isLoading = true;
            Application.LoadLevel ("mainMenu");
		}
        if (isLoading)
            GUI.Label(new Rect((Screen.width / 2) + 100, Screen.height - 72, 100, 35),
        "Loading...", "text");
	}
}
