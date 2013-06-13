using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stealth : MonoBehaviour {
	
	public float levelOfDetected = 0.0f;
	public List<Detecting> detectorList;
	
	public void wasDetected(Detecting m_detecting){
		if(!detectorList.Contains(m_detecting)){
			detectorList.Add(m_detecting);
		}
	}
	
	void updateDetectorList(){
		foreach(Detecting detect in detectorList){
			if(detect.detectedStealth != this){
				detectorList.Remove(detect);
			}
		}
	}
	
	float updateLevelOfDetected(){
		float m_levelOfDetected = 0;
		foreach(Detecting detecting in detectorList){
			if(detecting.levelOfDetection > m_levelOfDetected){
				m_levelOfDetected = detecting.levelOfDetection;
			}
		}
		return m_levelOfDetected;
	}
	
	// Update is called once per frame
	void Update () {
		updateDetectorList();
		updateLevelOfDetected();
	}
}
