using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        int _sessionId = 0; // TODO. 나중에 삭제하거나 서버로부터 아이디 부여받을 것 
        Dictionary<int, ServerSession> _sessions = new Dictionary<int, ServerSession>();
        object _lock = new object();

        public ServerSession Generate()
        {
            lock (_lock)
            {
                int sessionId = ++_sessionId;

                ServerSession session = new ServerSession();
                _sessions.Add(sessionId, session);

                Console.WriteLine($"Client ID {sessionId} is Connected to ###. Here is Clinent");

                return session;
            }
        }

        // 더미클라이언트 부하 테스트용 브로드캐스트
        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (var session in _sessions)
                {
                    // TODO
                }
            }
        }
    }


}
