using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour {
	public float scale;
	
	// the particle system doesn't self-destruct, so this will destroy it once the explosion is finished
	
	void Start(){
		//scale determines the size of the explosion, so a bigger explosion will last longer too
		Destroy(gameObject,scale);
	}
}
