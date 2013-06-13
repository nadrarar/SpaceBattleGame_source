using UnityEngine;
using System.Collections;

public class TurretCannon : MonoBehaviour {
	
	private GameObject player;
	public bool debug;
	public float maxBarrelLength;
	public float maxRaycastDistance = 20;
	public LayerMask detectingLayer;
	public WeaponTypes weaponType = WeaponTypes.LASER;
	public enum WeaponTypes{
		LASER,TRACTOR_BEAM
	};
	
	private FirePrimary firePrimary;
	private FireTractorBullet fireTractorBullet;
	// Use this for initialization
	void Start () {
		firePrimary = GetComponent<FirePrimary>();
		fireTractorBullet = GetComponent<FireTractorBullet>();
		CapsuleCollider barrelCapsule = GetComponentInChildren<CapsuleCollider>();
		maxBarrelLength = (barrelCapsule.height+barrelCapsule.radius)*barrelCapsule.gameObject.transform.lossyScale.y;
		player = GameObject.FindGameObjectWithTag("Player").transform.root.gameObject;
	}
	// Update is called once per frame
	private Ray turretRay;
	
	Vector3 getPlayerDirection(Vector3 latestPlayerPosition){
		Vector3 playerDirection = latestPlayerPosition - transform.position;
		playerDirection.Normalize();
		if(Vector3.Dot(playerDirection,transform.root.up)<0){
			playerDirection -= Vector3.Dot(playerDirection,transform.root.up)*transform.root.up;
			playerDirection.Normalize();
		}
		return playerDirection;
	}
	Vector3 gizmo_playerDirection;
	Vector3 gizmo_playerPosition;
	void aimTurret(){
		Vector3 playerDirection = getPlayerDirection(player.transform.position);
		Vector3 playerPositionRelative = player.transform.position - transform.FindChild("ProjectileSpawner").position;
		float distancePlayerTurret =  playerPositionRelative.magnitude;
		float timeToHitPlayer = distancePlayerTurret/firePrimary.bulletVelocity;
		if(player.rigidbody){
			Vector3 newPlayerPosition = player.transform.position;
			newPlayerPosition += player.rigidbody.velocity*timeToHitPlayer;
			playerDirection = getPlayerDirection(newPlayerPosition);
			gizmo_playerPosition = newPlayerPosition;
			gizmo_playerDirection = playerDirection;
			//Debug.Log("player position "+newPlayerPosition);
			//Debug.Log("actual player position "+player.transform.position);
		}
		Quaternion turretRotation = transform.rotation;
		turretRotation.SetLookRotation(playerDirection);
		transform.rotation = turretRotation;
	}
	void fireTurret(){
		RaycastHit raycastHit;
		Vector3 playerDirection = getPlayerDirection(player.transform.position);
		turretRay = new Ray(transform.position+playerDirection*maxBarrelLength,playerDirection*maxBarrelLength*2);
		bool rayHit = Physics.Raycast(turretRay,out raycastHit,maxRaycastDistance*transform.lossyScale.y,detectingLayer);
		if(rayHit){
			if(raycastHit.collider.transform.CompareTag(player.tag)){
				if(debug) Debug.Log("shooting "+weaponType.ToString()+" at: "+player.tag);
				switch(weaponType){
				case WeaponTypes.TRACTOR_BEAM:
					fireTractorBullet.shoot();
					break;
				case WeaponTypes.LASER:
					firePrimary.shoot();
					break;
				}
			}
		}else {
			fireTractorBullet.target = null;
		}
		
	}
	
	void Update () {
		if(player.activeSelf){
			aimTurret();
			fireTurret();
		}
	}
	void OnDrawGizmos(){
		if(debug){
			Color color = new Color(1,1,0,.3f);
			Gizmos.color = color;
			Gizmos.DrawRay(turretRay);
			Gizmos.DrawLine(transform.position,transform.position + gizmo_playerDirection*20);
			Gizmos.DrawCube(gizmo_playerPosition,new Vector3(2,2,2));
			Gizmos.DrawSphere(transform.position,maxRaycastDistance*transform.lossyScale.y);
		}
	}
}
