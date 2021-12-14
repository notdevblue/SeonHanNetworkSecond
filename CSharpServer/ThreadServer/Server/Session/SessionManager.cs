using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class SessionManager
    {
        static SessionManager _instance = new SessionManager();
        public static SessionManager Instance { get { return _instance; } }

        int _sessionId = 0; // 이건 증가하면서 부여한다.

        Dictionary<int, ClientSession> _session = new Dictionary<int, ClientSession>();

        object _lock = new object();

        public ClientSession Generate()
        {
            lock(_lock)
            {
                int sessionId = ++_sessionId;

                ClientSession s = new ClientSession();
                s.sessionId = sessionId;
                _session.Add(sessionId, s);
                return s;
            }
        }

        public ClientSession Find(int id)
        {
            ClientSession s = null;
            _session.TryGetValue(id, out s);
            return s;
        }

        public void Remove(ClientSession s)
        {
            lock(_lock)
            {
                _session.Remove(s.sessionId);
            }
        }
    }
}