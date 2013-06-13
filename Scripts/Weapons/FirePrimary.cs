using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FirePrimary : MonoBehaviour {
	
	public GameObject BulletPrefab;
	public float bulletVelocity = 80;
	public float bulletReloadTime = 0.2f;
	public GameObject shipRoot;
	private float lastShootTime = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if(Input.GetMouseButtonDown(0)== true){
			var bullet = (GameObject) Instantiate (BulletPrefab, transform.position, transform.rotation);
			bullet.rigidbody.velocity = (transform.root.GetComponent<Rigidbody>()).velocity;
			bullet.rigidbody.AddForce(transform.root.forward*force);
		}*/
	}
	public bool shoot () {
		
		if( Time.time > lastShootTime + bulletReloadTime ){
			Transform ship = transform.root;
			Transform spawn = transform.FindChild("ProjectileSpawner");
			GameObject bullet = Instantiate(BulletPrefab, spawn.position, spawn.rotation) as GameObject;
			bullet.rigidbody.velocity = (ship.GetComponent<Rigidbody>()).velocity;
			Vector3 shipForward = ship.forward;
			if(shipRoot) shipForward = shipRoot.transform.forward;
			bullet.rigidbody.AddForce(shipForward*bulletVelocity,ForceMode.VelocityChange);
			bullet.GetComponent<BulletController>().bulletSpawner = transform.root.gameObject;
			//ignore collisions between the bullet and the ship and weapon, so that the 
			//ship doesn't hit itself
			if(collider)
				Physics.IgnoreCollision(bullet.collider,collider);
			if(ship.GetComponent<Collider>())
				Physics.IgnoreCollision(bullet.collider,ship.collider);
			//this loop is added because ships could have multiple colliders, and those are stored
			//in the children of the ship
			foreach(Collider childCollider in ship.GetComponentsInChildren<Collider>()){
				Physics.IgnoreCollision(bullet.collider,childCollider.collider);
			}
			lastShootTime = Time.time;
			
			audio.Play();
			return true;
		}
		return false;
	}
	public void shootAt (Vector3 target) {
		/*
		if( Time.time > lastShootTime + bulletReloadTime ){
			Transform ship = transform.root;
			GameObject bullet = Instantiate(BulletPrefab, transform.position, transform.rotation) as GameObject;
			bullet.rigidbody.velocity = (ship.GetComponent<Rigidbody>()).velocity;
			bullet.rigidbody.AddForce(((target - ship.position).normalized)*bulletVelocity,ForceMode.VelocityChange);
			bullet.GetComponent<BulletController>().weapon = gameObject;
			//ignore collisions between the bullet and the ship and weapon, so that the 
			//ship doesn't hit itself
			
			Physics.IgnoreCollision(bullet.collider,collider);
			if(ship.GetComponent<Collider>())
				Physics.IgnoreCollision(bullet.collider,ship.collider);
			//this loop is added because ships could have multiple colliders, and those are stored
			//in the children of the ship
			foreach(Collider childCollider in ship.GetComponentsInChildren<Collider>()){
				Physics.IgnoreCollision(bullet.collider,childCollider.collider);
			}
			lastShootTime = Time.time;
			
			audio.Play();
			
		}
		*/
	}
}
