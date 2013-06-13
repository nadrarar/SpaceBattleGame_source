using UnityEngine;
using System.Collections;

public class PlayerCursor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		playerController = transform.root.gameObject.GetComponent<PlayerController>();
		if(!playerController) 
			Debug.LogError("error, did not determine the player controller");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public Texture cursorPicture;
	public Texture cursorForwardCirclePicture;
	public Texture cursorForwardHalfCirclePicture;
	public Vector2 cursorPictureScale = new Vector2(25,25);
	public Vector2 cursorMaxPosition = new Vector2(50,50);
	private PlayerController playerController;
	public Vector2 cursorOrigin = new Vector2(0,12.9f);
	void renderCursor(){
		Vector2 cursorPosition = new Vector2(Screen.width/2 + cursorOrigin.x - cursorPictureScale.x/2,
			Screen.height/2 + cursorOrigin.y - cursorPictureScale.y/2);
		cursorPosition.x += cursorMaxPosition.x * playerController.cursorPosition.x;
		cursorPosition.y += -cursorMaxPosition.y * playerController.cursorPosition.y;
		Rect cursorRect = new Rect(cursorPosition.x,cursorPosition.y,
			cursorPictureScale.x,cursorPictureScale.y);
		GUIStyle cursorStyle = new GUIStyle();
		GUI.Box(cursorRect,cursorPicture,cursorStyle);
	}
	
	public Vector2 cursorHalfCirclePosition;
	
	void renderCursorForwardHalfCircle(){
		Vector2 cursorForwardHalfCirclePictureScale = cursorPictureScale;
		Vector2 cursorForwardHalfCirclePosition = new Vector2(Screen.width/2 + cursorOrigin.x - cursorForwardHalfCirclePictureScale.x/2 + cursorHalfCirclePosition.x,
			Screen.height/2 + cursorOrigin.y - cursorForwardHalfCirclePictureScale.y/2 + cursorHalfCirclePosition.y);
		Rect cursorForwardHalfCircleRect = new Rect(cursorForwardHalfCirclePosition.x,cursorForwardHalfCirclePosition.y,
			cursorForwardHalfCirclePictureScale.x,cursorForwardHalfCirclePictureScale.y);
		GUIStyle cursorForwardHalfCircleStyle = new GUIStyle();
		GUI.Box(cursorForwardHalfCircleRect,cursorForwardHalfCirclePicture,cursorForwardHalfCircleStyle);
	}

	
	void renderCursorForwardCircle(){
		Vector2 cursorForwardCirclePictureScale = cursorPictureScale;
		Vector2 cursorForwardCirclePosition = new Vector2(Screen.width/2 + cursorOrigin.x - cursorForwardCirclePictureScale.x/2,
			Screen.height/2 + cursorOrigin.y - cursorForwardCirclePictureScale.y/2);
		Rect cursorForwardCircleRect = new Rect(cursorForwardCirclePosition.x,cursorForwardCirclePosition.y,
			cursorForwardCirclePictureScale.x,cursorForwardCirclePictureScale.y);
		GUIStyle cursorForwardCircleStyle = new GUIStyle();
		GUI.Box(cursorForwardCircleRect,cursorForwardCirclePicture,cursorForwardCircleStyle);
	}
	
	public Texture shieldTexture;
	public Texture healthTexture;
	public Vector2 barPictureScale = new Vector2(60,150);
	public Vector2 barPicturePositionRelative = new Vector2(85,60);
	void renderShieldBar(float currentShield){
		Vector2 shieldTextureScale = barPictureScale;
		Vector2 shieldTexturePosition = new Vector2(Screen.width/2 - shieldTextureScale.x/2,
													Screen.height/2 - shieldTextureScale.y/2);
		shieldTexturePosition.x -= barPicturePositionRelative.x;
		shieldTexturePosition.y += barPicturePositionRelative.y;
		GUI.BeginGroup(new Rect(shieldTexturePosition.x,shieldTexturePosition.y+shieldTextureScale.y*(1-currentShield),shieldTextureScale.x,
									shieldTextureScale.y*currentShield));
		GUI.DrawTexture(new Rect(0,-shieldTextureScale.y*(1-currentShield),shieldTextureScale.x,shieldTextureScale.y),shieldTexture);
		GUI.EndGroup();
	}
	void renderHealthBar(float currentHealth){
		Vector2 healthTextureScale = barPictureScale;
		Vector2 healthTexturePosition = new Vector2(Screen.width/2 - healthTextureScale.x/2,
													Screen.height/2 - healthTextureScale.y/2);
		healthTexturePosition += barPicturePositionRelative;
		GUI.BeginGroup(new Rect(healthTexturePosition.x,healthTexturePosition.y+healthTextureScale.y*(1-currentHealth),healthTextureScale.x,
									healthTextureScale.y*currentHealth));
		GUI.DrawTexture(new Rect(0,-healthTextureScale.y*(1-currentHealth),healthTextureScale.x,healthTextureScale.y),healthTexture);
		GUI.EndGroup();
	}
	Destructible destructibleForPlayer;
	void renderShieldAndHealthBar(){
		if(!destructibleForPlayer){
			destructibleForPlayer = transform.root.gameObject.GetComponent<Destructible>();
		}
		if(destructibleForPlayer){
		renderShieldBar((float)destructibleForPlayer.curShield/destructibleForPlayer.maxShield);
		renderHealthBar((float)destructibleForPlayer.curHealth/destructibleForPlayer.maxHealth);
		}
	}
	void OnGUI(){
		renderCursor();
		renderCursorForwardHalfCircle();
		//renderCursorForwardCircle();
		renderShieldAndHealthBar();
	}
}
