using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public Camera radarCamera;
	public Camera backgroundCamera;
	private ParticleSystem engine;
	// Use this for initialization
	void Start () {
		//this should be in game environment or game state, but I'm leaving in here for now because the curser was distracting while debugging
		Screen.showCursor = false;
		Screen.lockCursor = true;
	}
	// Update is called once per frame
	//FixedUpdate is used for rigidbodies instead of Update, but it 
	//works in pretty much the same way
	public float acceleration = 0;
	public float deltaAccelerate = 0.05f;
	public float dampeningAccelerate = 0.05f;
	
	public Vector2 playerRotation = new Vector2(0,0);
	public Vector2 cursorPosition = new Vector2(0,0);
	
	public Vector2 cursorScale = new Vector2(50,50);
	public float mouseSensitivity = .2f;
	public float cursorRadius = .3f;
	
	public float angularDragNormal = 5.0f;
	public float angularDragTurn = .94f;
	public float minMouseSensitivity = .2f;
	void FixedUpdate () {
		//if(Input.GetKeyDown(KeyCode.Q)){
		//	GetComponent<Ship>().ToggleInertialDampening();
		//}
		mouseSensitivity = Mathf.Clamp(PlayerPrefs.GetFloat("sensitivity"),0,1);
		cursorPosition.x += Input.GetAxis("Mouse X")/cursorScale.x;
		cursorPosition.y += Input.GetAxis("Mouse Y")/cursorScale.y;
		
		if(cursorPosition.magnitude > 1){
			cursorPosition.Normalize();
		}
		
		if(cursorPosition.magnitude > cursorRadius){
			playerRotation = (cursorPosition.magnitude-cursorRadius)*cursorPosition.normalized;
		}else{
			playerRotation = new Vector2(0,0);
		}
		
		if(playerRotation.magnitude > 0){
			Yaw(playerRotation.x*mouseSensitivity);
			Pitch(playerRotation.y*mouseSensitivity);
		}
		//Debug.Log(cursorPosition.magnitude);
		
		//weapons
		//primary can be fired as long as left button is down
		if(Input.GetMouseButton(0)){
			foreach(FirePrimary fp in GetComponentsInChildren<FirePrimary>())
				fp.shoot();
		}
		
		//secondary only fires once a click because ammo is not infinite
		if(Input.GetMouseButtonDown(1)){
			foreach(FireSecondary fs in GetComponentsInChildren<FireSecondary>())
				fs.shoot();
		}
		
		//Read the keyboard input
		if(Input.GetKey(KeyCode.W)){
			Accelerate(1);
			acceleration += deltaAccelerate;
		}else if (Input.GetKey(KeyCode.S)){
			Accelerate(-1);
			acceleration -= deltaAccelerate;
		}else{
			if(acceleration > 0)
				acceleration -= dampeningAccelerate;
			else
				acceleration += dampeningAccelerate;
		}
		acceleration = Mathf.Clamp(acceleration,-1,1);
		float angularDragAmount = (playerRotation.magnitude)/(1 - cursorRadius);
		angularDragAmount = Mathf.Sqrt(angularDragAmount);
		//Debug.Log(angularDragAmount);
		float angularDragStart = Mathf.Lerp(angularDragNormal,angularDragTurn, angularDragAmount);
		//rolling
		if(Input.GetKey(KeyCode.A)){
			Roll(1);
			angularDragStart = .9f;
		}else if(Input.GetKey(KeyCode.D)){
			Roll(-1);
			angularDragStart = .9f;
		}
		rigidbody.angularDrag = angularDragStart;
		
		//Switching weapons
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
			foreach(FireSecondary fs in GetComponentsInChildren<FireSecondary>())
			{
				fs.switchWeaponUp();
			}

        }
        else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
			foreach(FireSecondary fs in GetComponentsInChildren<FireSecondary>())
				fs.switchWeaponDown();
		}
        else if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            foreach (FireSecondary fs in GetComponentsInChildren<FireSecondary>())
                if (fs.hasMissiles)
                    fs.currentWeapon = 0;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            foreach (FireSecondary fs in GetComponentsInChildren<FireSecondary>())
                if (fs.hasScatter)
                    fs.currentWeapon = 1;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            foreach (FireSecondary fs in GetComponentsInChildren<FireSecondary>())
                if (fs.hasBurst)
                    fs.currentWeapon = 2;
        } else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            foreach (FireSecondary fs in GetComponentsInChildren<FireSecondary>())
                if (fs.hasSonic)
                    fs.currentWeapon = 3;
        } 
	}
	
	//move the ship forward or backward
	void Accelerate(float amount){
		GetComponent<Ship>().Accelerate(amount);
	}
	
	//rotates the ship up and down
	void Pitch(float amount){
		GetComponent<Ship>().Pitch(amount);
	}
	//rotates the ship left and right
	void Yaw(float amount){
		GetComponent<Ship>().Yaw(amount);
	}
	//rotates the ship by spinning
	void Roll(float amount){
		GetComponent<Ship>().Roll(amount);
	}
	void DestructImpl(){
		// Game over code
		//turn off the hud, radar, and make sure Player Camera isn't destroyed at the same time player is
		//gameObject.GetComponent<PlayerController>().radarCamera.GetComponent<Camera>().enabled = false;
		//gameObject.GetComponent<PlayerController>().backgroundCamera.GetComponent<GameHUD>().enabled = false;
		//transform.FindChild("Player Camera").parent = null;
	}
				
}
