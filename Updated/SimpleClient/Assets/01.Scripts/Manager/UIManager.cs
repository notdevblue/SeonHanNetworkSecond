using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public RectTransform loginPanel, box1, box2;
    public float transitionTime = 1f;
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("�ټ��� UIManager�� �������Դϴ�.");
        }
        instance = this;
    }

    public void ShowBox1()
    {
        //loginPanel.sizeDelta.y;
        // localposition�� �ǵ帱 ��
        float height = loginPanel.sizeDelta.y;
        box1.DOLocalMoveY(0, transitionTime);
        box2.DOLocalMoveY(-height, transitionTime);
    }

    public void ShowBox2()
    {
        float height = loginPanel.sizeDelta.y;
        box2.DOLocalMoveY(0, transitionTime);
        box1.DOLocalMoveY(height, transitionTime);
    }
}
