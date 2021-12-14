using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameRoom
    {
        public int SessionId { get; set; }
        public GameRoom Room { get; set; }

        List<ClientSession> _sessions = new List<ClientSession>();

        public void Enter(ClientSession session)
        {
            lock(_lock)
            {
                _sessions.Add(session);

            }
        }

        public void Leave(ClientSession session)
        {
            lock(_lock); //fk
            {
                _sessions.Remove(session);
            }
        }

        public void BroadCast(ClientSession session, string chat)
        {
            ChatBroad chatBroad = new ChatBroad();
            chatBroad.playerId = session.sessionId;
            chatBroad.chat = chat;

            ArraySegment<byte> segment = chatBroad.Write();

            lock(_lock)
            {
                _sessions.ForEach(x => x.Send(segment));
            }
        }
    }
}