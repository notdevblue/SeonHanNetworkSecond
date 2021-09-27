using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PostData : MonoBehaviour
{
    public InputField txtMsg;
    public InputField txtName;
    public InputField txtScore;

    public Button btnSend;

    private readonly string url = "http://localhost:54000/postdata";

    void Start()
    {
        btnSend.onClick.AddListener(() => {
            StartCoroutine(SendDataPost(txtName.text, txtMsg.text, txtScore.text));
        });
    }

    IEnumerator SendDataPost(string name,  string msg, string score)
    {
        RecordVO vo = new RecordVO(name, msg, score);
        string json = JsonUtility.ToJson(vo);

        UnityWebRequest req = UnityWebRequest.Post(url, json);
        req.SetRequestHeader("Content-Type", "application/json");

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(jsonToSend);

        // 바이트 raw 데이터로 json 을 변환해서 한 번 더 탑재해줘야 정상작동

        yield return req.SendWebRequest(); // 요청 전송
        
        if(req.result == UnityWebRequest.Result.Success)
        {
            string data = req.downloadHandler.text;
            Debug.Log(data);
        }
        else
        {
            Debug.LogError("데이터 통신중 요류 발생");
        }

    }
}
