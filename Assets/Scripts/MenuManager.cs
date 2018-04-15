using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

public class MenuManager : MonoBehaviour 
{
	//the input fields used to enter username and password text
	public InputField username;
	public InputField password;

	//instructions and message for players
	public Text LoginInstructions;
	public Text GameTypeInstructions;

	//The two parents responsible for each submenu
	public GameObject LoginScreen;
	public GameObject GameSelect;

	//AWS integration object
	public UserMan Checker;


	// Use this for initialization
	void Start () 
	{
		Toggle_LoginScreen (true);
		Toggle_GameSelect (false);
	}

	public void Login()
	{
		Checker.matchUserPass (username.text, password.text);
	}

	public void PickGame(int gameNum)
	{
		switch (gameNum) 
		{
		case 0:
			Application.LoadLevel ("Abstract"); 
			break;

		case 1:
			Application.LoadLevel ("Desk"); 
			break;
		}
	}

	public void completeLogin(Dictionary<string, AttributeValue> result, bool valid)
	{
		if (!valid)
			LoginInstructions.text = "Incorrect username/password combination.\nTry again.";
		else {
			Debug.Log (password.text);
			Debug.Log ("Login Success!");
			GameTypeInstructions.text = "Hello " + result ["Name"].S + "!\nWhat therapy would you like to play today?";
			Toggle_LoginScreen (false);
			Toggle_GameSelect (true);
			PlayerPrefs.SetString ("User", result ["Username"].S);
		}
	}

	public void Toggle_LoginScreen(bool Open)
	{
		LoginInstructions.text = "Please enter your credentials";

		Image[] allChildren = LoginScreen.GetComponentsInChildren<Image>();
		foreach (Image child in allChildren)
			child.enabled = Open;

		Text[] Txt_allChildren = LoginScreen.GetComponentsInChildren<Text>();
		foreach (Text child in Txt_allChildren)
			child.enabled = Open;
	}

	public void Toggle_GameSelect(bool Open)
	{
		Image[] allChildren = GameSelect.GetComponentsInChildren<Image>();
		foreach (Image child in allChildren)	
			child.enabled = Open;

		Text[] Txt_allChildren = GameSelect.GetComponentsInChildren<Text>();
		foreach (Text child in Txt_allChildren)	
			child.enabled = Open;
	}
}
