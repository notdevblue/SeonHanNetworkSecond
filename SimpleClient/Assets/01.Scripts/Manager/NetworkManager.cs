using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    static public NetworkManager instance = null;

    public string baseUrl = "http://localhost:54000";

    private string token = "";

    public void SetToken(string token)
    {
        this.token = token;
    }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("There are more than one NetworkManager running at same scene");
        }
        instance = this;
        token = PlayerPrefs.GetString("token"); // 없으면 null
    }

    public void SendGetRequest(string url, string querystring, System.Action<string> callback)
    {
        StartCoroutine(SendGet($"{baseUrl}/{url}{querystring}", callback));
    }

    public void SendPostRequest(string url, string payloadData, System.Action<string> callback)
    {
        StartCoroutine(SendPost($"{baseUrl}/{url}", payloadData, callback));
    }

    IEnumerator SendGet(string url, System.Action<string> callback)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);

        req.SetRequestHeader("Authorization", "Bearer " + token);

        yield return req.SendWebRequest();

        if(req.result == UnityWebRequest.Result.Success)
        {
            string data = req.downloadHandler.text;
            callback(data);
        }
        else
        {
            callback("{\"result\":false, \"msg\": \"error in communication\"}");
        }
    }

    IEnumerator SendPost(string url, string payload, System.Action<string> callback)
    {
        UnityWebRequest req = UnityWebRequest.Post(url, payload);
        req.SetRequestHeader("Content-Type", "application/json");

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(payload);
        req.uploadHandler = new UploadHandlerRaw(jsonToSend);
        
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string data = req.downloadHandler.text;
            callback(data);
        }
        else
        {
            callback("{\"result\":false, \"msg\": \"error in communication\"}");
        }
    }


}
