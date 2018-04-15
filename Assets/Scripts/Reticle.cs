using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Reticle : MonoBehaviour {
	public Camera CameraFacing;
	private Vector3 originalScale;

	float WAIT_INC = 1.5f;
	float SelectionTimer;
	Button currButton;
	
	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = new Ray (CameraFacing.transform.position,
		                   CameraFacing.transform.rotation * Vector3.forward);
		Debug.DrawRay (ray.origin, ray.direction * 10);
		RaycastHit hit;
		float distance;
		if (Physics.Raycast (ray, out hit)) {
			distance = hit.distance;
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
				currButton = null;
				SelectionTimer = 0;
			}
			
			if (SelectionTimer >= 100)
			{
				press.onClick.Invoke();
				SelectionTimer = 0;
			}
		} else {
			distance = CameraFacing.farClipPlane * 0.95f;
		}

		transform.position = CameraFacing.transform.position +
							 CameraFacing.transform.rotation * 
							 Vector3.forward * distance;
		transform.LookAt (CameraFacing.transform.position);
		transform.Rotate (0.0f, 180.0f, 0.0f);

		if (distance < 10.0f) {
			distance *= 1 + 5*Mathf.Exp (-distance);
		}

		transform.localScale = originalScale * distance;
	}
}