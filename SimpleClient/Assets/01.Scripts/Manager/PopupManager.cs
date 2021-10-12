using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupManager : MonoBehaviour
{
    public Button registerPopupBtn;
    public Button loginPopupBtn;
    public Transform popupParent;
    
    private CanvasGroup popupCanvasGroup;

    public RegisterPopup registerPopupPrefab;
    public AlertPopup alertPopupPrefab;
    public LoginPopup loginPopupPrefab;

    private Dictionary<string, Popup> popupDict = new Dictionary<string, Popup>();
    private Stack<Popup> popupStack = new Stack<Popup>();

    public static PopupManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("There are more than one PopupManager Running in same scene");
        }
        instance = this;
    }

    private void Start()
    {
        popupCanvasGroup = popupParent.GetComponent<CanvasGroup>();
        if(popupCanvasGroup == null)
        {
            popupCanvasGroup = popupParent.gameObject.AddComponent<CanvasGroup>();
        }

        popupCanvasGroup.alpha = 0;
        popupCanvasGroup.interactable = false;
        popupCanvasGroup.blocksRaycasts = false;

        popupDict.Add("register", Instantiate(registerPopupPrefab, popupParent));
        popupDict.Add("login", Instantiate(loginPopupPrefab, popupParent));
        popupDict.Add("alert", Instantiate(alertPopupPrefab, popupParent));

        registerPopupBtn.onClick.AddListener(() => OpenPopUp("register"));
        loginPopupBtn.onClick.AddListener(() => OpenPopUp("login"));
    }

    public void OpenPopUp(string name, object data = null, int closeCount = 1)
    {
        if(popupStack.Count == 0)
        {
            DOTween.To(() => popupCanvasGroup.alpha, value => popupCanvasGroup.alpha = value, 1.0f, 0.8f).OnComplete(() => {
                popupCanvasGroup.interactable = true;
                popupCanvasGroup.blocksRaycasts = true;
            });
            // 변화시키고자 할 수 있는 값을 가져올 수 있는 메서드 (Getter)
            // (Setter)
            // 목표 값
            // duration
            popupStack.Push(popupDict[name]);
            popupDict[name].Open(data, closeCount);
        }
    }

    public void ClosePopup()
    {
        popupStack.Pop().Close();
        if(popupStack.Count == 0)
        {
            DOTween.To(() => popupCanvasGroup.alpha, value => popupCanvasGroup.alpha = value, 0.0f, 0.8f).OnComplete(() => {
                popupCanvasGroup.interactable = false;
                popupCanvasGroup.blocksRaycasts = false;
            });
        }
    }

}
