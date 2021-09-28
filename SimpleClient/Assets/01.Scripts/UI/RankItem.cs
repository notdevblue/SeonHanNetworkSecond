using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankItem : MonoBehaviour
{
    public Text rankText;
    public Text nameText;
    public Text scoreText;
    public Text msgText;

    public void SetData(int rank, RecordVO vo)
    {
        rankText.text = rank.ToString();
        nameText.text = vo.name;
        scoreText.text = vo.score.ToString();
        msgText.text = vo.msg;
    }
}