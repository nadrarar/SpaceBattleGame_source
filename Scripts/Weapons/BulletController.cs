using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {
	public float lifetime = 4;
	public GameObject bulletSpawner;
	//the weapon that fired the bullet
	// Use this for initialization
	void Start() {
	    Destroy(gameObject, lifetime);
		//right before its about to run out of life, turn of explosion
	}
	
	
	public void OnCollisionEnter(Collision collision){
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
