using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ObjectiveManager : MonoBehaviour {
	
	public bool debug = true;
	private EnvironmentControllerObjective environmentControllerObjective;
	public bool moveObjective;
	public bool destroyObjective;
	void Start(){
		GameObject environment = GameObject.FindGameObjectWithTag("Environment");
		if(environment){
			environmentControllerObjective = environment.GetComponent<EnvironmentControllerObjective>();
		}else{
			Debug.LogError("no environment could be found");	
		}
		SharpUnit.Assert.NotNull(environmentControllerObjective,"error, did not have objective environment");
	}
	
	void OnTriggerEnter(Collider other){
		if(moveObjective){
			if(other.tag == "Player"&&environmentControllerObjective.getCurrentObjective() == gameObject){
				environmentControllerObjective.ObjectiveComplete();
			}
		}
	}
	
	void OnDestroy(){
		if(environmentControllerObjective&&destroyObjective){
			if(environmentControllerObjective.enabled){
				environmentControllerObjective.ObjectiveComplete(gameObject);
			}
		}
	}
}
