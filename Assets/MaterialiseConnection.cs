using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		Dictionary<string,string> parameters = new Dictionary<string, string>();
		parameters.Add("user", MaterialiseKeys.EMAIL);
		HTTP.Request materialsRequest = new HTTP.Request("GET", MaterialiseUrls.MATERIALS_URL+toQueryString(parameters));
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
	
	public static string toQueryString(Dictionary<string, string> parameters){
    	List<string> a = new List<string>();
        foreach(KeyValuePair<string, string> pair in parameters){                        
        	a.Add(pair.Key+"="+WWW.EscapeURL(pair.Value));
		}
        return "?" + string.Join("&", a.ToArray());
	}
}
