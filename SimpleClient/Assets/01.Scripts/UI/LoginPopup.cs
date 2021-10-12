using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPopup : Popup
{
    public Button btnLogin;
    public Button btnClose;

    public InputField idInput;
    public InputField passInput;

    private RectTransform menuPanel;

    protected override void Awake()
    {
        base.Awake();

        menuPanel = GameObject.Find("OuterPanel").GetComponent<RectTransform>();

        btnClose.onClick.AddListener(() => {
            PopupManager.instance.ClosePopup();
        });

        btnLogin.onClick.AddListener(() => {
            LoginVO vo = new LoginVO(idInput.text, passInput.text);
            string payload = JsonUtility.ToJson(vo);
            NetworkManager.instance.SendPostRequest("login", payload, res => {
                ResponseVO vo = JsonUtility.FromJson<ResponseVO>(res);
                if(vo.result)
                {
                    PopupManager.instance.OpenPopUp("alert", "로그인 성공", 2);
                    NetworkManager.instance.SetToken(vo.payload);
                    
                    #if UNITY_EDITOR
                    Debug.Log("로그인 성공");
                    #endif
                }
            });
        });
    }
}
