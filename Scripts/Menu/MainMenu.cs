using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{

	public GUISkin gSkin;
	private bool isLoading = false;
	
	private float startTime = 0.1f;
 
	private long tris = 0;
	private long verts = 0;
 
	private bool showfps = false;
	private bool showtris = false;
	private bool showvtx = false;
 
	public Color lowFPSColor = Color.red;
	public Color highFPSColor = Color.green;
 
	public int lowFPS = 30;
	public int highFPS = 50;
 
	public Color statColor = Color.yellow;
 
	public enum Page {
	    None,Options
	}
 
	private Page currentPage;
	
	private bool showControls = false;
	public Texture2D controlDiagram;
 
	private float[] fpsarray;
	private float fps;
 
	private int toolbarInt = 0;
	private string[]  toolbarstrings =  {"Game","Graphics","System"};

    // Volume, pitch, yaw, fps settings
    public float volume;
    public float pitch;
    public float yaw;
    public float showFpsFloat;

	void  OnGUI ()
	{
		if (gSkin)
			GUI.skin = gSkin;
		else
			Debug.Log ("StartMenuGUI: GUI Skin object missing!");

		GUIStyle backgroundStyle = new GUIStyle ();
		GUI.Label (new Rect ((Screen.width - (Screen.height * 2)) * 0.75f, 0, Screen.height * 2,
        Screen.height), "", backgroundStyle);
        
		GUI.Label (new Rect ((Screen.width / 2) - 350, (Screen.height / 3) - 120, 70, 70), "Space", 
       "mainMenuTitleLayer");
		GUI.Label (new Rect ((Screen.width / 2) - 350, (Screen.height / 3) - 50, 70, 70), "Battle",
       "mainMenuTitleLayer");
		GUI.Label (new Rect ((Screen.width / 2) - 350, (Screen.height / 3) + 20, 70, 70), "Game",
       "mainMenuTitleLayer");
		GUI.Label (new Rect ((Screen.width / 2) - 350, (Screen.height / 3) - 120, 70, 70), "Space",
       "mainMenuTitle");
		GUI.Label (new Rect ((Screen.width / 2) - 350, (Screen.height / 3) - 50, 70, 70), "Battle",
       "mainMenuTitle");
		GUI.Label (new Rect ((Screen.width / 2) - 350, (Screen.height / 3) + 20, 70, 70), "Game",
       "mainMenuTitle");
       
		GUI.Label (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 35, 200, 35), "     Level Select",
       "textLayer");
		GUI.Label (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 70, 200, 35), "     Options",
       "textLayer");
		//GUI.Label (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 105, 200, 35), "     Garage",
       //"textLayer");
		GUI.Label (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 105, 200, 35), "     Controls",
       "textLayer");
		GUI.Label (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 140, 200, 35), "     Store",
       "textLayer");
		GUI.Label (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 175, 200, 35), "     Quit",
       "textLayer");
       

		if (GUI.Button (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 35, 200, 35), "     Level Select", "text")) {
			isLoading = true;
			Application.LoadLevel ("LevelSelect"); // load the level select menu.
		}
		if (GUI.Button (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 70, 200, 35), "     Options", "text")) {
            switch (currentPage) 
			{
	            case Page.None: 
					PauseGame(); 
					break;
 
				case Page.Options: 
					UnPauseGame(); 
					break;
 
				default: 
					currentPage = Page.None;
					break;
	        }
		}
		if (GUI.Button (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 105, 200, 35), "     Controls", "text")) {
    		showControls = true;
		}
		//if (GUI.Button (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 105, 200, 35), "     Garage", "textGrey")) {
    //
		//}
		if (GUI.Button (new Rect ((Screen.width / 2) - 360, (Screen.height / 2) + 140, 200, 35), "     Store", "text")) {
    		isLoading = true;
			Application.LoadLevel ("Store");
		}

        if (GUI.Button(new Rect((Screen.width / 2) - 360, (Screen.height / 2) + 175, 200, 35), "     Quit", "text")){
            Application.Quit ();
        }
		
		//
		
		if (isLoading)
			GUI.Label (new Rect ((Screen.width / 3) * 2, (Screen.height / 2) + 175, 100, 35),
        "Loading...", "text");
		
		ShowStatNums();
	    if (IsGamePaused()) {
	        GUI.color = statColor;
	        ShowToolbar();
	    }
		
		if (showControls == true){
			GUI.color = Color.white;
			if (GUI.Button (new Rect(0, 0, Screen.width, Screen.height), controlDiagram)){
				showControls = false;
			}
		}
	}
 
 
	void Start() {
	    fpsarray = new float[Screen.width];
	    Time.timeScale = 1;
        if (!PlayerPrefs.HasKey("volume")){
            PlayerPrefs.SetFloat("volume", 1);
        }
        if (!PlayerPrefs.HasKey("pitch"))
        {
            PlayerPrefs.SetFloat("pitch", 80.0f);
        }
        if (!PlayerPrefs.HasKey("yaw"))
        {
            PlayerPrefs.SetFloat("yaw", 80.0f);
        }
        if (!PlayerPrefs.HasKey("showFpsFloat"))
        {
            PlayerPrefs.SetFloat("showFpsFloat", 0);
        }
        volume = PlayerPrefs.GetFloat("volume");
        pitch = PlayerPrefs.GetFloat("pitch");
        yaw = PlayerPrefs.GetFloat("yaw");
        showFpsFloat = PlayerPrefs.GetFloat("showFpsFloat");
        if (PlayerPrefs.GetFloat("showFpsFloat") == 1)
            showfps = true;
        else
            showfps = false;
	}
 
	void ScrollFPS() {
	    for (int x = 1; x < fpsarray.Length; ++x) {
	        fpsarray[x-1]=fpsarray[x];
	    }
	    if (fps < 1000) {
	        fpsarray[fpsarray.Length - 1]=fps;
	    }
	}
 
	static bool IsDashboard() {
	    return Application.platform == RuntimePlatform.OSXDashboardPlayer;
	}
 
	static bool IsBrowser() {
	    return (Application.platform == RuntimePlatform.WindowsWebPlayer ||
	        Application.platform == RuntimePlatform.OSXWebPlayer);
	}
 
	void LateUpdate () {
	    if (showfps) {
	        FPSUpdate();
	    }
	}
 
	void ShowToolbar() {
	    BeginPage(300,300);
	    toolbarInt = GUILayout.Toolbar (toolbarInt, toolbarstrings);
	    switch (toolbarInt) {
	        case 0: VolumeControl(); break;
	        case 2: ShowDevice(); break;
	        case 1: Qualities(); QualityControl(); break;
	    }
	    EndPage();
	}
 
	void ShowBackButton() {
	    if (GUI.Button(new Rect((Screen.width/2)+200, (Screen.height/2) + 38, 100, 20),"Save")) {
            volume = AudioListener.volume;
            PlayerPrefs.SetFloat("volume", volume);
            PlayerPrefs.SetFloat("pitch", pitch);
            PlayerPrefs.SetFloat("yaw", yaw);
            PlayerPrefs.SetFloat("showFpsFloat", showFpsFloat);
            if (showfps == true)
                PlayerPrefs.SetFloat("showFpsFloat", 1);
            else
                PlayerPrefs.SetFloat("showFpsFloat", 0);
            UnPauseGame();
	    }
	}
 
	void ShowDevice() {
	    GUILayout.Label("\nUnity player version "+Application.unityVersion);
	    GUILayout.Label("Graphics: "+SystemInfo.graphicsDeviceName+" "+
	    SystemInfo.graphicsMemorySize+"MB\n"+
	    SystemInfo.graphicsDeviceVersion+"\n"+
	    SystemInfo.graphicsDeviceVendor);
	    GUILayout.Label("Shadows: "+SystemInfo.supportsShadows);
	    GUILayout.Label("Image Effects: "+SystemInfo.supportsImageEffects);
	    GUILayout.Label("Render Textures: "+SystemInfo.supportsRenderTextures);
	}
 
	void Qualities() {
        GUILayout.Label("");
	    switch (QualitySettings.GetQualityLevel()) 
		{
	        case 0:
	        	GUILayout.Box("Fastest");
	        	break;
	        case 1:
	        	GUILayout.Box("Fast");
	        	break;
	        case 2:
		        GUILayout.Box("Simple");
		        break;
	        case 3:
		        GUILayout.Box("Good");
		        break;
	        case 4:
		        GUILayout.Box("Beautiful");
		        break;
	        case 5:
	        	GUILayout.Box("Fantastic");
	        	break;
	    }
        GUILayout.Label("");
	}
 
	void QualityControl() {
	    GUILayout.BeginHorizontal();
	    if (GUILayout.Button("Decrease")) {
	        QualitySettings.DecreaseLevel();
	    }
	    if (GUILayout.Button("Increase")) {
	        QualitySettings.IncreaseLevel();
	    }
        GUILayout.Label("");
        GUILayout.Label("");
        GUILayout.Label("");
	    GUILayout.EndHorizontal();
        GUILayout.Label("");
        GUILayout.BeginHorizontal();
        showfps = GUILayout.Toggle(showfps, " Display FPS");
        GUILayout.Label("");
        GUILayout.Label("");
        GUILayout.Label("");
        GUILayout.EndHorizontal();
	}
 
	void VolumeControl() {
        GUILayout.Label("\nVolume");
        AudioListener.volume = GUILayout.HorizontalSlider(AudioListener.volume, 0, 1);
        GUILayout.Label("Pitch Sensitivity");
        pitch = GUILayout.HorizontalSlider(pitch, 10.0f, 200.0f);
        GUILayout.Label("Yaw Sensitivity");
        yaw = GUILayout.HorizontalSlider(yaw, 10.0f, 200.0f);
        GUILayout.Label("");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset"))
        {
            AudioListener.volume = 1;
            pitch = 80.0f;
            yaw = 80.0f;
        }
        GUILayout.Label("");
        GUILayout.Label("");
        GUILayout.Label("");
        GUILayout.EndHorizontal();
	}
 
	void FPSUpdate() {
	    float delta = Time.smoothDeltaTime;
	        if (!IsGamePaused() && delta !=0.0) {
	            fps = 1 / delta;
	        }
	}
 
	void ShowStatNums() {
	    GUILayout.BeginArea( new Rect(Screen.width - 100, 10, 100, 200));
	    if (showfps) {
	        string fpsstring= fps.ToString ("#,##0 fps");
	        GUI.color = Color.Lerp(lowFPSColor, highFPSColor,(fps-lowFPS)/(highFPS-lowFPS));
	        GUILayout.Label (fpsstring);
	    }
	    if (showtris || showvtx) {
	        GetObjectStats();
	        GUI.color = statColor;
	    }
	    if (showtris) {
	        GUILayout.Label (tris+"tri");
	    }
	    if (showvtx) {
	        GUILayout.Label (verts+"vtx");
	    }
	    GUILayout.EndArea();
	}
 
	void BeginPage(int width, int height) {
	    GUILayout.BeginArea( new Rect(Screen.width/2, (Screen.height-height)/2, width, height));
	}
 
	void EndPage() {
	    GUILayout.EndArea();
	    ShowBackButton();
	}
 
	bool IsBeginning() {
	    return (Time.time < startTime);
	}
 
	void GetObjectStats() {
	    verts = 0;
	    tris = 0;
	    GameObject[] ob = FindObjectsOfType(typeof(GameObject)) as GameObject[];
	    foreach (GameObject obj in ob) {
	        GetObjectStats(obj);
	    }
	}
 
	void GetObjectStats(GameObject obj) {
	   	Component[] filters;
	    filters = obj.GetComponentsInChildren<MeshFilter>();
	    foreach( MeshFilter f  in filters )
	    {
	        tris += f.sharedMesh.triangles.Length/3;
	      	verts += f.sharedMesh.vertexCount;
	    }
	}
 
	void PauseGame() {
	    Time.timeScale = 0;
	    currentPage = Page.Options;
	}
 
	void UnPauseGame() {
	    Time.timeScale = 1;
		currentPage = Page.None;
	}
 
	bool IsGamePaused() {
	    return (Time.timeScale == 0);
	}
 
	void OnApplicationPause(bool pause) {
	    if (IsGamePaused()) {
	        AudioListener.pause = true;
	    }
	}
}