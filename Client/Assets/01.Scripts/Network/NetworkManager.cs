using ServerCore;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DummyClient;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _instance;
    public static NetworkManager Instance 
    {
        get {return _instance;}
    }

    ServerSession _session = new ServerSession();

    void Awake()
    {
        if(_instance != null)
            Debug.LogError("Multiple network manager is running");
        _instance = this;
    }

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    void Start()
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 55000);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => _session );
        //3초마다 패킷 전송
        StartCoroutine(CoSendPacket());
    }

    void OnDestroy()
    {
        _session.Disconnect();
    }

    private void Update() {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach(IPacket p in list)
        {
            PacketManager.Instance.HandlePacket(_session, p);
        }
    }


    IEnumerator CoSendPacket()
    {
        while(true)
        {
            yield return new WaitForSeconds(3f);
        }
    }
}
