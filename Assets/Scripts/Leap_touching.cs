using UnityEngine;
using System.Collections;

namespace Leap.Unity.PinchUtility 
{
	/// <summary>
	/// Use this component on a Game Object to allow it to be manipulated by a pinch gesture.  The component
	/// allows rotation, translation, and scale of the object (RTS).
	/// </summary>
	public class Leap_touching : MonoBehaviour {

		[SerializeField]
		private LeapPinchDetector _pinchDetectorA;

		[SerializeField]
		private LeapPinchDetector _pinchDetectorB;

		private LeapPinchDetector _pinchDetectorCurrent;

		Transform _anchor;
		public float AngleOffset = 0;
		public bool isDrawer = false;

		private Vector3 anchorPosOrigin;
		private float anchorAngleOrigin;
		private float anchorDrawerOrigin;

		private float _defaultNearClip;
		private bool gripping = false;
		private bool waitingForRelease = false;
		void Awake() 
		{
			if (_pinchDetectorA == null || _pinchDetectorB == null) 
			{
				Debug.LogWarning("Both Pinch Detectors of the LeapRTS component must be assigned. This component has been disabled.");
				enabled = false;
			}
			_anchor = this.transform.parent.transform;
			anchorAngleOrigin = _anchor.eulerAngles.y;
			anchorPosOrigin = _anchor.position;
			anchorDrawerOrigin = transform.position.z;
		}

		void Update() 
		{
			
			bool didUpdate = false;
			didUpdate |= _pinchDetectorA.DidChangeFromLastFrame;
			didUpdate |= _pinchDetectorB.DidChangeFromLastFrame;


			if (gripping && waitingForRelease) 
			{	
				WaitForRelease ();
			}

			if (_pinchDetectorA.IsPinching && (_pinchDetectorA.grippingObject == this.name || _pinchDetectorA.grippingObject == "")) 
			{
				transformSingleAnchor (_pinchDetectorA);
			} 
			else 
			{
				if (_pinchDetectorB.IsPinching &&  (_pinchDetectorB.grippingObject == this.name || _pinchDetectorB.grippingObject == ""))
				{
					transformSingleAnchor (_pinchDetectorB);
				}
			}


		}

		private void transformSingleAnchor(LeapPinchDetector singlePinch) 
		{
			//Debug.Log("gripping  " + gripping + "     grippingObject" +  singlePinch.grippingObject + " _pinchDetectorCurrent.grippingObject " + _pinchDetectorCurrent.grippingObject);

			if (gripping) 
			{
				//used to make sure this is the only object being grabbed
				singlePinch.grippingObject = this.name;

				//listing the hand being used as the current hand for other funtions to access it's values
				_pinchDetectorCurrent = singlePinch;

				if (isDrawer) 
				{
					float drawerLocation = singlePinch.Position.z;

					if (drawerLocation > anchorDrawerOrigin + 0.5f)
						drawerLocation = anchorDrawerOrigin + 0.5f;

					if (drawerLocation < anchorDrawerOrigin)
						drawerLocation = anchorDrawerOrigin;

					//Debug.Log("drawerLocation  " + drawerLocation);
					transform.position = new Vector3 (transform.position.x,  transform.position.y, drawerLocation);
				} 
				else 
				{
					//get relevant data for processing where the cabinet should be angled
					Vector3 currentRotation = _anchor.eulerAngles;
					Vector3 difference = singlePinch.Position - anchorPosOrigin;

					//calculate what the angle of the cabinet should be in relation to pinch point
					float diffAngle = Mathf.Rad2Deg * Mathf.Atan2 (difference.z, difference.x);
					float finalAngle = -diffAngle - AngleOffset;

					//make sure it isn't out of bounds
					//this code is kinda hacky
					if(AngleOffset > 0)
					{
						if (finalAngle < 0)	finalAngle += 360;
						if (anchorAngleOrigin > finalAngle)					finalAngle = anchorAngleOrigin;
						if (finalAngle < 0)	finalAngle += 360;
						if(finalAngle > anchorAngleOrigin + AngleOffset)	finalAngle =  anchorAngleOrigin + AngleOffset;
					}
					else
					{
						if (anchorAngleOrigin < finalAngle)					finalAngle = anchorAngleOrigin;
						if (finalAngle < anchorAngleOrigin + AngleOffset)	finalAngle = anchorAngleOrigin + AngleOffset;
					}

					//set the angle of the object
					//Debug.Log("finalAngle  " + finalAngle);
					_anchor.eulerAngles = new Vector3 (currentRotation.x, finalAngle, currentRotation.z);
				}

			}

		}

		void OnTriggerEnter(Collider other) 
		{
			if(other.name.Contains("bone") && other.transform.parent.name == "index")
			{
				gripping = true;
				waitingForRelease = false;
				//Debug.Log("enter  " + other.name);
			}
		}

		void OnTriggerExit(Collider other) 
		{
			if(other.name.Contains("bone") && other.transform.parent.name == "index")
			{
				waitingForRelease = true;
				WaitForRelease ();

				//Debug.Log("leave  " + other.name);
			}
		}

		void WaitForRelease()
		{
			if (_pinchDetectorCurrent == null) 
			{
				gripping = false;
				return;
			}
			if (_pinchDetectorCurrent.IsPinching) 
			{
				_pinchDetectorCurrent.grippingObject = "";
				gripping = false;
			}
		}
	}
}
