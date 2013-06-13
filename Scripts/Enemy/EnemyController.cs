using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	
	public GameObject[] waypoints;
	public bool debug = false;
	private int currentWaypoint = 0;
	public float waypointRadius;
	public float maxFireDistance = 60;
	
	//roll and pitch to avoid objects
	
	public GameObject DetectorLeft;
	public GameObject DetectorLeftFar;
	public GameObject DetectorRight;
	public GameObject DetectorRightFar;
	public GameObject DetectorCenter;
	public float maxDetectorDistance = 10.0f;
	
	private RaycastHit[] obstacleRaycastList = new RaycastHit[5];
	private RaycastHit[] targetRaycastList = new RaycastHit[5];
	
	//layers that are considered targets
	public LayerMask targetLayerMask;
	//layers that are considered obstacles
	public LayerMask obstacleLayerMask ;
	
	
	
	public Vector3 target;
	// Use this for initialization
	void Start () {
		obstacleLayerMask = ~(1 << LayerMask.NameToLayer("Radar")) & ~(1 << LayerMask.NameToLayer("Boundary"));
		targetLayerMask = 1 << LayerMask.NameToLayer("Player");
		target = waypoints[currentWaypoint].transform.position;
	}
	
	void FixedUpdate(){
		//if the current waypoint no longer exists, go to next waypoint, 
		//or if no more waypoints don't move
		//added to prevent errors when a waypoint is destroyed.
		if(!waypoints[currentWaypoint]){
			if(waypoints.Length > 1)
				waypoints[currentWaypoint] = waypoints[(currentWaypoint < (waypoints.Length - 1)) ? currentWaypoint + 1:0];
			else{
				waypoints[currentWaypoint] = new GameObject("Replacement For Destroyed GameObject");
				waypoints[currentWaypoint].transform.position = transform.position;
			}
		}
			
		ObstacleList obstacleList = DetectObjectsAndAvoid();
		LookForTargetsAndFire(obstacleList);
		Move(obstacleList);
		updateTarget();
		if(debug) Debug.Log("Current Obstacle: " + obstacleList);
	}
	enum ObstacleList{
		NONE,RIGHT,CENTER,CENTER_RIGHT,
		LEFT,LEFT_RIGHT,LEFT_CENTER,LEFT_CENTER_RIGHT	
	};
	
	enum TargetList{
		NONE,RIGHT,CENTER,CENTER_RIGHT,
		LEFT,LEFT_RIGHT,LEFT_CENTER,LEFT_CENTER_RIGHT	
	};
	
	ObstacleList DetectObjectsAndAvoid(){
		ObstacleList obstacleList = locateObstacles();
		//if(obstacleList != ObstacleList.NONE)
			//if(debug) Debug.Log(obstacleList);
		switch(obstacleList){
		case ObstacleList.NONE:
			break;
		case ObstacleList.RIGHT:
			Roll(-1);
			break;
		case ObstacleList.LEFT:
		case ObstacleList.LEFT_RIGHT:
			Roll(1);
			break;
		case ObstacleList.LEFT_CENTER:
			if(!isObstacle(waypoints[currentWaypoint].tag)){
				Yaw(-1);
			}
			break;
		case ObstacleList.CENTER_RIGHT:
			if(!isObstacle(waypoints[currentWaypoint].tag)){
				Yaw(1);
			}
				break;
		case ObstacleList.CENTER:
		case ObstacleList.LEFT_CENTER_RIGHT:
			if(!isObstacle(waypoints[currentWaypoint].tag)){
				Pitch(1);
			}
			break;
		default:
			break;
		}
		return obstacleList;
	}

	private float lastDotYaw = -1;
	private float lastDotPitch = -1;
	void Move(ObstacleList obstacleList = ObstacleList.NONE){
		Vector3 directionEnemy = transform.forward;
		Vector3 directionTarget = Vector3.Normalize(waypoints[currentWaypoint].transform.position - transform.position);
		
		//rotate to face the waypoint
		float dotForward = Vector3.Dot(transform.forward,directionTarget);
		float dotYaw = Vector3.Dot(transform.right,directionTarget);
		float dotPitch = Vector3.Dot(transform.up,directionTarget);
		//if(debug) Debug.Log ("dotForward is " + dotForward + "dotYaw is " + dotYaw + "dotPitch is " + dotPitch);
		if((dotPitch > 0) ^ (lastDotPitch > 0)){
			//if(debug) Debug.Log ("Hit pitch brake");
			GetComponent<Ship>().PitchBrake();
		}
		
		if((dotYaw > 0)^(lastDotYaw > 0)){
			//if(debug) Debug.Log ("Hit yaw brake");
			GetComponent<Ship>().YawBrake();
			
		}
		//make the yaw and pitch react more if the enemy is close but not quite at correct pitch or yaw
		dotYaw = (dotYaw > 0)?Mathf.Sqrt(dotYaw):-Mathf.Sqrt(-dotYaw);
		dotPitch = (dotPitch > 0)?Mathf.Sqrt(dotPitch):-Mathf.Sqrt(-dotPitch);
		if(dotForward > 0){
			if(obstacleList != ObstacleList.CENTER && obstacleList != ObstacleList.LEFT_CENTER_RIGHT){
				Pitch(dotPitch);
			}
			if(obstacleList != ObstacleList.LEFT_CENTER && obstacleList != ObstacleList.CENTER_RIGHT){
				Yaw(dotYaw);
			}
		}else if(dotForward < 0){
			if(obstacleList != ObstacleList.CENTER && obstacleList != ObstacleList.LEFT_CENTER_RIGHT){
				Pitch((dotPitch > 0)?1:-1);
			}
			if(obstacleList != ObstacleList.LEFT_CENTER && obstacleList != ObstacleList.CENTER_RIGHT){
				Yaw((dotYaw > 0)?1:-1);
			}
		}
		RollToBoostAccuracy();
		//move towards the waypoint, but really accelerate only when facing the correct direction
		float degrBetweenEnemyTarget = Vector3.Angle(directionEnemy,directionTarget);
		if(obstacleList != ObstacleList.NONE)
			Accelerate(-Vector3.Dot(rigidbody.velocity,transform.forward));
		else
			Accelerate(1/(1+10*degrBetweenEnemyTarget/360));
		lastDotYaw = dotYaw;
		lastDotPitch = dotPitch;
	}
	
	//explanation of obstacles and targets-
	//i realise obstacles and targets sound very similar, so I'm going to explain the
	//differences, since I use both a bunch.
	//	--obstacles are objects to avoid, targets are things to shoot at.  
	//	--targets can be detected from much further away, since you don't need to move out of the
	//	way of obstacles if you aren't close to crashing.
	//	--targets are just players, since you don't need to shoot an asteroid unless you are about to hit it
	//	and need to push it out of your way.
	ObstacleList locateObstacles(){
		bool obstacleRight = Physics.Raycast(DetectorRight.transform.position,DetectorRight.transform.forward,out obstacleRaycastList[1],maxDetectorDistance, obstacleLayerMask);
		bool obstacleRightFar = Physics.Raycast(DetectorRightFar.transform.position,DetectorRightFar.transform.forward,out obstacleRaycastList[0],maxDetectorDistance, obstacleLayerMask);
		obstacleRight = obstacleRight&&obstacleRightFar;
		
		bool obstacleLeft = Physics.Raycast(DetectorLeft.transform.position,DetectorLeft.transform.forward,out obstacleRaycastList[3],maxDetectorDistance, obstacleLayerMask);
		bool obstacleLeftFar = Physics.Raycast(DetectorLeftFar.transform.position,DetectorLeftFar.transform.forward,out obstacleRaycastList[4],maxDetectorDistance, obstacleLayerMask);
		obstacleLeft = obstacleLeft && obstacleLeftFar;
		
		bool obstacleCenter = Physics.Raycast(DetectorCenter.transform.position,DetectorCenter.transform.forward,out obstacleRaycastList[2],maxDetectorDistance, obstacleLayerMask);
		//the reason I made ObstacleList was so that the options would be mutually exclusive.
		//EX. you wanna roll if there is something blocking your leftwing only if something isn't blocking center.  If something
		//is blocking your center and your leftwing, then you should yaw right.
		ObstacleList obstacleList = (ObstacleList)((obstacleRight?1:0)+(obstacleCenter?2:0)+(obstacleLeft?4:0)); 
		return obstacleList;
	}
	
	TargetList locateTargets(){
		bool targetRight = Physics.Raycast(DetectorRight.transform.position,DetectorRight.transform.forward,out targetRaycastList[1],maxFireDistance,targetLayerMask);
		bool targetRightFar = Physics.Raycast(DetectorRightFar.transform.position,DetectorRightFar.transform.forward,out targetRaycastList[0],maxFireDistance,targetLayerMask);
		targetRight = targetRight || targetRightFar;
		
		bool targetLeft = Physics.Raycast(DetectorLeft.transform.position,DetectorLeft.transform.forward,out targetRaycastList[3],maxFireDistance, targetLayerMask);
		bool targetLeftFar = Physics.Raycast(DetectorLeftFar.transform.position,DetectorLeftFar.transform.forward,out targetRaycastList[4],maxFireDistance,targetLayerMask);
		targetLeft = targetLeft || targetLeftFar;
		
		bool targetCenter = Physics.Raycast(DetectorCenter.transform.position,DetectorCenter.transform.forward,out targetRaycastList[2],maxFireDistance,targetLayerMask);
		
		TargetList targetList = (TargetList)((targetRight?1:0)+(targetCenter?2:0)+(targetLeft?4:0)); 
		return targetList;
	}
	
	
	bool isObstacle(string tag){
		foreach(RaycastHit hit in obstacleRaycastList){
			if(!Equals(hit.collider,null)){
				if(hit.collider.gameObject.tag == tag){
					return true;
				}
			}
		}
		return false;
	}
	
	bool isTarget(string tag){
		foreach(RaycastHit hit in targetRaycastList){
			if(!Equals(hit.collider,null)){
				if(hit.collider.gameObject.tag == tag){
					return true;
				}
			}
		}
		return false;
	}
	
	void LookForTargetsAndFire(ObstacleList obstacleList = ObstacleList.NONE){
		
		TargetList targetList = locateTargets();
		if(debug) Debug.Log("Current Target: " + targetList);
		foreach(FirePrimary weaponComponent in gameObject.GetComponentsInChildren<FirePrimary>()){
			RaycastHit hit;
			bool targetAtWeapon = Physics.Raycast(weaponComponent.transform.position,weaponComponent.transform.up,out hit,maxFireDistance, targetLayerMask);
			//if there is something in the way, and it isn't another enemy, fire to try to knock it out of the way or to destroy it
				//if there isn't any obstacles then fire at the player if its in 
				//front of the ship, in front of the weapon or in front of the
				//side the weapon is on
				if((targetList & TargetList.CENTER) > 0 || targetAtWeapon)
						weaponComponent.shoot();
				if(weaponComponent.name == "Left Weapon"){
					if((targetList & (TargetList.LEFT)) > 0){
						//if(debug) Debug.Log("player shot left");
						weaponComponent.shoot();
					}
				}else if(weaponComponent.name == "Right Weapon"){
					if((targetList & (TargetList.RIGHT)) > 0){
						//if(debug) Debug.Log("player shot right");
						weaponComponent.shoot();
					}
				}else{
					Debug.LogError("error, invalid name of weapon '" + weaponComponent.name +"'");
				}
				
		}
	}
	
	void RollToBoostAccuracy(){
		RaycastHit hit;
		foreach(FirePrimary weaponComponent in gameObject.GetComponentsInChildren<FirePrimary>()){
			if(Physics.Raycast(weaponComponent.transform.position,weaponComponent.transform.up,out hit,maxFireDistance, targetLayerMask)){
				//if(debug) Debug.Log("Roll Break");
				GetComponent<Ship>().RollBrake();//stop rolling if the weapon will hit the player
			}else{
				//if(debug) Debug.Log("Roll enemy");
				Roll(.2f);//roll if the weapon is not hitting the player, but it should be 
				return;
			}
		}
	}
	
	void OnDrawGizmos(){
		if(debug){
			Gizmos.color = new Color(0.4f,0.6f,1,1);
			Gizmos.DrawLine(DetectorLeft.transform.position,DetectorLeft.transform.position + 5*DetectorLeft.transform.forward);
			Gizmos.DrawLine(DetectorLeftFar.transform.position,DetectorLeftFar.transform.position + 5*DetectorLeftFar.transform.forward);
			Gizmos.DrawLine(DetectorRight.transform.position,DetectorRight.transform.position + 5*DetectorRight.transform.forward);
			Gizmos.DrawLine(DetectorRightFar.transform.position,DetectorRightFar.transform.position + 5*DetectorRightFar.transform.forward);
			Gizmos.DrawLine(DetectorCenter.transform.position,DetectorCenter.transform.position + 5*DetectorCenter.transform.forward);
		}
		
		if(waypoints != null){
			if(debug&&(waypoints.Length > 0)){
				Vector3 directionEnemy = transform.forward;
				Vector3 directionTarget = Vector3.Normalize(waypoints[currentWaypoint].transform.position - transform.position);
				Gizmos.color = new Color(0,1,1,1);
				Gizmos.DrawLine(transform.position,transform.position + 5*directionEnemy);
				Gizmos.color = new Color(1,0,0,1);
				Gizmos.DrawLine(transform.position,transform.position + 5*directionTarget);
			}
		}
	}
	
	//move the ship forward or backward
	void Accelerate(float amount){
		GetComponent<Ship>().Accelerate(amount);
	}
	
	//rotates the ship up and down
	void Pitch(float amount){
		GetComponent<Ship>().Pitch(amount);
	}
	//rotates the ship left and right
	void Yaw(float amount){
		GetComponent<Ship>().Yaw(amount);
	}
	//rolls ship
	void Roll(float amount){
		GetComponent<Ship>().Roll(amount);
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag == waypoints[currentWaypoint].tag){
			if(Vector3.Distance(waypoints[currentWaypoint].transform.position,transform.position)<4.0f){
				if(currentWaypoint< waypoints.Length-1)
					currentWaypoint++;
				else
					currentWaypoint = 0;
				if(debug) Debug.Log("Current Waypoint: " + currentWaypoint);
			}
		}
	}
	
	void DestructImpl(){
		PlayerPrefs.SetInt("Points", PlayerPrefs.GetInt("Points") + 20);
	}
	
	
	
	
	void updateTarget(){
		GameObject targetWaypoint = waypoints[currentWaypoint];
		Transform root = transform.root;
		float bulletVelocity = (root.GetComponent<Rigidbody>()).velocity.magnitude + gameObject.GetComponentInChildren<FirePrimary>().bulletVelocity;
		target = target - root.position;
		float time = target.magnitude/bulletVelocity;
		
		Rigidbody targetWaypointRigidbody = targetWaypoint.GetComponent<Rigidbody>();
		if(targetWaypointRigidbody)
			target = targetWaypointRigidbody.velocity * time + targetWaypoint.transform.position;
		else
			target = targetWaypoint.transform.position;
		gameObject.GetComponentInChildren<FirePrimary>().shootAt(target);
	}

}
