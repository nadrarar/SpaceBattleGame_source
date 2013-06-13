using UnityEngine;
using System.Collections;

public class Detecting : MonoBehaviour {
	
	public LayerMask layersDetected;
	public Stealth detectedStealth;
	public float levelOfDetection;
	
	public void locatedStealth(Stealth m_detectedStealth,float m_levelOfDetection){
		m_levelOfDetection = Mathf.Clamp(m_levelOfDetection,0,1);
		if(m_levelOfDetection > levelOfDetection){
			detectedStealth = m_detectedStealth;
			levelOfDetection = m_levelOfDetection;
			m_detectedStealth.wasDetected(this);
		}
	}
	
}
