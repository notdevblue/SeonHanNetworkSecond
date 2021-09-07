using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SimpleImageGet : MonoBehaviour
{
    public Button btnLoad;
    public Button btnLoadSans;
    public Image loadedImage;

    private readonly string url = "http://localhost:32000/image";
    private readonly string urlsans = "http://localhost:32000/wasans";

    

    private void Start()
    {
        btnLoad.onClick.AddListener(() => StartCoroutine(ImageRequest(url)));
        //btnLoadSans.onClick.AddListener(() => StartCoroutine(ImageRequest(urlsans)));
    }

    IEnumerator ImageRequest(string url)
    {
        string path = Directory.GetCurrentDirectory() + "/Assets/youandme.png"; 
        if(File.Exists(path))
        {
            Texture2D t = new Texture2D(1,1);
            byte[] dataBytes = File.ReadAllBytes(path);
            t.LoadImage(dataBytes);
            Rect rect = new Rect(0, 0, t.width, t.height);
            loadedImage.sprite = Sprite.Create(t, rect, new Vector2(0.5f, 0.5f));

            yield break;
        }
        
        UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);

        yield return req.SendWebRequest();

        if(req.result == UnityWebRequest.Result.Success)
        {
            Texture2D t = (req.downloadHandler as DownloadHandlerTexture).texture; // 모든 유니티의 2D 스프라이트는 Texture2D 를 사용해야 함

            Rect rect = new Rect(0, 0, t.width, t.height);
            Sprite s = Sprite.Create(t, rect, new Vector2(0.5f, 0.5f)); // 스프라이트를 하나 만듬. Create(t, 크기, 피봇);
            loadedImage.sprite = s;

            byte[] byteData = t.EncodeToPNG();
            File.WriteAllBytes(path, byteData);
        }
        else
        {
            Debug.LogError("데이터 통신중 오류 발생");
        }
    }
}
