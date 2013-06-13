using UnityEngine;
using System.Collections;

public class AsteroidDestructImpl : MonoBehaviour {
	
	public GameObject explosionPrefab;
	public float size = 10;
	void DestructImpl(){
		//Object exp = 
		GameObject expl = Instantiate(explosionPrefab, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
		Detonator det = expl.GetComponent<Detonator>();
		if(det)
			det.size = size;
	}
	
}
