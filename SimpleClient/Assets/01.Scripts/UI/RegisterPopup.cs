using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPopup : Popup
{
    public Button registerBtn;
    public Button closeBtn;

    public InputField nameInput;
    public InputField idInput;
    public InputField passInput;
    public InputField passConfirmInput;

    protected override void Awake()
    {
        base.Awake();
        registerBtn.onClick.AddListener(() => {

            Regex reg = new Regex(@"^[a-zA-Z]{2,3}$"); // a ~ z, A ~ Z 만, 2 ~ 3 글짜만
            if(!reg.IsMatch(nameInput.text))
            {
                Debug.Log("이름은 반드시 영문 2 ~ 3 글자여야 합니다.");
                PopupManager.instance.OpenPopUp("alert", "이름은 반드시 영문 2 ~ 3글자여야합니다.");
                return;
            }
            else
            {
                RegisterVO vo = new RegisterVO(nameInput.text, idInput.text, passInput.text);
                string json = JsonUtility.ToJson(vo);

                NetworkManager.instance.SendPostRequest("register", json, res =>
                {
                    ResponseVO vo = JsonUtility.FromJson<ResponseVO>(res);

                    Debug.Log(vo.result);
                    Debug.Log(vo.payload);

                    PopupManager.instance.OpenPopUp("alert", vo.result ? "성공적으로 회원가입됬습니다." : "성공적으로 회원가입되지 않았습니다.", vo.result ? 2 : 1);
                });
            }

            

        });

        closeBtn.onClick.AddListener(() => PopupManager.instance.ClosePopup());
    }


}
