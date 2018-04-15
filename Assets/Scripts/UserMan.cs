using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Amazon;
using Amazon.Runtime;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentity;

public class UserMan : MonoBehaviour
{
	private string AuthPoolID = "us-east-1:aa610494-90f2-43a0-aca3-93e5360b7351";
	private static AmazonDynamoDBClient _ddbClient;
	private static DynamoDBContext _ddbContext;
	private Amazon.Runtime.AWSCredentials _credentials;
	public MenuManager myLogin;

	private Amazon.Runtime.AWSCredentials Credentials
	{
		get
		{
			if (_credentials == null)
				_credentials = new CognitoAWSCredentials(AuthPoolID, RegionEndpoint.USEast1);
			return _credentials;
		}
	}

	protected AmazonDynamoDBClient Client
	{
		get 
		{
			if (_ddbClient == null) {
				_ddbClient = new AmazonDynamoDBClient (Credentials, RegionEndpoint.USEast1);
			}

			return _ddbClient;
		}
	}

	private DynamoDBContext Context
	{
		get
		{
			if (_ddbContext == null)
				_ddbContext = new DynamoDBContext(Client);

			return _ddbContext;
		}
	}

	public void CreateItem(UserData data)
	{
		Context.SaveAsync(data, (result) =>{
		    if (result.Exception != null)
			{
				return;
			}
		});
	}

	public void matchUserPass(string username, string password)
	{
		// Define scan conditions
		Dictionary<string, Condition> conditions = new Dictionary<string, Condition>();

		Condition userMatch = new Condition();
		userMatch.ComparisonOperator = ComparisonOperator.EQ;
		userMatch.AttributeValueList.Add(new AttributeValue { S = username });
		conditions["Username"] = userMatch;

		Condition passMatch = new Condition();
		passMatch.ComparisonOperator = ComparisonOperator.EQ;
		passMatch.AttributeValueList.Add(new AttributeValue { S = password });
		conditions["Password"] = passMatch;

		// Issue request

		ScanRequest request = new ScanRequest 
		{
			TableName = "Users",
			ScanFilter = conditions
		};

		Client.ScanAsync (request, (result)=>{
			if (result.Response.Count == 1){
				this.myLogin.completeLogin(result.Response.Items[0], true);
			} else
				this.myLogin.completeLogin(null, false);
		});
	}
}

[DynamoDBTable("Users")]
public class UserData
{
	[DynamoDBHashKey]   // Hash key.
	public string ID { get; set; }
	[DynamoDBProperty("User")]
	public string Name { get; set; }
	[DynamoDBProperty("Username")]
	public string Username { get; set; }
	[DynamoDBProperty("Password")]
	public string Password { get; set; }
}