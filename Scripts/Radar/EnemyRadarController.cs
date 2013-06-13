using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyRadarController: MonoBehaviour {
	public EnemyControllerDefensive enemyControllerDefensive;
	// Use this for initialization
	public void Start () {
		enemyControllerDefensive = transform.root.GetComponent<EnemyControllerDefensive>();
		SharpUnit.Assert.NotNull(enemyControllerDefensive);
		SharpUnit.Assert.NotNull(enemyControllerDefensive.targetLayerMaskLockOn);
	}
	
	public void OnTriggerEnter(Collider otherCollider){
		GameObject triggerObject = otherCollider.collider.gameObject.transform.root.gameObject;
		//Debug.Log("Enemy Radar detects: "+triggerObject.name);
		//Debug.Log("Enemy Radar compares triggerObject: "+(1 << triggerObject.layer));
		//Debug.Log(enemyControllerDefensive.targetLayerMaskLockOn.value);
		if(enemyControllerDefensive){
			if(1 << triggerObject.layer == enemyControllerDefensive.targetLayerMaskLockOn){
				Debug.Log("Enemy locks onto: "+triggerObject.name);
				enemyControllerDefensive.SetLockOnTarget(triggerObject);
			}
		}
	}
	
	/*
	public void OnTriggerExit(Collider otherCollider){
		GameObject triggerObject = otherCollider.collider.gameObject.transform.root.gameObject;
		transform.root.GetComponent<EnemyControllerDefensive>().SetLockOnTarget(null);
	}
	*/
		
	// Update is called once per frame
	public void Update () {
		
	}
}
