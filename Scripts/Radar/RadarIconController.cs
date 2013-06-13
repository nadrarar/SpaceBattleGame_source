using UnityEngine;
using System.Collections;

public class RadarIconController : MonoBehaviour {
	public GameObject radar;
	public GameObject player;
	public GameObject playerRadarIcon;
	public GameObject objectTracked;
	public float radarRadius = 50.0f;
	public bool rotate;
	private Vector3 initialScale;
	private float removeIconRadiusScale = 1.1f;
	// Use this for initialization
	public void Start () {
		radarRadius = radar.GetComponent<SphereCollider>().radius;
		initialScale = transform.localScale;
	}
	
	// Update is called once per frame
	public void Update () {
		if(!player){
			Destroy(gameObject);
		}else if(!objectTracked){//if the object doesn't exist, delete the icon
			Destroy(gameObject);
		}else{
			//calculate where the icon should be on radar
			Vector3 fromPlayerToObject = objectTracked.transform.position - player.transform.position;
			if(fromPlayerToObject.magnitude > removeIconRadiusScale*radarRadius){
				//Debug.Log(objectTracked.name+"'s icon got too far away");
				Destroy(gameObject);
			}
			Quaternion rotatePlayer = Quaternion.FromToRotation(Vector3.forward,fromPlayerToObject.normalized);
			Vector3 newPosition = Vector3.Normalize(Quaternion.Inverse(player.transform.rotation) * rotatePlayer * -playerRadarIcon.transform.up);
			
			Quaternion rotate90 = Quaternion.FromToRotation(Vector3.forward,Vector3.up);
			newPosition = rotate90 * newPosition;
			
			newPosition *=fromPlayerToObject.magnitude/radar.GetComponent<SphereCollider>().radius * radarRadius;
			transform.position = playerRadarIcon.transform.position + newPosition;
			if(rotate){
				//calculate icon rotation
				Quaternion objectRotation = Quaternion.Inverse(player.transform.rotation)*objectTracked.transform.rotation;
				Vector3 eulerAngle = objectRotation.eulerAngles;
				//if(player.transform.rotation.eulerAngles.y > 90 &&player.transform.rotation.eulerAngles.y < 270)
				//	eulerAngle.y *= -1;
				//if(!(player.transform.rotation.eulerAngles.z > 90 &&player.transform.rotation.eulerAngles.z < 270))
				//	eulerAngle.y *= -1;
				eulerAngle.x = 0;
				eulerAngle.z = 0;
				objectRotation.eulerAngles = eulerAngle;
				Vector3 oAxis = new Vector3();
				float oAngle = 0;
				objectRotation.ToAngleAxis(out oAngle,out oAxis);
				objectRotation = Quaternion.AngleAxis(180+oAngle,-oAxis);
				Quaternion newRotation = rotate90* Quaternion.Inverse( objectRotation);
				transform.rotation = newRotation;
			}
			//fade out the icon if the object is above/below player
			transform.localScale = (newPosition.z > 0)?((1-newPosition.z/(radarRadius))*initialScale):((1-1.2f*newPosition.z/(radarRadius))*initialScale);
		}
	}
	
}
