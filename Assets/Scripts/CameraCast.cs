using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class CameraCast : MonoBehaviour 
{
	public Texture2D crosshairTexture;
	public Camera camera;

	float WAIT_INC = 1.5f;
	float SelectionTimer;

	Rect position;
	Ray ray;
	Button currButton;

	void Start() 
	{
		position = new Rect(0, 0, crosshairTexture.width, crosshairTexture.height);
		position.center = new Vector2(Screen.width / 2, Screen.height / 2);
		currButton = null;
		SelectionTimer = 0;
	}

	void Update()
	{
		// Cast a ray from the camera
		ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {

			// Grab the button hit by raycast
			Button press = hit.transform.GetComponent<Button> ();

			// If the button has been hit recently, continue SelectionTimer
			if (press && press.Equals (currButton)) {
				SelectionTimer += WAIT_INC;

				// If new button hit, change currButton, reset SelectionTimer, reset selection
			} else if ( press ){
				GameObject myEventSystem = GameObject.Find("EventSystem");
				myEventSystem .GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
				currButton = press;
				currButton.Select ();
				SelectionTimer = 0;

				// If no button hit, reset SelectionTimer, reset selection
			} else {
				GameObject myEventSystem = GameObject.Find("EventSystem");
				myEventSystem .GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
				SelectionTimer = 0;
			}

			if (SelectionTimer >= 100)
			{
				press.onClick.Invoke();
				SelectionTimer = 0;
			}
		}
	}


	void OnGUI() 
	{
		GUI.DrawTexture(position, crosshairTexture);
		if (SelectionTimer > 0 && SelectionTimer < 100) {
			int newWidth = (int)Mathf.Lerp( (float)0.0, 
											(float)crosshairTexture.width, 
											(SelectionTimer / (float)100));

			int newHeight = (int)Mathf.Lerp((float)0.0, 
											(float)crosshairTexture.height, 
											(SelectionTimer / (float)100));

			Rect cocentricPos = new Rect (0, 0, newWidth, newHeight);
			cocentricPos.center = position.center;
			GUI.DrawTexture (cocentricPos, crosshairTexture, ScaleMode.ScaleToFit);
		}
	}
}