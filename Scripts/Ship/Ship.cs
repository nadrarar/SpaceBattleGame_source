using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {

	//these should be in engine, but I wanted to make basic movement
	public float maxForwardVelocity = 28.0f;
	public float maxBackwardVelocity = 15.0f;
	public float maxPitchVelocity = 3.0f;
	public float maxYawVelocity = 3.0f;
	public float maxRollVelocity = 2.0f;
	//max Velocity provides an upper bound on the velocity of an object, so that a collision 
	//doesn't accelerate objects too much.  I would make it be maxForwardVelocity, but since
	//that is likely going to be in engine code eventually, I just made maxVelocity separate.
	public float maxVelocity = 50.0f;
	public float playerTurnControl = 0.05f;
	public float accelerationPerFrame = 0.7f;

	//determines speed that mouse rotates the ship
	public float yawPerFrame = 37.0f;
	public float pitchPerFrame = 37.0f;
	public float rollPerFrame = 4.8f;
	
	public float cameraPitch = 0.0f;
	public float cameraYaw = 0.0f;
	
	public bool inertialDampening = true;
	public float angularDragThis = .95f;
	public GameObject explosionPrefab;
	private ParticleSystem engine;
	private float engineTimeout;
	
	// Use this for initialization
	public void Start () {
		//Screen.lockCursor = false;
		//angularDrag slows down rotations.  It is added because if you hit something, the rigidbodies cause you to start spinning
		//and the current system makes it very difficult to stop.  To see what I mean, comment the rigidbody.angularDrag line
		//and crash into an asteroid.  Then try steering.  The effect is still there if there is drag, but it is temporary.  I think it works well
		//as an effect if you smash into an asteroid, so players are discouraged from smashing into asteroids.
		rigidbody.angularDrag = angularDragThis;
		
		//I'm not a fan of searching for a component with a particular name because names are very
		//easy to edit, but having "Engine" be a layer or tag seemed a bit excessive for 1 component.  
		//Maybe come back to this later and add a Engine tag.
		foreach(ParticleSystem parSystem in GetComponentsInChildren<ParticleSystem>()){
			if(parSystem.gameObject.name == "Engine"){
				engine = parSystem;
				engine.enableEmission = false;
			}
		}
		SharpUnit.Assert.NotNull(engine);
		engineTimeout = Time.time;
	}
	
	// stops rotation in pitch direction.  used to help enemy controller easily turn
	public void PitchBrake(){
			rigidbody.AddTorque(-Vector3.Dot(rigidbody.angularVelocity,transform.right)*transform.right,ForceMode.VelocityChange);
	}

	//stops rotation in yaw direction
	public void YawBrake(){
			rigidbody.AddTorque(-Vector3.Dot(rigidbody.angularVelocity,transform.up)*transform.up,ForceMode.VelocityChange);
	}
	
	//stops rotation in roll direction
	public void RollBrake(){
			rigidbody.AddTorque(-Vector3.Dot(rigidbody.angularVelocity,transform.forward)*transform.forward,ForceMode.VelocityChange);
	}

	// Update is called once per frame
	//FixedUpdate is used for rigidbodies instead of Update, but it 
	//works in pretty much the same way
	public void FixedUpdate () {
		if (Time.time - engineTimeout > .05)
			engine.enableEmission = false;
		//Inertial Dampening-------------------
		//ie things I added that make the controls better
		if(inertialDampening){
			//gives more control with turning by slowing down velocities that do not align with 
			//the current direction, ie it makes turning easier
			//this isn't realistic in space, but I think it makes the controls easier to use for players
			Vector3 velocityDirectionPlayer = Vector3.Dot(rigidbody.velocity, rigidbody.transform.forward)*rigidbody.transform.forward;
			Vector3 velocityNotDirectionOfPlayer = rigidbody.velocity-velocityDirectionPlayer;
			rigidbody.AddForce(-velocityNotDirectionOfPlayer*playerTurnControl,ForceMode.VelocityChange);
		}
		//secondary velocity checking.  Makes you incapable of going too fast.  Engine sort of does this, but if 
		//something collides into you, you don't want to be propelled out of the level.  Sometimes the collision system acts weird and can do this.
		//Long term, this might belong in Engine.
		if(rigidbody.velocity.magnitude > maxVelocity){
			rigidbody.velocity = maxForwardVelocity*rigidbody.velocity.normalized;
		}
		
	}
	
	//move the ship forward or backward
	public void Accelerate(float direction){
		//-1<direction<1
		if(direction > 0){
			engine.enableEmission = true;
			engineTimeout = Time.time;
		}
		direction = Mathf.Clamp(direction,-1,1);
		float amount;
		amount = direction * accelerationPerFrame;	
		
		Vector3 forwardForPlayer = transform.rotation*Vector3.forward;
		//this is devided into amount>0 and amount<=0 so that there can be separate max forward and backward velocity
		float velocityForward = Vector3.Dot(rigidbody.velocity,forwardForPlayer);
		
		// if trying to move in different direction than the current velocity, fire engines but don't check the max velocities
		if((velocityForward > 0) ^ (amount > 0)){
			rigidbody.AddForce(forwardForPlayer*amount,ForceMode.VelocityChange);
		}else{
			//trying to move in same direction as currently going, so check 
			//to make sure that adding force will not cause the player to go over maxForwardVelocity 
			if(amount > 0){
				if(velocityForward < maxForwardVelocity){
					rigidbody.AddForce(forwardForPlayer*amount,ForceMode.VelocityChange);
				}
			}else{
				if(velocityForward > -maxBackwardVelocity){
					rigidbody.AddForce(forwardForPlayer*amount,ForceMode.VelocityChange);
				}
			}
		}
	}
	public float maxCameraPitch = 8.0f;
	public float maxCameraYaw = 8.0f;
	public float cameraPitchPerFrame = 3.0f;
	public float cameraYawPerFrame = 3.0f;
	//rotates the ship up and down
	public void Pitch(float amount){
		amount = Mathf.Clamp(amount,-1,1);
		if(Mathf.Abs(cameraPitch+amount*cameraPitchPerFrame) < maxCameraPitch){
			cameraPitch += amount*cameraPitchPerFrame;
		}
		float currentPitchVelocity = Vector3.Dot(rigidbody.angularVelocity,Vector3.right); 
		if (Mathf.Abs(currentPitchVelocity) <= maxPitchVelocity){
			rigidbody.AddRelativeTorque(-amount*pitchPerFrame, 0, 0);
		}
	}
	//rotates the ship left and right
	public void Yaw(float amount){
		amount = Mathf.Clamp(amount,-1,1);
		if(Mathf.Abs(cameraYaw+amount*cameraYawPerFrame) < maxCameraYaw){
			cameraYaw += amount*cameraYawPerFrame;
		}

		float currentYawVelocity = Vector3.Dot(rigidbody.angularVelocity,Vector3.up); 
		if (Mathf.Abs(currentYawVelocity) <= maxYawVelocity){
			rigidbody.AddRelativeTorque(0, amount*yawPerFrame, 0);
		}
	}
	//rotates the ship by spinning
	public void Roll(float amount){
		amount = Mathf.Clamp(amount,-1,1);
		float currentRollVelocity = Vector3.Dot(rigidbody.angularVelocity,Vector3.forward); 
		if (Mathf.Abs(currentRollVelocity) <= maxRollVelocity){
			rigidbody.AddRelativeTorque(0, 0, amount*rollPerFrame);
		}
	}
	
	public void ToggleInertialDampening(){
		inertialDampening = !inertialDampening;
	}
	public void DestructImpl(){
		//Object exp = 
		Instantiate(explosionPrefab, gameObject.transform.position, gameObject.transform.rotation);
	}
				
}
