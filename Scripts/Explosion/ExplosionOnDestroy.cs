using UnityEngine;
using System.Collections;

public class ExplosionOnDestroy : MonoBehaviour {
	public GameObject explosion;
	public float scale;
	//use to turn off the explosion if required
	public bool explode = true;
	// Use this for initialization
	public void InstanciateExplosion(Vector3 explosionPosition){
		if(explode){
			GameObject explosionInstanciated = Instantiate(explosion,explosionPosition,transform.rotation) as GameObject;
			ParticleSystem explosionParticleSystem = explosionInstanciated.GetComponent<ParticleSystem>();
			explosionParticleSystem.startLifetime = scale;
			ExplosionController explosionInstanciatedController = explosionInstanciated.GetComponent<ExplosionController>();
			explosionInstanciatedController.scale = scale;
		}
	}
}
