using UnityEngine;
using System.Collections;

public class BulletControllerTractorBeam : BulletController {
	public FireTractorBullet fireTractorBullet;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision collision){
		fireTractorBullet.target = collision.collider.gameObject.transform.root.gameObject;
		base.OnCollisionEnter(collision);
	}
}
