using UnityEngine;
using System.Collections;

public class InnerBoundary : MonoBehaviour {
	
	public bool warn;
	
	void OnTriggerExit(Collider other){
		if (other.transform.root.name == "Player"){
			warn = true;
		}
	}
	
	void OnTriggerEnter(Collider other){
		if (other.transform.root.name == "Player"){
			warn = false;
		}
	}
	
	void OnGUI(){
		if (warn){
			GUI.Label(new Rect(Screen.width / 2 - 100, 30, 200, 200), "Return to battle or die as a traitor!");
		}
	}
}
