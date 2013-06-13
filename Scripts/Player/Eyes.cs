using UnityEngine;
using System.Collections;

public class Eyes : MonoBehaviour {
	public Detecting detecting;
	// Use this for initialization
	void Start () {
		detecting = gameObject.GetComponent<Detecting>();
		SharpUnit.Assert.NotNull(detecting);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
