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
		
		//Dictionary<string,string> parameters = new Dictionary<string, string>();
		WWWForm parameters = new WWWForm();
		parameters.AddField("plugin", MaterialiseKeys.PRODUCT_TYPE_ID);
		parameters.AddBinaryData("file", filebytes, "cube.stl", "application/x-octetstream");
		
		string data = MiniJSON.Json.Serialize(parameters);
		HTTP.Request uploadRequest = new HTTP.Request(MaterialiseUrls.UPLOAD_URL, parameters);
		
		Debug.Log("Upload Request Started");
		uploadRequest.OnRedirect = printRedirectUrl;
		uploadRequest.Send();
		
		while(!uploadRequest.isDone) yield return new WaitForEndOfFrame();
		
        if (uploadRequest.exception != null)
			Debug.Log(uploadRequest.exception);
		else
			Application.OpenURL(uploadRequest.response.GetHeader("Location"));
	
		
	}
	
	private void printRedirectUrl(Uri uri){
		Debug.Log(uri.ToString());
	}
	
	public IEnumerator getPrices(){
		Dictionary<string,object> parameters = new Dictionary<string, object>();
		Dictionary<string,string> modelParameters = new Dictionary<string, string>();
		Dictionary<string,string> shippingParameters = new Dictionary<string, string>();
		
		List<Dictionary<string,string>> modelsList = new List<Dictionary<string, string>>();
		
		modelParameters.Add("ModelReference", "Test");
		modelParameters.Add("MaterialID","035f4772-da8a-400b-8be4-2dd344b28ddb");
		modelParameters.Add("FinishID", "bba2bebb-8895-4049-aeb0-ab651cee2597");
		modelParameters.Add("Quantity", "1");
		modelParameters.Add("XDimMm", "10");
		modelParameters.Add("YDimMm", "10");
		modelParameters.Add("ZDimMm", "10");
		modelParameters.Add("VolumeCm3", "1");
		modelParameters.Add("SurfaceCm2", "6");
		
		modelsList.Add(modelParameters);
		
		shippingParameters.Add("CountryCode", "CL");
		shippingParameters.Add("City", "Santiago");
		shippingParameters.Add("ZipCode", "8320000");
		
		parameters.Add("models", modelsList);
		parameters.Add("shipmentInfo", shippingParameters);
		
		string data = MiniJSON.Json.Serialize(parameters);
		Debug.Log(data);
		HTTP.Request priceRequest = new HTTP.Request("POST", MaterialiseUrls.PRICES_URL, UTF8Encoding.UTF8.GetBytes (data));
		priceRequest.SetHeader("Accept", "application/json");
        priceRequest.SetHeader("Content-type", "application/json");
		priceRequest.SetHeader("APICode", MaterialiseKeys.API_CODE);
		
		priceRequest.Send();
		
		while(!priceRequest.isDone) yield return new WaitForEndOfFrame();
		
        if (priceRequest.exception != null)
			Debug.LogError(priceRequest.exception.ToString());
                 
        else
			Debug.Log(priceRequest.response.Text);

	}
	
	public static string toQueryString(Dictionary<string, string> parameters){
    	List<string> a = new List<string>();
        foreach(KeyValuePair<string, string> pair in parameters){                        
        	a.Add(pair.Key+"="+WWW.EscapeURL(pair.Value));
		}
        return "?" + string.Join("&", a.ToArray());
	}
}
