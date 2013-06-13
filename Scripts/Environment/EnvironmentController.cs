using UnityEngine;
using System.Collections;
public class EnvironmentController : MonoBehaviour {
	
	public int[] numberOfEnemiesInWaves;
	public GameObject[] EnemyFactories;
	public int waveNumber = 0;
	private int currentEnemyFactory = 0;
	public GameHUD gameHUD;
	public EndGameMenu endGameMenu;
	// Use this for initialization

    // Determines if the game ends through level completion (versus player death)

	void Start () {
		BeginLevel();
		BeginWave();
	}
	
	// Update is called once per frame
	void Update () {
		int numberOfEnemies =  GameObject.FindGameObjectsWithTag("Enemy").Length;
		//if all enemies destroyed, start new wave or end the level
		if(numberOfEnemies<=0){
			EndWave();
			waveNumber++;
			//I implemented it this way so that there is a level 0, just in case we decide to implement
			//some kind of array of waves.  This might have a bunch of waypoints in each wave, or something
			if(waveNumber >= numberOfEnemiesInWaves.Length){
				endGameMenu.levelComplete = true;
				gameHUD.levelComplete = true;
				EndLevel();
				PlayerPrefs.Save ();
			}else{
				BeginWave();
			}
		}
		if(!GameObject.FindGameObjectWithTag("Player")){
			EndLevel();
		}
	}
	
	void BeginWave(){
		Debug.Log("Wave " + waveNumber + " Begin");
		for(int i = 0; i < numberOfEnemiesInWaves[waveNumber]; ++i){
			EnemyFactory factory = EnemyFactories[currentEnemyFactory].GetComponent<EnemyFactory>();
			factory.SpawnNewEnemy();
			currentEnemyFactory++;
			if(currentEnemyFactory >= EnemyFactories.Length)
				currentEnemyFactory = 0;
		}
		if(gameHUD)
			gameHUD.StartWaveMessage(waveNumber,"Kill All Enemies");
	}
	
	void EndWave(){
		Debug.Log("Wave " + waveNumber + " End");
        gameHUD.displayWave = false;
	}
	
	void BeginLevel(){
		Debug.Log("Level Begin");
	}


	
	void EndLevel(){
	    Screen.lockCursor = false;
		Screen.showCursor = true;
	}
	
}
