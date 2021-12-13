using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();

        JobQueue _jobQueue = new JobQueue();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        Random random = new Random();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;

            PlayerList list = new PlayerList();
            _sessions.ForEach(s =>
            {
                list.players.Add(new PlayerList.Player()
                {
                    isSelf = (s.sessionId == session.sessionId),
                    playerId = s.sessionId,
                    posX = s.posX,
                    posY = s.posY,
                    posZ = s.posZ,
                    r = (float)random.NextDouble(),
                    g = (float)random.NextDouble(),
                    b = (float)random.NextDouble()
                });
            });
            session.Send(list.Write());

            BroadcastEnterGame enter = new BroadcastEnterGame();
            enter.playerId = session.sessionId;
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;

            enter.r = (float)random.NextDouble();
            enter.g = (float)random.NextDouble();
            enter.b = (float)random.NextDouble();

            BroadCast(enter.Write());
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);

            BroadcastLeaveGame leave = new BroadcastLeaveGame();
            leave.playerId = session.sessionId;
            BroadCast(leave.Write());
        }

        public void BroadCast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Flush()
        {
            _sessions.ForEach(x => x.Send(_pendingList));
            _pendingList.Clear();
        }

        public void Move(ClientSession session, Move packet)
        {
            session.posX = packet.posX;
            session.posY = packet.posY;
            session.posZ = packet.posZ;

            BroadcastMove bMove = new BroadcastMove()
            {
                playerId = session.sessionId,
                posX = session.posX,
                posY = session.posY,
                posZ = session.posZ,
            };

            BroadCast(bMove.Write());
        }
    }
}
