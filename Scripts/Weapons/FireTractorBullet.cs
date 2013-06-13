using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FireTractorBullet : MonoBehaviour {
	
	public GameObject BulletPrefab;
	public float bulletVelocity = 80;
	public float bulletReloadTime = 0.2f;
	public GameObject shipRoot;
	private float lastShootTime = 0;
	private bool p_targetInertialDampening;
	private GameObject p_target;
	//inertial dampening interferes with the tractor beam, so if it is turned on, the tractor beam will turn it off
	//until the beam is destroyed 
	public GameObject target {
		get {
			return p_target;
		}
		set {
			if(p_target){
				Ship targetShip = p_target.GetComponent<Ship>();
				if(targetShip){
					targetShip.inertialDampening = p_targetInertialDampening;
				}
			}
			if(value){
				Ship valueShip = value.GetComponent<Ship>();
				if(valueShip){
					p_targetInertialDampening = valueShip.inertialDampening;
					valueShip.inertialDampening = false;
				}
			}
			p_target = value;
			currentResistance = 0;
		}
	}
	
	public void Update(){
		
	}
	public float strength = 1.0f;
	public float maxResistance = 200.0f;
	public float currentResistance = 0.0f;
	public float resistanceDecay = 2.0f;
	public bool shoot () {
		
		if(target){
			if(target.rigidbody){
				Vector3 directionFromTargetToWeapon = transform.position - target.transform.position;
				directionFromTargetToWeapon.Normalize();
				Debug.Log("Direction to Weapon: "+directionFromTargetToWeapon);
				float velocityToWeaponSize = Vector3.Dot(directionFromTargetToWeapon,target.rigidbody.velocity);
				Vector3 velocityToWeapon = directionFromTargetToWeapon * velocityToWeaponSize;
				Debug.Log("velocity in direction to weapon: "+velocityToWeapon);
				if(velocityToWeaponSize < 0){
					target.rigidbody.AddForce(-strength * velocityToWeapon,ForceMode.VelocityChange);
					currentResistance += -velocityToWeaponSize;
				}
			}
			currentResistance += resistanceDecay;
			if(currentResistance > maxResistance){
				target = null;
				currentResistance = 0;
			}
		}else if( Time.time > lastShootTime + bulletReloadTime){
			Transform ship = transform.root;
			Transform spawn = transform.FindChild("ProjectileSpawner");
			GameObject bullet = Instantiate(BulletPrefab, spawn.position, spawn.rotation) as GameObject;
			bullet.rigidbody.velocity = (ship.GetComponent<Rigidbody>()).velocity;
			Vector3 shipForward = ship.forward;
			if(shipRoot) shipForward = shipRoot.transform.forward;
			bullet.rigidbody.AddForce(shipForward*bulletVelocity,ForceMode.VelocityChange);
			BulletControllerTractorBeam m_bulletControllerTractorBeam = bullet.GetComponent<BulletControllerTractorBeam>();
			m_bulletControllerTractorBeam.bulletSpawner = transform.root.gameObject;
			m_bulletControllerTractorBeam.fireTractorBullet = this;
			
			
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
}
