using UnityEngine;
using System.Collections;

/******************************************************************************
		This class will attach to the player object and control the 
		display state of the crosshair in the center of the screen
******************************************************************************/
public class CrossHair : MonoBehaviour {

	#region Variables and Properties
	
	public Texture2D crosshairImage;
	public bool CrossOn = true;	

	#endregion
	
	#region Methods - Private and UnityEngine

	void OnGUI()
	{
		float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
		float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
		GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
	}
	#endregion
}
