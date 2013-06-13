using UnityEngine;
using System.Collections;

public class ObjectiveIconController : MonoBehaviour {
	public GameObject player;
	public GameObject followingObjective;
	public GameObject radar;
	public GameObject objectiveRadarIcon;
	// Use this for initialization
	void Start () {
		SharpUnit.Assert.True(spinningFrequency != 0, "spinningFrequencey can't equal 0");
		SharpUnit.Assert.NotNull(radar);
		
		//make objective radar icon
		GameObject objectIconInstance = Instantiate(objectiveRadarIcon)as GameObject;
		ObjectiveRadarIconController objectiveRadarIconController = objectIconInstance.GetComponent<ObjectiveRadarIconController>();
		SharpUnit.Assert.NotNull(objectiveRadarIconController);
		//if(debug) Debug.LogError("There's an error for finding player:"+transform.root.gameObject.name);
		RadarController radarController = radar.GetComponent<RadarController>();
		objectIconInstance.transform.parent = radarController.radarIconCamera.transform;
		SharpUnit.Assert.NotNull(radarController);
		objectiveRadarIconController.player = radarController.transform.root.gameObject;
		objectiveRadarIconController.playerRadarIcon = radarController.playerIcon;
		objectiveRadarIconController.radar = radar;
		objectiveRadarIconController.objective = gameObject;
		objectiveRadarIconController.radarRadius = radar.GetComponent<SphereCollider>().radius;
	}
	private float currentAngle = 0;
	public float spinningFrequency = 0.2f;
	// Update is called once per frame
	void Update () {
		if(followingObjective){
			transform.position = followingObjective.transform.position;
			if(player){
				//find current angle
				currentAngle += Time.deltaTime*spinningFrequency*360.0f;
				currentAngle %= 360.0f;
				//face the icon towards the player
				Vector3 up = player.transform.position - transform.position;
				//make the icon spin
				Vector3 forward = Vector3.Cross(up,Quaternion.AngleAxis(currentAngle,Vector3.forward)*Vector3.up);
				transform.rotation = Quaternion.LookRotation(forward,up);
			}
		}
	}
}
