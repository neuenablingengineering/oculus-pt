using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

public class Login : MonoBehaviour {
	private string username = "";
	private string password = "";
	public UserMan Checker;

	private Rect windowRect = new Rect (0, 0, Screen.width, Screen.height);

	void OnGUI()
	{
		GUI.Window (0, windowRect, WindowFunction, "Log in");	
	}

	void WindowFunction(int a)
	{
		username = GUI.TextField (new Rect (Screen.width / 3, 2 * Screen.height / 5, Screen.width / 3, Screen.height / 10), username, 20);
		password = GUI.PasswordField (new Rect (Screen.width / 3, 2 * Screen.height / 3, Screen.width / 3, Screen.height / 10), password, "*"[0], 20);

		if (GUI.Button(new Rect(Screen.width / 2, 4 * Screen.height / 5, Screen.width / 8, Screen.height / 8), "Login"))
		{
			Checker.matchUserPass ( username, password );
		}

		GUI.Label (new Rect (Screen.width / 3, 35 * Screen.height / 100, Screen.width / 5, Screen.height / 8), "Username");
		GUI.Label (new Rect (Screen.width / 3, 62 * Screen.height / 100, Screen.width / 8, Screen.height / 8), "Password");
	}

	public void completeLogin(Dictionary<string, AttributeValue> result)
	{
		Debug.Log ("Login Success!");
		PlayerPrefs.SetString ("User", result ["Username"].S);
		Application.LoadLevel ("Menu");
	}
}
