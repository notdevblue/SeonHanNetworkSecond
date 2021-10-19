using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPopup : PopUp
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
        registerBtn.onClick.AddListener(() =>
        {
            Regex reg = new Regex(@"^[��-�Ra-zA-Z]{2,3}$");
            
            //   @"^[0-9]{3}-[0-9]{3,4}-[0-9]{4}$";
            if (!reg.IsMatch(nameInput.text))
            {
                Debug.Log("�̸��� �ݵ�� �ѱ� �Ǵ� �������� 2~3���ڿ��� �մϴ�.");
                PopUpManager.instance.OpenPopUp(
                    "alert", "�̸��� �ݵ�� �ѱ� �Ǵ� �������� 2~3���ڿ��� �մϴ�.");
                return;
            }
            
            //��ĭ�� �ִ°�?
            
            //passInput�� passConfirm �� �ٸ��� Debug.Error ��쵵�� �ϰ� => popup���� ��ü

            RegisterVO vo = new RegisterVO(nameInput.text, idInput.text, passInput.text);
            string json = JsonUtility.ToJson(vo);
            NetworkManager.instance.SendPostRequest("register", json, result =>
            {
                //ResponseVO ���·� result�� �Ľ��ؼ�
                // �� ����� true��� ���� Ŭ���� ȸ������â�� ���� ������
                // false��� �� Ŭ���� �󷵸� ������
                ResponseVO vo = JsonUtility.FromJson<ResponseVO>(result);
                if (vo.result)
                {
                    PopUpManager.instance.OpenPopUp("alert", vo.payload, 2);
                }
                else
                {
                    PopUpManager.instance.OpenPopUp("alert", vo.payload);
                }
            });
        });

        closeBtn.onClick.AddListener(() => PopUpManager.instance.ClosePopUp());
    }
}
