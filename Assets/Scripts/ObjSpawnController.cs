using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

public class ObjSpawnController : MonoBehaviour 
{
	//scoring data
	AWSMan aws;

	/*
	public Text currScore;
	*/
	public Map myMap;

	//collectible data
	int numCollectibles = 16;
	Obj currentObject;

	//game state
	bool GameActive = true;
	float t = 0;
	float maxTimeout = 60;
	int Index = 0;	//current index of the following two arrays
	float CurrentScore=0;
	float MaxScore=0;

	//scoring info
	List<float> ObjectTimeArray = new List<float>(); //how long it took them to get each goal
	List<float> ObjectScoreArray = new List<float>(); //SCORE BASED ON TIME AND INDEX
	List<int> ObjectIndexArray = new List<int>(); //keeps track of which locations were selected as goals

	//used for sending data to AWS
	public List<ObjSpawn> spawnPoints;
	public List<GameObject> spawnObjects;
	List<int> objectDifficultyArray = new List<int>();
	List<int> objectDifficultyArrayDEFAULT = new List<int>{0,0,0,0,0,0,0,0,5,10,10,20,20,20,20,20} ;
	

	void Awake()
	{

	}


	// Use this for initialization
	void Start () 
	{
		aws = new AWSMan();
		Dictionary<string, AttributeValue> lastSession = aws.LastSession ();
		if (lastSession != null) {
			foreach (AttributeValue value in lastSession["ObjectDifficultyArray"].L) {
				objectDifficultyArray.Add (int.Parse(value.N));
			}
		} else {
			objectDifficultyArray = objectDifficultyArrayDEFAULT;
		}

		List<int> usedObjects = new List<int>();

		//for (int i = 0; i < numCollectibles; i++)	usedObjects.Add(
		//select a random cannon to fire
		foreach (ObjSpawn location in spawnPoints)
		{
			int objectNum = Random.Range(0,numCollectibles);

			while(usedObjects.Contains(objectNum))
				objectNum = Random.Range(0,numCollectibles);

			usedObjects.Add(objectNum);
			location.Spawn(spawnObjects[objectNum]);
		}

		ObjectIndexArray.Add(nextObjectPicker ());

		//EndGame();

	}

	public int nextObjectPicker(){
		//take in object difficulty array
		int totalweight = 0;

		//OK peter, we grab the total weight, then we pick a number in the range
		//Think of it like this: There's 
		foreach (int x in objectDifficultyArray) totalweight += x;
		int rand;
		int selectedRegion = -1;
		do {
			rand = Random.Range(0, totalweight);

			for (int i = 0; i < numCollectibles; i++) {
				if (rand < objectDifficultyArray [i]) {
					selectedRegion = i;
					break;
				}
				rand = rand - objectDifficultyArray [i];
			}
		} while (ObjectIndexArray.Contains (selectedRegion));

		Debug.Log ("selectedRegion = " + selectedRegion);
		//use this is in spawnpoints[selectedRegion]
		t = 0;
		spawnPoints [selectedRegion].SelectMe ();
		//update map
		myMap.SelectLocation (selectedRegion);
		return selectedRegion;
	}

	// Update is called once per frame
	void Update () 
	{
		if (GameActive)
		{
			if ((t  += Time.deltaTime) >= 60)
			{
				t = 60;
				ObjectCollected (currentObject.difficultyScore);
			}
		}
	}

	public void SetCurrentObject(Obj newObject)
	{
		currentObject = newObject;
	}

	public void ObjectCollected(float difficultyScore)
	{
		//record the time it took to collect current object
		ObjectTimeArray.Add(t);

		//calculate the relative score of the object collected
		float drawerScore = difficultyScore * (maxTimeout / 2 - t);
		CurrentScore += drawerScore;
		MaxScore += difficultyScore * (maxTimeout / 2);
		//if the score was negative, be nice, give em' a break
		if (drawerScore < 0)	drawerScore = drawerScore / 2;

		//record the score determined of the object collected
		ObjectScoreArray.Add (drawerScore);

		//asign the next object to be found
		Index++;
		if (Index < 8) {
			ObjectIndexArray.Add(nextObjectPicker ());
		} else { 
			EndGame();
		}
	}

	void EndGame(){
		GameActive = false;
		AutoScalingDifficulty ();
		RealData sessionData = new RealData
		{
			ID = System.Guid.NewGuid().ToString(),
			User = PlayerPrefs.GetString("User"),
			Time = System.DateTime.UtcNow.ToString(),
			GameType = "Desk",
			//ObjectScoreArray = ObjectScoreArray,
			ObjectDifficultyArray = objectDifficultyArray
		};
		aws.CreateItem (sessionData);

	}

	void AutoScalingDifficulty()
	{
		float ScorePercent = (CurrentScore / MaxScore);
		// 1,2,3,4,5 (col1) 6,7 (col2) 8,9 (col3) 10,11 (col4) 12,13,14,15,16 (col5)
		List<int> DifficultyAdjustment = new List<int>{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0} ;

		if (ScorePercent < .2) //0%-20%
		{
			DifficultyAdjustment = new List<int>{0,0,0,0,0,0,0,5,5,5,5,5,5,5,5,5} ;
		}
		else if (ScorePercent < .3) //20%-30%
		{
			DifficultyAdjustment = new List<int>{0,0,0,0,0,0,5,10,10,10,10,5,5,5,5,5} ;
		}
		else if (ScorePercent < .5) //30%-50%
		{
			DifficultyAdjustment = new List<int>{0,0,0,0,0,10,10,10,10,5,5,0,0,0,0,0} ;
		}
		else if (ScorePercent < .7) //50%-70%
		{
			DifficultyAdjustment = new List<int>{1,2,3,4,5,10,10,5,5,0,0,0,0,0,0,0} ;
		}
		else if (ScorePercent < .8) //70%-80%
		{
			DifficultyAdjustment = new List<int>{1,2,3,4,5,6,7,8,9,5,5,-1,-1,-1,-1,-1} ;
		}
		else //if score => .8
		{
			DifficultyAdjustment = new List<int>{5,5,5,5,5,6,7,8,9,0,0,-1,-2,-3,-4,-5} ;
		}

		AdjustObjDiffArr(DifficultyAdjustment); //modifies objectDifficultyArray


	}


	public void AdjustObjDiffArr(List<int> change){
		for (int i=0; i < objectDifficultyArray.Count; i++ )
		{
			objectDifficultyArray[i] += change[i];
			if (objectDifficultyArray[i] < 0) objectDifficultyArray[i]=0;
			//Debug.Log ("xRegionWeight[" + i + "] = " + xRegionWeight[i]);
		}
		
		Debug.Log ("Drawer Weight:  " + objectDifficultyArray[0] + ", " 
		           + objectDifficultyArray[1] + ", " 
		           + objectDifficultyArray[2] + ", " 
		           + objectDifficultyArray[3] + ", " 
		           + objectDifficultyArray[4] + ", " 
		           + objectDifficultyArray[5] + ", " 
		           + objectDifficultyArray[6] + ", " 
		           + objectDifficultyArray[7] + ", " 
		           + objectDifficultyArray[8] + ", " 
		           + objectDifficultyArray[9] + ", " 
		           + objectDifficultyArray[10] + ", " 
		           + objectDifficultyArray[11] + ", " 
		           + objectDifficultyArray[12] + ", " 
		           + objectDifficultyArray[13] + ", " 
		           + objectDifficultyArray[14] + ", " 
		           + objectDifficultyArray[15]);
	}
}