using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Map : MonoBehaviour 
{
	public Image[] Locations; 
	Color baseColor = new Color (.8745f, .8745f, .8745f);
	Color selectColor = new Color (.7137f, .2078f, .2078f);

	// Use this for initialization
	void Start () {
		

	}


	public void SelectLocation(int locationIndex)
	{
		for (int x = 0; x < Locations.Length; x++) 
		{
			//Locations [x].enabled = false;
			Locations [x].color = baseColor;
		}
		//Locations [locationIndex].enabled = true;
		Debug.Log ("Change color of... " + locationIndex);
		Locations [locationIndex].color = selectColor;
	}
}
