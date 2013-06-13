using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadarController: MonoBehaviour {
	public GameObject playerIcon;
	public GameObject radarIconCamera;
	public List<GameObject> iconObjects;
	public List<string> iconTags;
	private List<GameObject> objectsOnRadar = new List<GameObject>();
	private List<GameObject> iconsOnRadar = new List<GameObject>();
	public bool debug;
	// Use this for initialization
	public void Start () {
		
	}
	
	public void OnTriggerEnter(Collider otherCollider){
		GameObject triggerObject = otherCollider.collider.gameObject.transform.root.gameObject;
		if(debug) Debug.Log("entered radar:  "+triggerObject.tag);
		if(iconTags.Contains(triggerObject.tag) && !objectsOnRadar.Contains(triggerObject)){
			if(debug) Debug.Log("creating an icon:  "+triggerObject.tag);
			//add enemy to the list
			objectsOnRadar.Add(triggerObject);
			//add icon to icon list
			int tagIndex = iconTags.FindIndex(r => r.Equals(triggerObject.tag));
			GameObject objectIconInstance = Instantiate(iconObjects[tagIndex])as GameObject;
			objectIconInstance.transform.parent = radarIconCamera.transform;
			RadarIconController enemyRadarIconController = objectIconInstance.GetComponent<RadarIconController>();
			SharpUnit.Assert.NotNull(enemyRadarIconController);
			//if(debug) Debug.LogError("There's an error for finding player:"+transform.root.gameObject.name);
			enemyRadarIconController.player = transform.root.gameObject;
			enemyRadarIconController.playerRadarIcon = playerIcon;
			enemyRadarIconController.radar = gameObject;
			enemyRadarIconController.objectTracked = triggerObject;
			if(triggerObject.tag == "Enemy"){
				enemyRadarIconController.rotate = true;
			}
			if(GetComponent<SphereCollider>())
				enemyRadarIconController.radarRadius = GetComponent<SphereCollider>().radius;
			iconsOnRadar.Add(objectIconInstance);
		}
		
	}
	
	public void OnTriggerExit(Collider otherCollider){
		GameObject triggerObject = otherCollider.collider.gameObject.transform.root.gameObject;
		if(objectsOnRadar.Contains(triggerObject)){
			iconsOnRadar.RemoveAt(objectsOnRadar.IndexOf(triggerObject));
			objectsOnRadar.Remove(triggerObject);
		}
	}
		
	// Update is called once per frame
	public void Update () {
	
	}
}
