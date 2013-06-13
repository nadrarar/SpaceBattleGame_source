	using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvironmentControllerObjective : MonoBehaviour {
	
	//objectives are similar to waypoints, but
	//are places the player should go.  A level
	//made with objectives makes player go from
	//Objective 0 to 1 to 2 to 3 to...N, while spawning enemies
	//to attack the player on the way.  However, a player
	//can win even if they don't destroy the enemies, by getting
	//to last objective.
	public List<GameObject> objectives = new List<GameObject>();
	public List<string> objectiveMessages = new List<string>();
	public GameObject objectiveIcon;
	public GameObject objectiveRadarIcon;
	
	public float instructionTimeout = 7.0f;
	
	private int currentObjective = 0;
	
	public GameObject player;
	public GameHUD gameHUD;
	public EndGameMenu endGameMenu;
	
	
	public GameObject getCurrentObjective(){
		return objectives[currentObjective];
	}
	
	//enemyFactories[objective #] [factory #]. Note that although
	//there should be an array of factories for every objective,
	//that array could be empty
	public MultidimensionalGameObject[] enemyFactories;
	
	//how long it takes before the level ends if player dies
	public float deathTime = 4.0f;
	
	public bool debug = true;
	private Vector3 objectiveIconScale;
	//draw Gizmos with these colors
	public Color objectiveColor = new Color(0,.8f,.1f,1);
	public Color objectivePathColor = new Color(.6f,.9f,.8f,1);
	//color of the last objective
	public Color objectiveColorFinal = new Color(.8f,.3f,.2f,1);
	//color of the current objective
	public Color objectiveColorCurrent = new Color(1.0f,.6f,.3f,.7f);
	
	// Use this for initialization
	void Start () {
		SharpUnit.Assert.Equal(objectives.Count,objectiveMessages.Count,
			"# of objective messages has to be same as # of objectives");
		SharpUnit.Assert.Equal(objectives.Count,enemyFactories.Length, 
			"# of objectives should equal # of enemy factory arrays");
		SharpUnit.Assert.True(objectives.Count > 0, "no objectives to meet");
		
		//initialize
		if(!player){
			Debug.LogError("Player not defined");
			GameObject.Find("Player");
		}
		objectiveIcon = Instantiate(objectiveIcon) as GameObject;
		objectiveIcon.GetComponent<ObjectiveIconController>().player = player;
		objectiveIcon.GetComponent<ObjectiveIconController>().radar = player.GetComponentInChildren<RadarController>().gameObject;
		objectiveIcon.GetComponent<ObjectiveIconController>().objectiveRadarIcon = objectiveRadarIcon;
		objectiveIconScale = objectiveIcon.transform.localScale;
		//start the level
		BeginLevel();
		BeginObjective();
	}
	
	// Update is called once per frame
	void Update () {
		//if player dies, end the level
		if(!GameObject.FindGameObjectWithTag("Player")){
			Invoke("EndLevel",deathTime);
		}
	}
	
	public void ObjectiveComplete(GameObject objectiveThatIsComplete){
		int index = objectives.FindIndex(o => o.Equals(objectiveThatIsComplete));
		if(index >= 0){
			if(currentObjective == index){
				ObjectiveComplete();
			}else if(currentObjective < index){
				//if player destroys an object before they are supposed 
				//to, then remove objective
				objectives.RemoveAt(index);
				//objectives.Insert(index,new GameObject("Objective "+index+" Impersonation"));
			}
		}else{
			Debug.LogError(index+"error, objective is not found");
		}
	}
	
	public void ObjectiveComplete(){
		EndObjective();
		if(currentObjective < objectives.Count - 1){
			currentObjective++;
			BeginObjective();
		}else{
			endGameMenu.levelComplete = true;
			gameHUD.levelComplete = true;
			EndLevel();
		}
	}
	
	
	void BeginObjective(){
		Debug.Log("Objective " + currentObjective + " Begin");
		objectiveIcon.GetComponent<ObjectiveIconController>().followingObjective = objectives[currentObjective];
		//changing the parent won't change the scale, but since children will inherit
		//the scale of parents, resetting the scale will make the icon scale correctly
		//based on parent size
		objectiveIcon.transform.localScale =  Vector3.Scale(objectiveIconScale,objectives[currentObjective].transform.localScale);
		foreach(GameObject factory in enemyFactories[currentObjective]){
			factory.GetComponent<EnemyFactory>().SpawnNewEnemy();
		}
		gameHUD.StartObjectiveMessage(currentObjective,objectiveMessages[currentObjective],instructionTimeout);
	}
	
	void EndObjective(){
		Debug.Log("Objective " + currentObjective + " End");
		gameHUD.displayObjective = false;
	}
	
	void BeginLevel(){
		Debug.Log("Level Begin");
		
	}

	void EndLevel(){
		Debug.Log("Level End");
	    Screen.lockCursor = false;
		Screen.showCursor = true;
		//Application.LoadLevel ("mainMenu");
		
	}
	
	void OnDrawGizmos(){
		if(!debug)
			return;
		
		if(objectives.Count == 0){
			return;
		}else{
			//draw the first objective
			if(!objectives[0]) return;
			Gizmos.color = objectiveColor;
			Gizmos.DrawLine(objectives[0].transform.position+Vector3.down,objectives[0].transform.position+Vector3.up);
			Gizmos.DrawLine(objectives[0].transform.position+Vector3.left,objectives[0].transform.position+Vector3.right);
			Gizmos.DrawLine(objectives[0].transform.position+Vector3.forward,objectives[0].transform.position+Vector3.back);
			if(objectives.Count > 1){
				for(int i = 1; i < objectives.Count;++i){
					if(!objectives[i]) return;
					//draw the objective
					if(i == objectives.Count-1)
						Gizmos.color = objectiveColorFinal;
					else
						Gizmos.color = objectiveColor;
					Gizmos.DrawLine(objectives[i].transform.position+Vector3.down,objectives[i].transform.position+Vector3.up);
					Gizmos.DrawLine(objectives[i].transform.position+Vector3.left,objectives[i].transform.position+Vector3.right);
					Gizmos.DrawLine(objectives[i].transform.position+Vector3.forward,objectives[i].transform.position+Vector3.back);
					//draw line showing path to each objective
					Gizmos.color = objectivePathColor;
					Gizmos.DrawLine(objectives[i-1].transform.position,objectives[i].transform.position);
					
				}
				
			}
			//draw the current objective
			Gizmos.color = objectiveColorCurrent;
			Gizmos.DrawSphere(objectives[currentObjective].transform.position,1.0f);
		}
	}
	
}
