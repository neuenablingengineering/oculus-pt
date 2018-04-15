using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CollectibleFrame
{
	public bool Result;
    public int Score;
    public int TOTAL;

	public CollectibleFrame(bool result, int score)
	{
		Result = result;
        Score = score;
        if (Result)
            TOTAL = score;
        else
            TOTAL = 0;
	}
}

public class GameControl : MonoBehaviour 
{
	//instantiate level object to read in details of gameplay
	//level theLevel;

	//list of object to collect
	List<GameObject> Collectibles;

	//scoring data
	public Text Lives;
	int livesLeft = 3;
	public Text currScore;
	int score = 0;
	int highscore = 0;
	float defaultSpawnTime = 1.5f;

	//spawner stuff
	public GameObject glassWall;
	public GameObject objPrefab;


	//gameplay data
	float TimeLapse = 0;
	float NextSpawn = 0;
    float NextSpawnTime = 3.0f;
	int Index = 0;
	int numCaught = 0;
	int numCollectibles = 20;
	//float percentCaught = 0;
    List<int> xRegionWeight = new List<int>(new int[] { 0, 0, 0, 0, 0, 26, 37, 37 });

	//used for dynamic difficulty
	LinkedList<CollectibleFrame> scoreStream = new LinkedList<CollectibleFrame>();
	int scoreStreamScore;
	int BUFFERSIZE = 5;
	int gameMode = 2;
	bool GameActive = true;
	
	//used for sending data to AWS
    List<List<int>> SpawnedArray = new List<List<int>>();
    List<List<int>> CollectArray = new List<List<int>>();

	void Awake()
	{
		Application.targetFrameRate = 30;

		//instantiate the array of objects
		Collectibles = new List<GameObject>();
	}

	// Use this for initialization
	void Start () 
	{
		Debug.Log (System.DateTime.UtcNow.ToString ());
        for (int x = 0; x < 8; x++)
        {
			SpawnedArray.Add(new List<int> {0, 0, 0, 0, 0, 0, 0, 0});
			CollectArray.Add(new List<int> {0, 0, 0, 0, 0, 0, 0, 0});
        }
		//EndGame();
       
	}

	// This Update uses the a random seed to spawn eggs;
	void Update () 
	{
		if(GameActive)
		{
			if (numCollectibles > Index) {
				TimeLapse += Time.deltaTime;

				if (NextSpawnTime < TimeLapse) {
					//get the Timelapse in sync with the rythm specified
					TimeLapse = TimeLapse - NextSpawnTime;

					//Spawn in the area designated by calcNextSpawn
					SpawnInArea (calcNextSpawnLocation ());

					Index++;
				}
			} else
				EndGame ();
		}
	}


    public void SpawnInArea(Vector3 region) // x from 0 to 7, y from 0 to 7, z is the score!
    {
        //get the location of the glass wall
        Vector3 wallLocation = glassWall.GetComponent<Transform>().position;

        //divide by two to get the distance from center to bounds, multiply by 0.8 to avoid edge spawns
        Vector3 offset = (glassWall.GetComponent<Transform>().localScale / 2) * 0.8f;
		
		//divide by two to get the distance from center to bounds, multiply by 0.8 to avoid edge spawns
		//Vector3 offset = (glassWall.GetComponent<Transform> ().localScale / 2) * 0.8f;
		
		//*
		Bounds myBounds = glassWall.GetComponent<MeshFilter>().mesh.bounds;
		
		Vector3 min = glassWall.GetComponent<Transform>().TransformPoint(myBounds.min);
		Vector3 max = glassWall.GetComponent<Transform>().TransformPoint(myBounds.max);

		Vector3 diff = max-min;

        //define the region where we want it to randomly spawn within
        Vector3 spawnLocation = new Vector3(
			Random.Range((min.x + (diff.x / 8) * region.x), (min.x + (diff.x / 8) * (region.x + 1)) ),
		    Random.Range((min.y + (diff.y / 8) * region.y), (min.y + (diff.y / 8) * (region.y + 1)) ),
            wallLocation.z);

        //create new object
        GameObject obj = Instantiate(objPrefab, spawnLocation, Quaternion.identity) as GameObject;
        obj.GetComponent<CollectibleController>().score = (int)region.z;
		obj.GetComponent<CollectibleController>().xRegion = (int)region.x;
		obj.GetComponent<CollectibleController>().yRegion = (int)region.y;
        obj.name = "Sphere";

        updateSpawnedArray((int)region.x, (int)region.y);
    }

    public Vector3 calcNextSpawnLocation()
    {
        scoreStreamScore = 0;
        int scoreStreamPossible = 0;
        CollectibleFrame[] sphereFrames = new CollectibleFrame[BUFFERSIZE];
        int xRegion; // 0-7
        int yRegion; // 0-7
        int score; // 5-125
        List<int> xRegionChange = new List<int>(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 });

        if (Index > 5)
        {
            scoreStream.CopyTo(sphereFrames, 0);

			for (int i = 0; i < scoreStream.Count; i++)
            {
                scoreStreamPossible += sphereFrames[i].Score;
                scoreStreamScore    += sphereFrames[i].TOTAL;
            }
            float scoreStreamPercent = scoreStreamScore / scoreStreamPossible;

            ////////////////////////////////
            //HERES WHERE THINGS GET WEIRD//
            ////////////////////////////////
            
            if (scoreStreamPercent < .2)
            {
                xRegionChange = new List<int>(new int[] { -5, -4, -3, -2, -1, 0, 5, 10 });
            }
            else if (scoreStreamPercent < .3)
            {
                xRegionChange = new List<int>(new int[] { -2, -2, -2, -2, 0, 5, 5, 5 });
            }
            else if (scoreStreamPercent < .5)
            {
                xRegionChange = new List<int>(new int[] { 0, 0, 0, 1, 5, 3, -1, -2 });
            }
            else if (scoreStreamPercent < .7)
            {
                xRegionChange = new List<int>(new int[] { 0, 1, 2, 5, 4, -1, -2, -5 });
            }
            else if (scoreStreamPercent < .8)
            {
                xRegionChange = new List<int>(new int[] { 1, 2, 4, 6, 8, -2, -4, -8 });
            }
            else //if score => .8
            {
                xRegionChange = new List<int>(new int[] { 3, 4, 5, 5, 0, -5, -5, -5 });
            }
        }
        regionChanger(xRegionChange); //modifies xRegionWeight
        
        xRegion = regionPicker(xRegionWeight); //anywhere from 5-7
        yRegion = Random.Range(0, 8); //anywhere from 0-7
        //Debug.Log ("SpawnTime" + defaultSpawnTime);
        score = calculateScore(xRegion);

        Vector3 spawnLocation = new Vector3(
            xRegion,
            yRegion,
            score);
        return spawnLocation;
    }

    public void regionChanger(List<int> change){
        for (int i=0; i < xRegionWeight.Count; i++ )
		{
            xRegionWeight[i] += change[i];
            if (xRegionWeight[i] < 0) xRegionWeight[i]=0;
			//Debug.Log ("xRegionWeight[" + i + "] = " + xRegionWeight[i]);
        }

		Debug.Log ("Region Weight:  " + xRegionWeight[0] + ", " + xRegionWeight[1] + ", " + xRegionWeight[2] + ", " + xRegionWeight[3] + 
		           ", " + xRegionWeight[4] + ", " + xRegionWeight[5] + ", " + xRegionWeight[6] + ", " + xRegionWeight[7]);
    }

    public int calculateScore(int xregion)
    {
        int minScore = 0;
        int maxScore = 0;
        switch (xregion)
        {
            case 0: 
            case 1: 
            case 2:
            case 3: maxScore = 125 - (25 * xregion);
                minScore = 125 - (25 * (xregion + 1));
                break;
            case 4: 
            case 5: 
            case 6: 
            case 7: maxScore = 25 - (5 * (xregion - 4));
                minScore = 25 - (5 * (xregion - 3));
                break;
        }
		int score = Random.Range(minScore, maxScore + 1);
		return score;
    }

    public int regionPicker(List<int> weights){
        List<int> regions = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
        int totalweight = 0;
        foreach (int x in weights) totalweight += x;
        int rand = Random.Range(0, totalweight);
        int selectedRegion = -1;

        for (int i = 0; i < regions.Count; i++)
        {
            if (rand < weights[i])
            {
                selectedRegion = regions[i];
                break;
            }

            rand = rand - weights[i];
        }

        return selectedRegion;
    }

	//selects the game mode from the main menu button items
	public void GameMode(int option)
	{
		if((option >= 0) && (option < 4))	gameMode = option;
		//Debug.Log("gamemode = " + gameMode);
	}

	public int CurrentGameType()
	{
		return gameMode;
	}

	public void PlayGame()
	{
		if (gameMode == 0)
		{
			
		}
		else if(gameMode == 1)
		{
			livesLeft = 3;
			Lives.text = "Lives: " + livesLeft.ToString();
			MakeTextTransparent (false);
		} 
		else 
		{
			Lives.enabled = false;
			MakeTextTransparent (true);
		}
			
		//clear any egg information
		scoreStream.Clear ();
		if(Collectibles.Count != 0)	Collectibles.Clear ();

		//reset game values
		defaultSpawnTime = 0.5f;
		Index = 0;
		score = 0;
		currScore.text = score.ToString();

		StartCoroutine(SlightPause());
		//GameActive = true;
	}

	void MakeTextTransparent(bool go)
	{
		Color myColor = currScore.color;

		if (go)
			currScore.color = new Color (myColor.r, myColor.g, myColor.b, 0);
		else
			currScore.color = new Color (myColor.r, myColor.g, myColor.b, 255);
	}

	public void EndGame()
	{
		//screenManager.Toggle_RetryMenu(true);
		GameActive = false;
		Collectibles.Clear ();

		if(score > highscore)
		{
			highscore = score;
			//highScore.text = "New High\nScore:\n" + highscore.ToString();
			//currScoreRetry.enabled = false;
		}
		else{
			//highScore.text = "High\nScore:\n" + highscore.ToString();
			//currScoreRetry.enabled = true;
			//currScoreRetry.text = "current score: " + score.ToString();
		}

		//Connecting to AWS servers to feed data
		HeatMap sessionData = new HeatMap
		{
			ID = System.Guid.NewGuid().ToString(),
			User = PlayerPrefs.GetString("User"),
			Time = System.DateTime.UtcNow.ToString(),
			GameType = "Abstract",
			Spawned = SpawnedArray,
			Collect = CollectArray,
		};

		AWSMan aws = new AWSMan();
		aws.CreateItem(sessionData);
	}

	public void incrementScore(int increment)
	{
		if(increment>0)	
		{
			score += increment;
			currScore.text = (score).ToString();

			numCaught++;
			//this.gameObject.GetComponent<AudioSource>().clip = goodSnd;
			//this.gameObject.GetComponent<AudioSource>().Play();
		}
		//currScore.text = (score+=increment).ToString();
	}

	public int getScore()
	{
		return score;
	}

	public void incrementScoreStream(CollectibleFrame frame)
	{
		if (scoreStream.Count > BUFFERSIZE-1)	scoreStream.RemoveFirst ();

		scoreStream.AddLast (frame);
	}
	
	public void updateSpawnedArray(int x, int y)
	{
        SpawnedArray[x][y] += 1;
	}

    public void updateCollectArray(int x, int y)
	{
        CollectArray[x][y] += 1;
	}

	IEnumerator SlightPause() 
	{
		float t = 0f;
		
		while((t += Time.deltaTime) < 2)
		{
			yield return null;
		}
		GameActive = true;
		yield return 0;
	}

}