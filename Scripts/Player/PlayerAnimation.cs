using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour {
	
	public float maxPlayerPitch;
	public float maxPlayerYaw;
	public float maxPlayerForward;
	
	private float playerPitch;
	private float playerYaw;
	private float playerForward;
	
	private Vector3 startLocalPosition;
	private Quaternion startLocalRotation;
	
	private PlayerController playerController;
	// Use this for initialization
	void Start () {
		if(gameObject.tag != "Player"){
			gameObject.SetActive(false);
		}
		startLocalPosition = transform.localPosition;
		startLocalRotation = transform.localRotation;
		playerController = transform.root.GetComponent<PlayerController>();
		SharpUnit.Assert.NotNull(playerController);
	}
	
	// Update is called once per frame
	void Update () {
		playerForward = playerController.acceleration;
		playerYaw = playerController.playerRotation.x;
		playerPitch = playerController.playerRotation.y;
		transform.localPosition = startLocalPosition + maxPlayerForward*playerForward*Vector3.forward;
		transform.localRotation = startLocalRotation * Quaternion.AngleAxis(maxPlayerPitch*-playerPitch,Vector3.right)*Quaternion.AngleAxis(maxPlayerYaw*playerYaw,Vector3.forward);
		//transform.localRotation = startLocalRotation * Quaternion.AngleAxis(playerYaw,Vector3.right);
	}
}
