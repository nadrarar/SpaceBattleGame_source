using UnityEngine;
using System.Collections;

public class ObjectiveRadarIconController : MonoBehaviour {
	public GameObject radar;
	public GameObject player;
	public GameObject playerRadarIcon;
	public GameObject objective;
	public float radarRadius = 50.0f;
	private Vector3 initialScale;
	public float removeIconRadiusScale = 1.2f;
	// Use this for initialization
	public void Start () {
		radarRadius = removeIconRadiusScale*radar.GetComponent<SphereCollider>().radius;
		initialScale = transform.localScale;
	}
	
	// Update is called once per frame
	public void Update () {
		if(!player){
			Destroy(gameObject);
		}else if(!objective){//if the object doesn't exist, delete the icon
			Destroy(gameObject);
		}else{
			//calculate where the icon should be on radar
			Vector3 fromPlayerToObject = objective.transform.position - player.transform.position;
			Quaternion rotatePlayer = Quaternion.FromToRotation(Vector3.forward,fromPlayerToObject.normalized);
			Vector3 newPosition = Vector3.Normalize(Quaternion.Inverse(player.transform.rotation) * rotatePlayer * -playerRadarIcon.transform.up);
			newPosition.Normalize();
			//Debug.Log("new position of icon: "+newPosition);
			Quaternion rotate90 = Quaternion.FromToRotation(Vector3.forward,Vector3.up);
			newPosition = rotate90 * newPosition;
			
			newPosition *=radarRadius;
			
			transform.position = playerRadarIcon.transform.position + newPosition;
			if(fromPlayerToObject.magnitude > radarRadius)
				gameObject.GetComponentInChildren<Renderer>().enabled = true;
			else
				gameObject.GetComponentInChildren<Renderer>().enabled = false;
			//fade out the icon if the object is above/below player
			float newScale = newPosition.z/radarRadius;
			//Debug.Log("scale of icon: "+newScale);
			transform.localScale = (newScale > 0)?((1-.5f*newScale)*initialScale):((1-newScale)*initialScale);
		}
	}
	
}
