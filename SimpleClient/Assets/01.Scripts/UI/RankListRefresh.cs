using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankListRefresh : MonoBehaviour
{
    public Button refreshBtn;
    public Transform contentView;
    public RankItem rankItemPrefab;

    private void Start()
    {
        refreshBtn.onClick.AddListener(() => {
            NetworkManager.instance.SendGetRequest("list", "", ListGenerate);
        });  
    }

    private void ListGenerate(string json)
    {
        RecordListVO vo = JsonUtility.FromJson<RecordListVO>(json);

        for (int i = contentView.childCount - 1; i >= 0; --i)
        {
            Destroy(contentView.GetChild(i).gameObject);
        }

        for (int i = 0; i < vo.list.Count; ++i)
        {
            RankItem ri = Instantiate(rankItemPrefab, contentView);
            ri.SetData(i + 1, vo.list[i]);
        }

    }
}
