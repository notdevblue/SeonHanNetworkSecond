using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SimpleGet : MonoBehaviour
{
    public Button btnGet;
    private readonly string url = "http://localhost:32000";

    private void Start()
    {
        btnGet.onClick.AddListener(() => StartCoroutine(GetRequest()));
    }

    IEnumerator GetRequest()
    {
        UnityWebRequest req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest(); // 요청 전송


        if(req.result == UnityWebRequest.Result.Success) // result = 200, 400 같은 것들
        {
            string data = req.downloadHandler.text;
            HelloVO vo = JsonUtility.FromJson<HelloVO>(data);
            Debug.Log($"Name: {vo.msg} , msg: {vo.msg}" );
            
            vo.hobbies.ForEach(x => {
                Debug.Log(x.id);
                Debug.Log(x.name);
            });
        }
        else
        {
            Debug.LogError("데이터 통신중 오류 발생");
        }
    }
}
