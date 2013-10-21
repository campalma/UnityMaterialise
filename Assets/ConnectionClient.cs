using UnityEngine;
using System.Collections;

public class ConnectionClient : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		MaterialiseConnection connection = MaterialiseConnection.Instance;
		
		//Materials example
		yield return StartCoroutine(connection.getMaterials());
		Debug.Log(connection.materials);
		
		//File example
		string fileLocation = "Assets/cube.stl";
		yield return StartCoroutine(connection.uploadFile(fileLocation));
		Debug.Log(connection.modelUrl);
		
		//Prices example
		yield return StartCoroutine(connection.getPrices());
		Debug.Log(connection.prices);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
