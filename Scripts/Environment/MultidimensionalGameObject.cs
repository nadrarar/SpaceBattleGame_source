using UnityEngine;
using System.Collections;

//c# and Unity don't work together regarding Multidimensional Arrays
//c# arrays can be of the form mulArray[a][b] or mulArray[a,b].  However,
//this doesn't display right in unity.  This is a workaround that was
//suggested here - http://answers.unity3d.com/questions/64479/how-to-declare-a-multidimensional-array-of-strings.html

[System.Serializable]
public class MultidimensionalGameObject {
	public GameObject[] gameObjectArray = new GameObject[0];
	
	public GameObject this[int index] {
		get{
			return gameObjectArray[index];
		}
		set{
			gameObjectArray[index] = value;
		}
	}
	
	public int Length {
		get{
			return gameObjectArray.Length;
		}
	}
	
	public long LongLength{
		get{
				return gameObjectArray.LongLength;
		}
	}
	
	public IEnumerator GetEnumerator(){
		return gameObjectArray.GetEnumerator();
	}
	
}
