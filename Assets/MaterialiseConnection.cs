using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

public class MaterialiseConnection : MonoBehaviour {
	
	//Singleton
 	private static MaterialiseConnection m_instance;
	
	//Responses to queries
	public IList materials;
    public IDictionary prices;
	public string modelUrl;
	
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
	
	public IEnumerator getMaterials(){
		Dictionary<string,string> parameters = new Dictionary<string, string>();
		parameters.Add("user", MaterialiseKeys.EMAIL);
		HTTP.Request materialsRequest = new HTTP.Request("GET", MaterialiseUrls.MATERIALS_URL+toQueryString(parameters));
		materialsRequest.SetHeader("Accept", "application/json");
        materialsRequest.SetHeader("Content-type", "application/x-www-form-urlencoded");
		materialsRequest.Send();
		
		while(!materialsRequest.isDone) yield return new WaitForEndOfFrame();
                
        if (materialsRequest.exception != null)
			Debug.LogError(materialsRequest.exception.ToString());
                 
        else{
			IDictionary response = (IDictionary)MiniJSON.Json.Deserialize(materialsRequest.response.Text);
			this.materials = (IList)response["materials"];
		}
	}
	
	public IEnumerator uploadFile(string fileLocation){
		
		FileStream fs = new FileStream(fileLocation, FileMode.Open, FileAccess.Read);
        byte[] filebytes = new byte[fs.Length];
        fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
        string encodedData = Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);
        string urlenc = WWW.EscapeURL(encodedData);

		WWWForm parameters = new WWWForm();
		parameters.AddField("plugin", MaterialiseKeys.PRODUCT_TYPE_ID);
		parameters.AddBinaryData("file", filebytes, "cube.stl", "application/x-octetstream");
		
		string data = MiniJSON.Json.Serialize(parameters);
		
		HTTP.Request uploadRequest = new HTTP.Request(MaterialiseUrls.UPLOAD_URL, parameters);
		uploadRequest.OnRedirect = onRedirectCallback;
		uploadRequest.Send();
		
		while(!uploadRequest.isDone) yield return new WaitForEndOfFrame();
		
        if (uploadRequest.exception != null)
			Debug.Log(uploadRequest.exception);
		else
			modelUrl = uploadRequest.response.GetHeader("Location");
	}
	
	public IEnumerator getPrices(string modelReference, string materialID, string finishID, string quantity, string xDimMm, string yDimMm, string zDimMm, string volumeCm3, string surfaceCm2, string countryCode, string city, string zipCode){
		Dictionary<string,object> parameters = new Dictionary<string, object>();
		Dictionary<string,string> modelParameters = new Dictionary<string, string>();
		Dictionary<string,string> shippingParameters = new Dictionary<string, string>();
		
		List<Dictionary<string,string>> modelsList = new List<Dictionary<string, string>>();
		
		modelParameters.Add("ModelReference", modelReference);
		modelParameters.Add("MaterialID",materialID);
		modelParameters.Add("FinishID", finishID);
		modelParameters.Add("Quantity", quantity);
		modelParameters.Add("XDimMm", xDimMm);
		modelParameters.Add("YDimMm", yDimMm);
		modelParameters.Add("ZDimMm", zDimMm);
		modelParameters.Add("VolumeCm3", volumeCm3);
		modelParameters.Add("SurfaceCm2", surfaceCm2);
		
		modelsList.Add(modelParameters);
		
		shippingParameters.Add("CountryCode", countryCode);
		shippingParameters.Add("City", city);
		shippingParameters.Add("ZipCode", zipCode);
		
		parameters.Add("models", modelsList);
		parameters.Add("shipmentInfo", shippingParameters);
		
		string data = MiniJSON.Json.Serialize(parameters);
		HTTP.Request priceRequest = new HTTP.Request("POST", MaterialiseUrls.PRICES_URL, UTF8Encoding.UTF8.GetBytes (data));
		priceRequest.SetHeader("Accept", "application/json");
        priceRequest.SetHeader("Content-type", "application/json");
		priceRequest.SetHeader("APICode", MaterialiseKeys.API_CODE);
		
		priceRequest.Send();
		
		while(!priceRequest.isDone) yield return new WaitForEndOfFrame();
		
        if (priceRequest.exception != null)
			Debug.LogError(priceRequest.exception.ToString());
                 
        else{
			prices = (IDictionary)MiniJSON.Json.Deserialize(priceRequest.response.Text);
		}

	}
	
	private void onRedirectCallback(Uri uri){
		;
	}
	
	private string toQueryString(Dictionary<string, string> parameters){
    	List<string> a = new List<string>();
        foreach(KeyValuePair<string, string> pair in parameters){                        
        	a.Add(pair.Key+"="+WWW.EscapeURL(pair.Value));
		}
        return "?" + string.Join("&", a.ToArray());
	}
}
