using UnityEngine;
using System.Collections;

public class PlayerSight : MonoBehaviour {

	GameControl gameController;

	// Use this for initialization
	void Start () 
	{
		//get the gameControl
		gameController = GameObject.Find ("SpawnWall").GetComponent<GameControl>();
	}

	void Update() {
		Ray ray = this.GetComponent<Camera>().ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0));
		RaycastHit rayHitInfo;
		GameObject hitObject;
		if (Physics.Raycast(ray, out rayHitInfo)) 
		{
			//Debug.Log (rayHitInfo.collider.gameObject.name);

			if(rayHitInfo.collider.gameObject.name == "Sphere")
			{
				hitObject = rayHitInfo.collider.gameObject;
				int objScore = hitObject.GetComponent<CollectibleController>().score;
                int objX = hitObject.GetComponent<CollectibleController>().xRegion;
                int objY = hitObject.GetComponent<CollectibleController>().yRegion;

				gameController.incrementScore(objScore);
				gameController.incrementScoreStream(new CollectibleFrame(true, objScore));
                gameController.updateCollectArray(objX, objY);

				Destroy (hitObject);
				//Debug.Log ("Destroyed with laser vision");
			}
		}
	}
}
