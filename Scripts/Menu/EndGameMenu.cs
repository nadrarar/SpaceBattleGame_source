using UnityEngine;
using System.Collections;
 
public class EndGameMenu : MonoBehaviour
{
 
	public Color statColor = Color.white;

    public int startPoints = 0;
    public int endPoints = 0;
    private bool subtracted = false;
    // used to calculate difference between points at the start and end of game to determine points earned
    private bool isLoading = false;
    public bool levelComplete = false;

    public enum Page {
        None, Main
    }

    private Page currentPage;

	void Start() {
        startPoints = PlayerPrefs.GetInt("Points");
	}

    public void Update(){
        //levelComplete = environmentScript.GetComponent<EnvironmentController>().levelComplete;
	}
 
	void OnGUI () {
        if (!GameObject.FindGameObjectWithTag("Player") || levelComplete == true)
        {
            GUI.color = statColor;
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            switch (currentPage)
            {
                case Page.Main:
                    EndGame(); break;
                default:
                    currentPage = Page.Main;
                    break;
            }
        }
		if (isLoading){
            GUI.Label (new Rect( (Screen.width/3)*2, (Screen.height/2) + 175, 100, 35),
            "Loading...");
		}
	}
 
	void BeginPage(int width, int height) {
	    GUILayout.BeginArea( new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height));
	}
 
	void EndPage() {
	    GUILayout.EndArea();
	}
 
	void EndGame() {
	    BeginPage(200,200);
        Screen.showCursor = true;
        Screen.lockCursor = false;
        if (levelComplete == true)
        {
            GUILayout.Box("Level Complete!");
            endPoints = PlayerPrefs.GetInt("Points");
            if (subtracted == false)
            {
                startPoints = endPoints - startPoints;
                subtracted = true;
            }
            GUILayout.Box("Points Earned: " + startPoints);
            GUILayout.Box("Total Points: " + endPoints);
        }
        else
        {
            GUILayout.Box("Your ship was destroyed.");
        }
	    if (GUILayout.Button ("Restart")) {
	        isLoading = true;
            Screen.showCursor = false;
            Screen.lockCursor = true;
            Application.LoadLevel(Application.loadedLevel);
	    }
	    if (GUILayout.Button ("Level Select")) {
	        isLoading = true;
            Application.LoadLevel("LevelSelect");
	    }
	    if (GUILayout.Button ("Main Menu")) {
            isLoading = true;
	        Application.LoadLevel ("mainMenu");
	    }
	    EndPage();
	}
}