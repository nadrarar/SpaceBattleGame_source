using UnityEngine;
using System.Collections;

public class OuterBoundary : MonoBehaviour {

	void OnTriggerExit(Collider other){
		if (other.transform.root.name == "Player" && other.transform.root.GetComponent<Destructible>() != null){
			other.transform.root.GetComponent<Destructible>().AdjustCurrentShield(-other.transform.root.GetComponent<Destructible>().maxHealth - other.transform.root.GetComponent<Destructible>().maxShield);
		}
	}
	
}
