using UnityEngine;
using System.Collections;

public class ObjSpawn : MonoBehaviour 
{
	public float difficultyValue;
	public GameObject obj;

	public void Spawn(GameObject spawnMe)
	{		
		//create new object

		obj = Instantiate(spawnMe, transform.position, Quaternion.identity) as GameObject;
		obj.transform.SetParent(this.gameObject.GetComponent<Transform>());

		//set the proper scaling after parenting to the canvas
		obj.GetComponent<Transform>().localScale = obj.GetComponent<Transform>().localScale/4;//new Vector3(0.75f, 0.75f, 0.75f);// transform.localScale;
		obj.AddComponent<Obj> ().difficultyScore = difficultyValue;
	}

	public void SelectMe()
	{
		obj.GetComponent<Obj> ().Selected ();
	}
}
