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

            Regex reg = new Regex(@"^[a-zA-Z]{4,10}$"); // a ~ z, A ~ Z 만, 2 ~ 3 글짜만
            if(reg.IsMatch(nameInput.text))
            {
                Debug.Log("이름은 반드시 영문 2 ~ 3 글자여야 합니다.");
            }

            reg = new Regex(@"^[ ]$");
            if(reg.IsMatch(passInput.text))
            {
                Debug.Log("비밀번호에 공백을 포함할 수 없습니다.");
            }

            if(nameInput.text.Trim().Length == 0 || idInput.text.Trim().Length == 0 || passInput.text.Trim().Length == 0)
            {
                Debug.LogError("모조리 입력해야 함");
            }
            else if(passInput != passConfirmInput)
            {
                Debug.LogError("비번이 서로 같지 않음");
            }
            else
            {
                RegisterVO vo = new RegisterVO(nameInput.text, idInput.text, passInput.text);

                NetworkManager.instance.SendPostRequest("register", JsonUtility.ToJson(vo), res =>
                {
                    Debug.Log(res);
                });
            }

            

        });

        closeBtn.onClick.AddListener(() => PopupManager.instance.ClosePopup());
    }


}
