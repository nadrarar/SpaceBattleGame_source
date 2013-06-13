using UnityEngine;
using System.Collections;

public class EnemyControllerDefensive : MonoBehaviour {
	
	private GameObject currentTarget;
	private GameObject lockOnTarget;
	private bool lockOn = false;
	public float maxDistanceFromPatrolArea;
	public float maxLockOnTime = 10.0f;
	
	public GameObject[] waypoints;
	private int currentWaypoint;
	
	public bool debug = false;
	public float shipLength = 4.0f;
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
	enum ObstacleList{
		NONE,RIGHT,CENTER,CENTER_RIGHT,
		LEFT,LEFT_RIGHT,LEFT_CENTER,LEFT_CENTER_RIGHT	
	};
	
	enum TargetList{
		NONE,RIGHT,CENTER,CENTER_RIGHT,
		LEFT,LEFT_RIGHT,LEFT_CENTER,LEFT_CENTER_RIGHT	
	};
	
	
	//layers that are considered targets
	public LayerMask targetLayerMaskLockOn;
	private LayerMask targetLayerMask;
	//layers that are considered obstacles
	public LayerMask obstacleLayerMask ;
	
	public void OnCollisionEnter(Collision collision){
		
		GameObject collisionObject = collision.collider.gameObject.transform.root.gameObject;
		
		if(collisionObject.layer == LayerMask.NameToLayer("Bullet")){
			BulletController bulletController = collisionObject.GetComponent<BulletController>();
			MissileController missileController = collisionObject.GetComponent<MissileController>();
			if(bulletController){
				GameObject collisionObjectSpawner = bulletController.bulletSpawner.transform.root.gameObject;
				SetLockOnTarget(collisionObjectSpawner);
			}else if(missileController){
				GameObject collisionObjectSpawner = missileController.missileSpawner.transform.root.gameObject;
				SetLockOnTarget(collisionObjectSpawner);
				
			}
		}
		if(1 << collisionObject.layer == targetLayerMaskLockOn)
			SetLockOnTarget(collisionObject);
	}
	
	public Vector3 targetPosApproximate;
	// Use this for initialization
	void Start () {
		if(obstacleLayerMask.value == 0)
			obstacleLayerMask = ~((1 << LayerMask.NameToLayer("Radar")|(1 << LayerMask.NameToLayer("Objective")))) & ~(1 << LayerMask.NameToLayer("Boundary"));
		targetPosApproximate = waypoints[currentWaypoint].transform.position;
	}
	
	public GameObject GetLockOnTarget(){
		if(lockOn){
			return lockOnTarget;
		}else{
			return null;
		}
	}
	
	public void SetLockOnTarget(GameObject target){
		if(lockOnTimeBegin <= 0)
			lockOnTimeBegin = Time.time;
		lockOn = true;
		lockOnTarget = target;
		
	}
	
	private float lockOnTimeBegin = 0.0f;
	public float maxDistanceFromTarget = 50;
	bool GiveUp(){
		if(currentTarget){
			//give up on the locked on target if the enemy gets too far from the patrol area
			Vector3 relativePosition = waypoints[currentWaypoint].transform.position - transform.position;
			float distanceFromPatrolArea = relativePosition.magnitude;
			if(distanceFromPatrolArea > maxDistanceFromPatrolArea){
				return true;
			}
			//give up on target if enemy is too far away from the target.
			if(lockOnTarget){
				Vector3 relativePositionTarget = lockOnTarget.transform.position - transform.position;
				float distanceFromTarget = relativePositionTarget.magnitude;
				if(debug) Debug.Log("distance target " + distanceFromTarget);
				if(distanceFromTarget > maxDistanceFromTarget){
					return true;
				}
			}
			//give up on target if the enemy attacks for too long
			//if(debug) Debug.Log("locking on: "+(Time.time - lockOnTimeBegin));
			if(Time.time - lockOnTimeBegin > maxLockOnTime){
				return true;
			}
		}else{
			if(debug) Debug.Log("target does not exist");
			//give up on target if it doesn't exist
			return true;
		}
		return false;
	}
	
	void FixedUpdate(){
		//add radar, and make it send messages
		if(GiveUp()){
			lockOn = false;
			lockOnTimeBegin = 0;
			if(debug && lockOnTarget) Debug.Log("Giving Up on " + lockOnTarget.name);
		}
		if(lockOn){
			currentTarget = lockOnTarget;
			targetLayerMask = targetLayerMaskLockOn;
		}else{
			currentTarget = waypoints[currentWaypoint];
			targetLayerMask = 0;
		}
		//if(debug) Debug.Log("target of enemy: "+currentTarget.name);
		ObstacleList obstacleList = DetectObjectsAndAvoid();
		LookForTargetsAndFire(obstacleList);
		Move(obstacleList);
		updateTarget();
		//if(debug) Debug.Log("Current Obstacle: " + obstacleList);
	}
	
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
			if(!isObstacle(currentTarget.tag)){
				Yaw(-1);
			}
			break;
		case ObstacleList.CENTER_RIGHT:
			if(!isObstacle(currentTarget.tag)){
				Yaw(1);
			}
				break;
		case ObstacleList.CENTER:
		case ObstacleList.LEFT_CENTER_RIGHT:
			if(!isObstacle(currentTarget.tag)){
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
		Vector3 directionTarget = Vector3.Normalize(currentTarget.transform.position - transform.position);
		
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
			Accelerate(1/(1+degrBetweenEnemyTarget));
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
		//if(debug) Debug.Log("Current Target: " + targetList);
		foreach(FirePrimary weaponComponent in gameObject.GetComponentsInChildren<FirePrimary>()){
			RaycastHit hit;
			bool targetAtWeapon = Physics.Raycast(weaponComponent.transform.position,weaponComponent.transform.up,out hit,maxFireDistance, targetLayerMask);
			//if there is something in the way, and it isn't another enemy, fire to try to knock it out of the way or to destroy it
			//if(obstacleList != ObstacleList.NONE && !isTarget("Enemy")){
				//commented out because it was weird that the enemies shot at terrain
				//might add back later, but shooting doesn't seem to help much
				//weaponComponent.shoot();
			//}else{
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
				
			//}
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
	public Color gizmoWaypointColor;
	void OnDrawGizmos(){
		//draw waypoints
		if(debug){
			Gizmos.color = gizmoWaypointColor;
			for(int i = 1; i < waypoints.Length;++i){
				Gizmos.DrawLine(waypoints[i-1].transform.position,waypoints[i].transform.position);
			}
			if(waypoints[0]){
				if(waypoints[waypoints.Length - 1])	
					Gizmos.DrawLine(waypoints[waypoints.Length - 1].transform.position,waypoints[0].transform.position);
			}
		}
		
		if(debug){
			Gizmos.color = new Color(.4f,.8f,0,.2f);
			float maxDistanceRadar = GetComponentInChildren<EnemyRadarController>().GetComponent<SphereCollider>().radius;
			Gizmos.DrawSphere(transform.position,maxDistanceRadar);
		}
		
		if(debug&&currentTarget){
			Vector3 directionEnemy = transform.forward;
			Vector3 directionTarget = Vector3.Normalize(currentTarget.transform.position - transform.position);
			Gizmos.color = new Color(0,.8f,1,1);
			Gizmos.DrawLine(transform.position,transform.position + 5*directionEnemy);
			Gizmos.color = new Color(1,0,0,1);
			Gizmos.DrawLine(transform.position,transform.position + 5*directionTarget);
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
			//make sure the trigger is close enough to actually be the right waypoint, but remember that the distance from ship to trigger is ~(triggerRadius + shipLength)
			if(Vector3.Distance(waypoints[currentWaypoint].transform.position,transform.position)<waypoints[currentWaypoint].GetComponent<SphereCollider>().radius+shipLength){
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
		GameObject target = currentTarget;
		Transform root = transform.root;
		float bulletVelocity = (root.GetComponent<Rigidbody>()).velocity.magnitude + GetComponentInChildren<FirePrimary>().bulletVelocity;
		Vector3 targetRelativePosition = target.transform.position - root.position;
		float time = targetRelativePosition.magnitude/bulletVelocity;
		Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
		if(targetRigidbody)
			targetPosApproximate = targetRigidbody.velocity * time + target.transform.position;
		else
			targetPosApproximate = target.transform.position;
	}

}
