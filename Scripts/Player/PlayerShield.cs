using UnityEngine;
using System.Collections;

public class PlayerShield : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision collision){
		Debug.Log("collision "+collision.gameObject.layer);
		if(collision.gameObject.layer == 1 << LayerMask.NameToLayer("Bullet")){
			Debug.Log("Destroys "+collision.gameObject);
			//GameObject.Destroy(collision.gameObject);
		}
	}
}
