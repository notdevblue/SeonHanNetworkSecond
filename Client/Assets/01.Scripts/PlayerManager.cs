using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    private MyPlayer _myPlayer;
    Dictionary<int, Player> _players = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; } = new PlayerManager();

    public void Add(PlayerList listPacket)
    {
        Object obj = Resources.Load("Player");

        foreach (PlayerList.Player p in listPacket.players)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;
            go.GetComponent<Renderer>().material.SetColor("_Color", new Color(p.r, p.g, p.b));
            

            if (p.isSelf)
            {
                _myPlayer = go.AddComponent<MyPlayer>();
                _myPlayer.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                _myPlayer.PlayerId = p.playerId;
            }
            else
            {
                Player player = go.AddComponent<Player>();
                player.PlayerId = p.playerId;
                player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                _players.Add(p.playerId, player);
            }
        }
    }

    public void Enter(BroadcastEnterGame packet)
    {
        if (packet.playerId != _myPlayer.PlayerId)
        {
            Object obj = Resources.Load("Player");
            GameObject go = Object.Instantiate(obj) as GameObject;

            Player player = go.AddComponent<Player>();
            player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);

            _players.Add(packet.playerId, player);
        }

        _myPlayer.GetComponent<Renderer>().material.SetColor("_Color", new Color(packet.r, packet.g, packet.b));

    }

    public void Leave(BroadcastLeaveGame packet)
    {
        if(_myPlayer.PlayerId == packet.playerId)
        {
            GameObject.Destroy(_myPlayer.gameObject);
            _myPlayer = null;
        }
        else
        {
            Player player = null;
            if(_players.TryGetValue(packet.playerId, out player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(packet.playerId);
            }
        }
    }

    public void Move(BroadcastMove packet)
    {
        if(_myPlayer.PlayerId == packet.playerId)
        {
            _myPlayer.targetPos = new Vector3(packet.posX, packet.posY, packet.posZ);
        }
        else
        {
            Player player = null;
            if(_players.TryGetValue(packet.playerId, out player))
            {
                player.targetPos = new Vector3(packet.posX, packet.posY, packet.posZ);
            }
        }
    }
}
//ebiccxz