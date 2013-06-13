using UnityEngine;
using System.Collections;
 
public class PauseMenu : MonoBehaviour {
 
	private float startTime = 0.1f;
 
	private long tris = 0;
	private long verts = 0;
	private float savedTimeScale;
 
	private bool showfps = false;
	private bool showtris = false;
	private bool showvtx = false;
	private bool isLoading = false;
 
	public Color lowFPSColor = Color.red;
	public Color highFPSColor = Color.green;
 
	public int lowFPS = 30;
	public int highFPS = 50;
 
	public Color statColor = Color.yellow;

    public Ship shipScript;
 
	public enum Page {
	    None,Main,Options
	}
 
	private Page currentPage;
 
	private float[] fpsarray;
	private float fps;
 
	private int toolbarInt = 0;
    private string[] toolbarstrings = { "Game", "Graphics", "System" };

    // Volume, sensitivity, fps settings
    public float volume;
    public float sensitivity;
    public float showFpsFloat;
 
	void Start() {
        GameObject player = GameObject.Find("Player");
        shipScript = player.GetComponent<Ship>();
	    fpsarray = new float[Screen.width];
	    Time.timeScale = 1;
		if(PlayerPrefs.GetFloat("sensitivity") == 0)
			PlayerPrefs.SetFloat("sensitivity",.18f);
        volume = PlayerPrefs.GetFloat("volume");
        sensitivity = PlayerPrefs.GetFloat("sensitivity");
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
 
		if (Input.GetKeyDown("escape")) 
		{
	        switch (currentPage) 
			{
	            case Page.None: 
					PauseGame(); 
					break;
 
				case Page.Main: 
					if (!IsBeginning()) 
						UnPauseGame(); 
					break;
 
				default: 
					currentPage = Page.Main;
					break;
	        }
	    }
	}
 
	void OnGUI () {
	    ShowStatNums();
	    if (IsGamePaused()) {
	        GUI.color = statColor;
			GUI.Box (new Rect (0,0,Screen.width,Screen.height), "");
	        switch (currentPage) {
	            case Page.Main: MainPauseMenu(); break;
	            case Page.Options: ShowToolbar(); break;
	        }
	    }
		if (isLoading){
            GUI.Label (new Rect( (Screen.width/3)*2, (Screen.height/2) + 175, 100, 35),
            "Loading...");
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
        if (GUI.Button(new Rect((Screen.width / 2) + 50, (Screen.height / 2) + 38, 100, 20), "Save"))
        {
            volume = AudioListener.volume;
            PlayerPrefs.SetFloat("volume", volume);
            PlayerPrefs.SetFloat("sensitivity", sensitivity);
            PlayerPrefs.SetFloat("showFpsFloat", showFpsFloat);
            if (showfps == true)
                PlayerPrefs.SetFloat("showFpsFloat", 1);
            else
                PlayerPrefs.SetFloat("showFpsFloat", 0);
            currentPage = Page.Main;
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
        GUILayout.Label("Mouse Sensitivity");
		sensitivity = GUILayout.HorizontalSlider(sensitivity, 0.04f, 0.54f);
        GUILayout.Label("");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset"))
        {
            AudioListener.volume = 1;
            sensitivity = .16f;
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
	    GUILayout.BeginArea( new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height));
	}
 
	void EndPage() {
	    GUILayout.EndArea();
	    if (currentPage != Page.Main) {
	        ShowBackButton();
	    }
	}
 
	bool IsBeginning() {
	    return (Time.time < startTime);
	}
 
 
	void MainPauseMenu() {
	    BeginPage(200,200);
	    if (GUILayout.Button ("Resume")) {
	        UnPauseGame();
	    }
	    if (GUILayout.Button ("Options")) {
	        currentPage = Page.Options;
	    }
	    if (GUILayout.Button ("Restart")) {
            AudioListener.pause = false;
	        isLoading = true;
            Application.LoadLevel(Application.loadedLevel);
	    }
	    if (GUILayout.Button ("Quit")) {
            isLoading = true;
            AudioListener.pause = false;
	        Application.LoadLevel ("mainMenu");
	    }
	    EndPage();
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
	    savedTimeScale = Time.timeScale;
	    Time.timeScale = 0;
	    AudioListener.pause = true;
	    currentPage = Page.Main;
		Screen.showCursor = true;
		Screen.lockCursor = false;
	}
 
	void UnPauseGame() {
	    Time.timeScale = savedTimeScale;
	    AudioListener.pause = false;
 
		currentPage = Page.None;
		Screen.showCursor = false;
		Screen.lockCursor = true;
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