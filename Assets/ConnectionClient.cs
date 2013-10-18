using UnityEngine;
using System.Collections;

public class ConnectionClient : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		MaterialiseConnection connection = MaterialiseConnection.Instance;
		yield return StartCoroutine(connection.materials());
		
		string fileLocation = "Assets/cube.stl";
		yield return StartCoroutine(connection.uploadFile(fileLocation));
//		yield return StartCoroutine(connection.getPrices());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
