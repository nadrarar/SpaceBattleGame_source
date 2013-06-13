using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour {
	
	public float velocity = 10.0f;
	public float rotationSpeed = 90.0f;
	public Transform target;
	public float lifetime = 4f;
    public string seeks;
	private bool tracking = false;
	private float startTime;
	public GameObject missileSpawner;
	
	private Transform myTransform;
	
	void Awake() {
		myTransform = transform;
	}
	
	// Use this for initialization
	void Start() {
	    Destroy(gameObject, lifetime);
		startTime = Time.time;
		//right before its about to run out of life, turn of explosion
	}
	
	void Update() {
		if(tracking){
	        int enemyCount = GameObject.FindGameObjectsWithTag(seeks).Length;
	        GameObject[] enemies = GameObject.FindGameObjectsWithTag(seeks);
			float minDistance = float.MaxValue;
	        for (int i = enemyCount-1; i >= 0; i--){
				Transform thisEnemy = enemies[i].transform;
				float thisDistance = Vector3.Distance (thisEnemy.position, myTransform.position);
	            if ( thisDistance < minDistance){
					minDistance = thisDistance;
					target = thisEnemy.transform;
				}
	        }
			if ( target != null){
				Vector3 rotateTo = target.position - myTransform.position;
				Quaternion rotateThisMuch = Quaternion.LookRotation(rotateTo);
				myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, rotateThisMuch, rotationSpeed * Time.deltaTime);
				myTransform.rigidbody.velocity = myTransform.forward * velocity;
			}
		}
		else{
			//Checks to make sure the secondary weapon is switched to missile
			FireSecondary fs = GameObject.Find("Secondary Weapon").GetComponent("FireSecondary") as FireSecondary;
			if (Time.time - startTime > .05 && fs.missileTracking == true){
				tracking = true;
			}
		}
	}
	
	void OnCollisionEnter(Collision collision){
		//Debug.Log(collision.collider.gameObject.name);
		//Debug.Log(collision.collider.gameObject.tag);
		//remove after destructible is added 
		ExplodeBullet(collision.contacts[0].point);
		Destroy(gameObject);
		
	}
	
	//in destructable this should be implemented, not in bullet, but in destructable by searching a 
	//destructible and that destructable's children for a ExplosionOnDestroy, and then
	//running InstanciateExplosion method for each ExplosionOnDestroy
	public void ExplodeBullet(Vector3 explosionPosition){
		ExplosionOnDestroy bulletExplosion = GetComponent<ExplosionOnDestroy>();
		bulletExplosion.InstanciateExplosion(explosionPosition);
	}
	
}