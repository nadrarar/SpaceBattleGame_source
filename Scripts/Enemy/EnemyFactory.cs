using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyFactory : MonoBehaviour {
	public GameObject enemy;
	
	public List<GameObject> waypoints;
	public Color gizmoColor = new Color(0,1,0,1);
	public Color gizmoWaypointColor;
	public float waypointRadius = 1.0f;
	public float maxDistanceFromPatrolArea = 160;
	public float maxLockOnTime = 30;
	public float maxDistanceFromTarget = 120;
	
	// Use this for initialization
	void Start () {
		//SpawnNewEnemy();
	}
	
	
	public void SpawnNewEnemy(){
		GameObject m_enemy = Instantiate(enemy,transform.position,transform.rotation) as GameObject;
		EnemyController enemyController = m_enemy.GetComponent<EnemyController>();
		EnemyControllerDefensive enemyControllerDefensive = m_enemy.GetComponent<EnemyControllerDefensive>();
		if(enemyController){
			Debug.Log("follow enemies");
			enemyController.waypoints = waypoints.ToArray();
			enemyController.waypointRadius = waypointRadius;
		}else if(enemyControllerDefensive){
		if(enemyControllerDefensive)
			Debug.Log("defensive enemies");
			enemyControllerDefensive.waypoints = waypoints.ToArray();
			enemyControllerDefensive.maxDistanceFromPatrolArea = maxDistanceFromPatrolArea;
			enemyControllerDefensive.maxLockOnTime = maxLockOnTime;
			enemyControllerDefensive.maxDistanceFromTarget = maxDistanceFromTarget;
			
		}else{
			Debug.LogError("error, no enemy controller");
		}
	}
	
	void OnDrawGizmos(){
		//draw the enemyFactory
		Gizmos.color = gizmoColor;
		Gizmos.DrawLine(transform.position+Vector3.down,transform.position+Vector3.up);
		Gizmos.DrawLine(transform.position+Vector3.left,transform.position+Vector3.right);
		Gizmos.DrawLine(transform.position+Vector3.forward,transform.position+Vector3.back);
		//draw line showing path of the enemy
		Gizmos.color = gizmoWaypointColor;
		for(int i = 1; i < waypoints.Count;++i){
			Gizmos.DrawLine(waypoints[i-1].transform.position,waypoints[i].transform.position);
		}
		if(waypoints[0]){
			if(waypoints[waypoints.Count - 1])	
				Gizmos.DrawLine(waypoints[waypoints.Count - 1].transform.position,waypoints[0].transform.position);
		}
	}
}
