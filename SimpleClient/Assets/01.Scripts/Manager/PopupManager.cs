using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupManager : MonoBehaviour
{
    public Button registerPopupBtn;
    public Transform popupParent;
    
    private CanvasGroup popupCanvasGroup;

    public RegisterPopup registerPopupPrefab;

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
        registerPopupBtn.onClick.AddListener(() => OpenPopUp("register"));
    }

    public void OpenPopUp(string name)
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
            popupDict[name].Open();
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
