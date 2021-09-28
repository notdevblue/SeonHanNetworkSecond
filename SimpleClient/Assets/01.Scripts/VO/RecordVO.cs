using System;
using System.Text;
using System.Web;
using UnityEngine;

[Serializable]
public class RecordVO
{
    public string name;
    public string msg;  //32��Ʈ  CPU�������� 4����Ʈ�� 64��Ʈ�� 8����Ʈ��
    public int score;

    public RecordVO(string name, string msg, string score)
    {
        this.name = name;
        this.msg = msg;
        this.score = int.Parse(score);
    }

    public override string ToString()
    {
        return $"?name={EncodeStr(name)}&msg={EncodeStr(msg)}&score={score}";
    }

    private string EncodeStr(string str)
    {
        return HttpUtility.UrlEncode(str, Encoding.UTF8);
    }
}
