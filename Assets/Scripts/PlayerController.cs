using UnityEngine;
using System.Collections.Generic;
using System; //This allows the IComparable Interface
// Used to disable/enable player control and GUI

/******************************************************************************
   This is one of the most important  classes in the project. It is used to 
   handle the player's motion and interaction with other objects. The 
   script tracks cursor motion and then maps it to the scenario world to 
   rotate both the player obelisk and the attached camera. There is also
   functionality to allow the player's obelisk to move with WASD. Finally, 
   there are functions to allow for (un)pausing the player's motion and 
   cursor tracking. 
******************************************************************************/
public class PlayerController : MonoBehaviour {

	#region Variables and Properties
	/// <summary>
	///	Public accessor to player state
	/// True if camera on, false if camera off
	/// </summary> 
	public bool PlayerState() {
		return playerEnabled;
	}
	/// <summary>
	/// Public accessor to change or check current active canvas
	/// </summary> 
	public int GUIState {
		get{return guiState;}
		set{guiState = value;}
	}
	#endregion
	
	#region Fields- Private and Internal
	/// <summary>
	/// Reference to MouseLook component
	/// Tracks player mouse motion
	/// </summary> 
	private MouseLook playerMouseLook;
	/// <summary>
	/// Reference to MouseLook component
	/// Tracks mouse motion relative to camera and adjusts camera
	/// </summary> 
	private MouseLook cameraMouseLook;
	/// <summary>
	/// Reference to CharacterMotor component
	/// Allows player to move player object around the room with WASD
	/// </summary> 
	private CharacterMotor characterMotor;
	/// <summary>
	/// Refernce to camera attached to player object
	/// </summary> 
	private Camera playerCam; 
	/// <summary>
	/// Reference to obelisk that represents player
	/// </summary> 
	private  GameObject playerBody; 
	/// <summary>
	/// Reference to crosshair component 
	/// </summary> 
	private GameObject crosshair;
	/// <summary>
	/// Is player motion control enabled?
	/// </summary> 
	private bool playerEnabled;
	/// <summary>
	/// Corresponds to which canvas is active
	/// </summary> 
	private int guiState;
	/// <summary>
	/// Current number of GUI canvases in the scene
	/// </summary> 
	private int numCanvas = 0;
	/// <summary>
	/// Currently active canvas
	/// 0 is neutral state when all Canvas' may be active	
	/// </summary> 
	private int ActiveCanvas = 0;
	#endregion
	
	#region Methods - Private and UnityEngine
	/// <summary>
	/// Runs at start of scene before objects loaded 
	/// Used to set references and init variables
	/// </summary>
	void Awake ()  {
		playerMouseLook = gameObject.GetComponent<MouseLook>();
		cameraMouseLook = transform.Find("Main Camera").GetComponent<MouseLook>();

		// Player movement
		characterMotor = gameObject.GetComponent<CharacterMotor>();

		// Crosshair gui
		crosshair = GameObject.Find("GUI/Crosshair");	

		//assigns camera reference to Main Camera
		playerCam = GameObject.Find("First Person Controller/Main Camera").GetComponent<Camera>();
			
		playerBody = GameObject.Find("First Person Controller/Graphics");

		playerEnabled = true;
		SetPlayerState(playerEnabled);
	}
	#endregion
	
	#region Methods - Public
	/// <summary>
	/// Called only when canvas is first created, assigns next ID in the series
	/// </summary> 
	public int getCanvasID() {
		return numCanvas++;
	}
	/// <summary>
	/// Sets the currently active canvas based on input
	/// </summary> `
	public void setCanvasState(int index) {
		ActiveCanvas = index;
	}
	/// <summary>
	/// Checks which canvases are active
	/// Returns true for 0, which means all active 
	/// </summary> 
	public bool getCanvasState(int index) {
		if((ActiveCanvas == 0) || (ActiveCanvas == index)) 	return true;
		else return false;
	}
	/// <summary>
	/// Method to en/disable player motion control
	/// Turns player and player camera mouse tracking off
	/// Disables player WASD motion 
	/// Locks cursor in place and sets crosshair to active 
	/// </summary> 
	public void SetPlayerState(bool state) {
		playerEnabled = state;

		// Stop MouseLook two cameras
		playerMouseLook.enabled = state;
		cameraMouseLook.enabled = state;

		// Stop movement
		characterMotor.enabled = state;

		Screen.lockCursor = state;

//		crosshair.GetComponent<CrossHair>().CrossOn = state;

		playerCam.enabled = state; 
		playerBody.SetActive(state); 
	}
	#endregion

}
