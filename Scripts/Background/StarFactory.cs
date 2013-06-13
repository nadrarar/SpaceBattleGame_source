using UnityEngine;
using System.Collections;

public class StarFactory: MonoBehaviour {
	public GameObject star;
	public int totalNumberOfStars = 200;
	public float distanceFromFactory = 40.0f;
	public float varietyOfSize = 0.1f;
	//public float varietyOfColor = 0.9f;
	// Use this for initialization
	void Start () {
		GameObject field = new GameObject("Star Field");
		
		for(int i = 0; i < totalNumberOfStars; ++i){
			GameObject iStar = Instantiate(star,transform.position + distanceFromFactory*Random.onUnitSphere,Quaternion.identity) as GameObject;
			//can change the color of stars.  If this is done, don't use texture for lighting
			//Color iStarColor = iStar.renderer.material.color - varietyOfColor * new Color(Random.value,Random.value,Random.value);
			//iStar.renderer.material.color = iStarColor;
			iStar.transform.localScale *= 1-varietyOfSize*Random.value;
			iStar.transform.parent = field.transform;
		}
	}
	void OnDrawGizmos(){
		Gizmos.DrawWireSphere(transform.position,distanceFromFactory);
	}
}
