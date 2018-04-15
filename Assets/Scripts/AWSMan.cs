using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentity;

public class AWSMan
{
	private Text resultText;
	private string IdentityPoolId = "us-east-1:c253a5a1-2171-4d00-95b8-9d4430389e6d";
	private static IAmazonDynamoDB _ddbClient;
	private static DynamoDBContext _ddbContext;
	private Amazon.Runtime.AWSCredentials _credentials;

	private Amazon.Runtime.AWSCredentials Credentials
	{
		get
		{
			if (_credentials == null)
				_credentials = new CognitoAWSCredentials(IdentityPoolId, RegionEndpoint.USEast1);
			return _credentials;
		}
	}

	protected IAmazonDynamoDB Client
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

	public void CreateItem(RealData data)
	{
		Context.SaveAsync(data, (result) =>{
			if (result.Exception != null)
			{
				resultText.text += result.Exception.Message;
				return;
			}
		});
	}

	public void CreateItem(HeatMap map)
	{
		Context.SaveAsync(map, (result) =>
			{
				if (result.Exception != null)
				{
					resultText.text += result.Exception.Message;
					return;
				}
			});
	}

	public Dictionary<string, AttributeValue> LastSession()
	{
		// Define scan conditions
		Dictionary<string, Condition> conditions = new Dictionary<string, Condition>();

		// Title attribute should contain the string "Adventures"
		Condition userMatch = new Condition();
		userMatch.ComparisonOperator = ComparisonOperator.CONTAINS;
		userMatch.AttributeValueList.Add(new AttributeValue { S = "John Doe" });
		conditions["User"] = userMatch;

		Condition typeMatch = new Condition();
		userMatch.ComparisonOperator = ComparisonOperator.CONTAINS;
		userMatch.AttributeValueList.Add(new AttributeValue { S = "Abstract" });
		conditions["GameType"] = typeMatch;

		var request = new ScanRequest 
		{
			TableName = "VRPT",
			ScanFilter = conditions
		};

		// Issue request
		Dictionary<string, AttributeValue> last = null;
		Client.ScanAsync(request, (result)=>{
			if (result.Response != null) {
				List<Dictionary<string, AttributeValue>> items = result.Response.Items;
				foreach(Dictionary<string, AttributeValue> item in items){
					if(last == null || compUTC(item["Time"].S, last["Time"].S)) {
						last = item;
					}
				}
			}
		});

		return last;
	}

	bool compUTC(string utc1, string utc2){
		DateTime DT1 = DateTime.Parse (utc1);
		DateTime DT2 = DateTime.Parse (utc2);
		return DateTime.Compare (DT1, DT2) > 0;
	}

}

[DynamoDBTable("VRPT")]
public class HeatMap
{
	[DynamoDBHashKey]   // Hash key.
	public string ID { get; set; }
	[DynamoDBProperty("User")]
	public string User { get; set; }
	[DynamoDBProperty("Time")]
	public string Time { get; set; }
	[DynamoDBProperty("GameType")]
	public string GameType { get; set; }
	[DynamoDBProperty("Difficulty")]
	public List<int> Difficulty { get; set; }
	[DynamoDBProperty("Spawned")]
	public List<List<int>> Spawned { get; set; }
	[DynamoDBProperty("Collected")]
	public List<List<int>> Collect { get; set; }
}

[DynamoDBTable("VRPT")]
public class RealData
{
	[DynamoDBHashKey]   // Hash key.
	public string ID { get; set; }
	[DynamoDBProperty("User")]
	public string User { get; set; }
	[DynamoDBProperty("Time")]
	public string Time { get; set; }
	[DynamoDBProperty("GameType")]
	public string GameType { get; set; }
	// [DynamoDBProperty("ObjectScoreArray")]
	// public List<int> ObjectScoreArray { get; set; }
	[DynamoDBProperty("ObjectDifficultyArray")]
	public List<int> ObjectDifficultyArray { get; set; }
}