using UnityEngine;
using System.Collections;

public class ConnectionClient : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		MaterialiseConnection connection = MaterialiseConnection.Instance;
		
		//Materials example
		yield return StartCoroutine(connection.getMaterials());
		Debug.Log(connection.materials);
		Debug.Log(MiniJSON.Json.Serialize(connection.materials));
		
		//File example
		string fileLocation = "Assets/cube.stl";
		yield return StartCoroutine(connection.uploadFile(fileLocation));
		Debug.Log(connection.modelUrl);
		
		//Prices example
		yield return StartCoroutine(
		connection.getPrices("Test", "035f4772-da8a-400b-8be4-2dd344b28ddb", "bba2bebb-8895-4049-aeb0-ab651cee2597", "1", "10", "10", "10", "1", "6", "CL", "Santiago", "8320000"));
		Debug.Log(connection.prices);
		Debug.Log(MiniJSON.Json.Serialize(connection.prices));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
