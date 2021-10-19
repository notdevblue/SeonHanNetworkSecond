using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPopup : PopUp
{
    public Button loginButton;
    public Button closeButton;

    public InputField idInput;
    public InputField passInput;

    public RectTransform menuPanel;

    protected override void Awake()
    {
        base.Awake();

        menuPanel = GameObject.Find("OuterPanel").GetComponent<RectTransform>();

        closeButton.onClick.AddListener(() =>
        {
            PopUpManager.instance.ClosePopUp();
        });

        loginButton.onClick.AddListener(() =>
        {
            //���⼭ id�� pass�� �ùٸ��� �������ڳ� ������ �ִ� �� �˻��ؾ���
            LoginVO vo = new LoginVO(idInput.text, passInput.text);
            string payload = JsonUtility.ToJson(vo);
            NetworkManager.instance.SendPostRequest("login", payload, res =>
            {
                Debug.Log(res);
                ResponseVO vo = JsonUtility.FromJson<ResponseVO>(res);
                if(vo.result)
                {
                    PopUpManager.instance.OpenPopUp("alert", "�α��� ����", 2);
                    NetworkManager.instance.SetToken(vo.payload); //��ū ����
                }
                else
                {
                    PopUpManager.instance.OpenPopUp("alert", vo.payload, 1);
                }
            });
        });
    }
}
