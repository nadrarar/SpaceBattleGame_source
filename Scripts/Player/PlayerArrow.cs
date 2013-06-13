using UnityEngine;
using System.Collections;

public class PlayerArrow : MonoBehaviour {
	// Use this for initialization
	private EnvironmentControllerObjective environmentControllerObjective;
	void Start () {
		environmentControllerObjective = GameObject.FindGameObjectWithTag("Environment").GetComponent<EnvironmentControllerObjective>();
		if(!environmentControllerObjective){
			gameObject.SetActive(false);
		}
	}
	// Update is called once per frame
	void Update () {
		if(environmentControllerObjective){
			GameObject objective = environmentControllerObjective.getCurrentObjective();
			if(objective){
				Vector3 direction = objective.transform.position - transform.position;
				transform.rotation = Quaternion.LookRotation(direction);
			}
		}
	}
}
