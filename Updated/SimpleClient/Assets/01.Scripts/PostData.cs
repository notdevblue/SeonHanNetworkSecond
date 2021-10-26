using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PostData : MonoBehaviour
{
    public InputField txtName;
    public InputField txtMsg;
    public InputField txtScore;

    public Button btnSend;

    private readonly string url = "http://localhost:54000/postdata";

    void Start()
    {
        btnSend.onClick.AddListener(() =>
        {
            //StartCoroutine(
            //    SendDataPost(txtName.text, txtMsg.text, txtScore.text));

            //���⼭ ���� �α��εǾ����� �ʴٸ� ���â ���� ���� ����.

            RecordVO vo = new RecordVO(txtName.text, txtMsg.text, txtScore.text);
            string json = JsonUtility.ToJson(vo);
            NetworkManager.instance.SendPostRequest("postdata", json, res =>
            {
                Debug.Log(res);
            });
        });
    }
    
}