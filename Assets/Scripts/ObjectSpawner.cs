using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour 
{
	//hand the spawner a copy of the ball prefab
	public GameObject objPrefab;
	//have a reference to the canvas
	//public Transform Parent;
	//which direction should this spawner send its spawnees
	//disabled for the moment, left in in case we need it
	//public Vector2 direction;

	float t = 0;						//keep track of time
	public float NextSpawn = 1.0f;		//how often should a cannon be fired
	public Camera playerCam;
	public GameObject glassWall;

	// Update is called once per frame
	void Update () 
	{
		//if the game is active and it is time to fire off a shot, fire away
		if (NextSpawn < (t += Time.deltaTime)) 
		{
			//select a random cannon to fire
			SpawnInArea();
			//reset the timer to zero
			t = 0;
		}
	}

	public void SpawnInArea()
	{		
		//get the location of the glass wall
		Vector3 location = glassWall.GetComponent<Transform>().position;

		//divide by two to get the distance from center to bounds, multiply by 0.8 to avoid edge spawns
		//Vector3 offset = (glassWall.GetComponent<Transform> ().localScale / 2) * 0.8f;

		//*
		Bounds myBounds = glassWall.GetComponent<MeshFilter>().mesh.bounds;

		Vector3 min = glassWall.GetComponent<Transform>().TransformPoint(myBounds.min);
		Vector3 max = glassWall.GetComponent<Transform>().TransformPoint(myBounds.max);

		//Vector3 worldPt = glassWall.GetComponent<Transform>().TransformPoint();

		//define the region where we want it to randomly spawn within
		Vector3 spawnLocation = new Vector3(
			Random.Range(min.x, max.x),
			Random.Range(min.y, max.y),
			location.z
			);
		//*/
		/*
		//define the region where we want it to randomly spawn within
		Vector3 spawnLocation = new Vector3(
			Random.Range((location.x - offset.x), (location.x + offset.x)),
			Random.Range((location.y - offset.y), (location.y + offset.y)),
			location.z
			);
		//*/

		//create new object
		GameObject obj = Instantiate(objPrefab, spawnLocation,Quaternion.identity) as GameObject;
		obj.name = "Sphere";
	}
}
