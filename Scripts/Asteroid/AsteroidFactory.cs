using UnityEngine;
using System.Collections;

public class AsteroidFactory : MonoBehaviour {
	public GameObject[] asteroids;
	public int totalNumberOfAsteroids = 50;
	public Vector3 distanceFromFactory = new Vector3(50.0f,20.0f,20.0f);
	public Color gizmoColor;
	public float minSize = 0.5f;
	public float maxSize = 10.0f;
	void OnDrawGizmos(){
		Gizmos.color = gizmoColor;
		Gizmos.DrawWireCube(transform.position,distanceFromFactory);
	}
	
	// Use this for initialization
	void Start () {
		GameObject asteroidGroup = new GameObject();
		asteroidGroup.name	 = "Asteroid Field";
		for(float i = 0; i < totalNumberOfAsteroids; ++i){
			Vector3 distance = (new Vector3(Random.value,Random.value,Random.value) - new Vector3(0.5f,0.5f,0.5f));
			distance.x *= distanceFromFactory.x;
			distance.y *= distanceFromFactory.y;
			distance.z *= distanceFromFactory.z;
			GameObject iAsteroid = Instantiate(asteroids[Random.Range(0,asteroids.Length)],transform.position + distance,Random.rotation) as GameObject;
			float randomScale = maxSize-(maxSize-minSize)*Random.value;
			iAsteroid.transform.localScale *= randomScale;
			iAsteroid.rigidbody.mass *= randomScale;
			Destructible destructibleScript = iAsteroid.GetComponent<Destructible>();
			destructibleScript.maxHealth = (int)(destructibleScript.maxHealth*randomScale);
			destructibleScript.curHealth = (int)(destructibleScript.curHealth*randomScale);
			if(randomScale > 1)
				i += (randomScale) - 1;
			iAsteroid.transform.parent = asteroidGroup.transform;
		}
	}
}
