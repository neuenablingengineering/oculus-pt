using UnityEngine;
using System.Collections;

public class CollectibleController : MonoBehaviour 
{
	GameControl gameController;
	float t;
	public int score;
	public int xRegion;
	public int yRegion;
	Color colorA = Color.red;
	Color colorB = Color.green;

	// Use this for initialization
	void Start () 
	{
		//get the gameControl
		gameController = GameObject.Find ("SpawnWall").GetComponent<GameControl>();
		t = 0f;
	}

	// Update is called once per frame
	void Update () 
	{
		if((t += Time.deltaTime) > 15.0f)
		{
			Destroy(this.gameObject);
			gameController.incrementScore(0);
			gameController.incrementScoreStream(new CollectibleFrame(false, score));
			//Debug.Log ("Death by aging");
			//if(lastEgg)	gameController.EndGame();
			//GetComponent<Material>().color = colorA;
		}
		//GetComponent<Material>().color = Color.Lerp (colorB, colorA, (t / 15.0f));
		GetComponent<MeshRenderer>().material.color = Color.Lerp (colorB, colorA, (t / 15.0f));
	}

}
