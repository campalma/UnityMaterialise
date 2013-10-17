using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

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
		Dictionary<string,string> parameters = new Dictionary<string, string>();
		parameters.Add("user", MaterialiseKeys.EMAIL);
		HTTP.Request materialsRequest = new HTTP.Request("GET", MaterialiseUrls.MATERIALS_URL+toQueryString(parameters));
		materialsRequest.SetHeader("Accept", "application/json");
        materialsRequest.SetHeader("Content-type", "application/x-www-form-urlencoded");
		Debug.Log("Materials Request Started");
		materialsRequest.Send();
		
		while(!materialsRequest.isDone) yield return new WaitForEndOfFrame();
                
        if (materialsRequest.exception != null){
			Debug.LogError(materialsRequest.exception.ToString());
		}
                 
        else{
			Debug.Log(materialsRequest.response.Text);
		}
	}
	
	public IEnumerator uploadFile(string fileLocation){
		
		FileStream fs = new FileStream(fileLocation, FileMode.Open, FileAccess.Read);
        byte[] filebytes = new byte[fs.Length];
        fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
        string encodedData = Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);
        string urlenc = WWW.EscapeURL(encodedData);
		
		Dictionary<string,string> parameters = new Dictionary<string, string>();		
		parameters.Add("file", urlenc);
		parameters.Add("plugin", MaterialiseKeys.PRODUCT_TYPE_ID);
		
		string data = MiniJSON.Json.Serialize(parameters);
		HTTP.Request uploadRequest = new HTTP.Request("POST", MaterialiseUrls.UPLOAD_URL, UTF8Encoding.UTF8.GetBytes (data));
		uploadRequest.SetHeader("Accept", "application/json");
        uploadRequest.SetHeader("Content-type", "application/x-www-form-urlencoded");
		
		Debug.Log("Upload Request Started");
		uploadRequest.Send();
		
		while(!uploadRequest.isDone) yield return new WaitForEndOfFrame();
		
        if (uploadRequest.exception != null)
			Debug.LogError(uploadRequest.exception.ToString());
                 
        else
			Debug.Log(uploadRequest.response.Text);
		
	}
	
	public static string toQueryString(Dictionary<string, string> parameters){
    	List<string> a = new List<string>();
        foreach(KeyValuePair<string, string> pair in parameters){                        
        	a.Add(pair.Key+"="+WWW.EscapeURL(pair.Value));
		}
        return "?" + string.Join("&", a.ToArray());
	}
}
