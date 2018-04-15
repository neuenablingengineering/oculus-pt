using UnityEngine;
using System.Collections;

public class WaterMove : MonoBehaviour {

	//float movement;
	int timer = 0;
	public GameObject Duck;
	public float DuckSpeed = 1;
	public float DuckRotation = 7;
	Transform DuckTran;
	float[] sinWave;
	float[] cosWave;

	void Start() 
	{
		sinWave = new float[360];
		cosWave = new float[360];
		DuckTran = Duck.GetComponent<Transform>();

		for (int i = 0; i < 360; i++)
		{
			sinWave[i] = .004F* Mathf.Sin((Mathf.Deg2Rad * i));
			//Debug.Log ("Sin( " + i + " ) = " + sinWave[i]);
		}
		//*/
	}

	void FixedUpdate () 
	{
	//chgange to DELTA TIME
	
		if (timer++ >= 359)
			timer = 0;

		DuckTran.Translate(0, DuckSpeed * Time.deltaTime, sinWave[timer]); 
		DuckTran.Rotate(0,0, DuckRotation * Time.deltaTime, Space.Self); 
		transform.Translate (sinWave [timer], (0.5F * sinWave [timer]), sinWave[timer]);

	}
}
