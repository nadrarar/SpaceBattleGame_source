using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour {
	
	public int maxHealth = 300;
	public int curHealth = 300;
	
	public int damage = 1;
	
	public int maxShield = 100;
	public int curShield = 100;
	
	public float shieldTimer = 0;
	public float rechargeReady = 3.0f;
	
	public bool destructibleDealsVelocityDamage = true;
	public float minVelocityToCauseDamage = 10.0f;
	public float velocityDamageMagnitude = 2.0f;
	public bool invincible = false;
	
	// Use this for initialization
	void Start () {
		//Check if we can recharge shield every second
		InvokeRepeating("RegenerateShield", 0, 0.2f);
	}
	
	// Update is called once per frame
	void Update () {
		//FOR TESTING PURPOSES
		if(Input.GetKeyUp(KeyCode.F)) {
			//test for player only
			if (gameObject.GetComponent<PlayerController>() != null){
				AdjustCurrentShield(-6);
			}
		}
		// END TEST CODE
		
		if(shieldTimer > 0) {
			shieldTimer -= Time.deltaTime;
		}
		
		if(shieldTimer < 0) {
			shieldTimer = 0;
		}
		
		
	}
	
	public void AdjustCurrentHealth(int adj) {
		curHealth += adj;
		if(curHealth < 1) {
			curHealth = 0;
			Destruct();
		}
		if(curHealth > maxHealth) {
			curHealth = maxHealth;
		}
		if(maxHealth < 1) {
			maxHealth = 1;
		}
	}
	
	private void RegenerateShield() {
		if(shieldTimer == 0) {
			AdjustCurrentShield(3);
		}
	}
	
	public void AdjustCurrentShield(int adj) {
		if(adj < 0) {
			shieldTimer = rechargeReady;
		}
		curShield += adj;
		if(curShield < 1) {
			int healthAdj = curShield;
			curShield = 0;
			if (healthAdj < 0) {
				AdjustCurrentHealth(healthAdj);
			}
		} 
		if(curShield > maxShield) {
			curShield = maxShield;
		}
		if(maxShield < 1) {
			maxShield = 1;
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		Destructible otherDestructible = collision.gameObject.GetComponent<Destructible>();
		if (otherDestructible != null){
			if(!otherDestructible.invincible){
				if(destructibleDealsVelocityDamage){
					Vector3 collisionVelocity = new Vector3(0,0,0);
					if(transform.root.gameObject.rigidbody)
						collisionVelocity += transform.root.gameObject.rigidbody.velocity;
					Rigidbody rigidbodyCollider = collision.collider.transform.root.gameObject.rigidbody;
					if(rigidbodyCollider){
						collisionVelocity -= rigidbodyCollider.velocity;
					}
					if(collisionVelocity.magnitude > minVelocityToCauseDamage){
						//Debug.Log("Dealing damage due to velocity:"+Mathf.RoundToInt(-collisionVelocity.magnitude*velocityDamageMagnitude));
						otherDestructible.AdjustCurrentShield(Mathf.RoundToInt(-collisionVelocity.magnitude*velocityDamageMagnitude));
					}
				}
				//Debug.Log("Dealing damage due to destructible:"+-damage);
				otherDestructible.AdjustCurrentShield(-damage);
			}
		}
	}
	
	void Destruct(){
		gameObject.SendMessage("DestructImpl",SendMessageOptions.DontRequireReceiver);
		//this should be in destruct impl, but destruct impl isn't working for the player.  the message
		//doesn't seem to reach PlayerController before Player is destroyed
		if(gameObject.tag == "Player"){
			//turn off the hud and make sure the camera isn't destroyed too
			gameObject.GetComponent<PlayerController>().radarCamera.GetComponent<Camera>().enabled = false;
			gameObject.GetComponent<PlayerController>().backgroundCamera.GetComponent<GameHUD>().enabled = false;
			transform.root.FindChild("Player Camera").parent = null;
		}
		Destroy (gameObject);
	}
}
