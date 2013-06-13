using UnityEngine;
using System.Collections;

public class FollowRotation : MonoBehaviour {
	public GameObject followed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(followed)
			transform.rotation = followed.transform.rotation;
	}
}
