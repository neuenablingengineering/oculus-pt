using UnityEngine;
using System.Collections;


public class Obj : MonoBehaviour 
{
	ObjSpawnController master;
	public bool active = false;
	public float difficultyScore;

	void Awake()
	{
		//find the game manager in the object heirarchy
		master = GameObject.Find("Desk").GetComponent<ObjSpawnController>();
	}

	//called when collisions occur
	void OnTriggerEnter(Collider other) 
	{
		if(active && other.name.Contains("bone"))
			Collected();
	}

	public void Selected()
	{
		master.SetCurrentObject (this);
		active = true;
	}

	public void Collected()
	{
		master.ObjectCollected (difficultyScore);
		Destroy(this.gameObject);
	}

}
