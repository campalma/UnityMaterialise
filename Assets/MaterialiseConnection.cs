using UnityEngine;
using System.Collections;

public class MaterialiseConnection : MonoBehaviour {

 private static MaterialiseConnection m_instance;
        
    public static MaterialiseConnection Instance{
            get{
                    if(m_instance == null)
                            m_instance = new MaterialiseConnection();
                    return m_instance;
            }
            
    }
    
    void Awake(){
            m_instance = this;
    }
	
	public IEnumerator materials(){
		Debug.Log("Materials");
		HTTP.Request materialsRequest = new HTTP.Request("GET", "http://i.materialise.com/web-api/materials?user=camilo@thinkerthing.com");
		materialsRequest.SetHeader("Accept", "application/json");
        materialsRequest.SetHeader("Content-type", "application/x-www-form-urlencoded");
		materialsRequest.Send();
		
		while(!materialsRequest.isDone) yield return new WaitForEndOfFrame();
                
        if (materialsRequest.exception != null){
			Debug.LogError(materialsRequest.exception.ToString());
		}
                 
        else{
			Debug.Log(materialsRequest.response.Text);
		}
	}
}
