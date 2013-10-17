using UnityEngine;
using System.Collections;

public class ConnectionClient : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		MaterialiseConnection connection = MaterialiseConnection.Instance;
		yield return StartCoroutine(connection.materials());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
