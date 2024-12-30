using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient.Session
{
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        int _sessionId = 0; // TODO. 나중에 삭제하거나 서버로부터 아이디 부여받을 것 
        Dictionary<int, ServerSession> _sessions = new Dictionary<int, ServerSession>();
        object _lock = new object();
        Random _rand = new Random();

        public ServerSession Generate()
        {
            lock (_lock)
            {
                int sessionId = ++_sessionId;

                ServerSession session = new ServerSession();
                _sessions.Add(sessionId, session);

                Console.WriteLine($"Client ID {sessionId} is Connected to ###. Here is Client");

                return session;
            }
        }

        public PositionInfo dummyPosition { get; private set; } = new PositionInfo();

        // 더미클라이언트 부하 테스트용 브로드캐스트
        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (var session in _sessions)
                {
                    dummyPosition.DestinationPosX = _rand.Next(-50, 50);
                    dummyPosition.DestinationPosY = _rand.Next(0, 5);
                    dummyPosition.DestinationPosZ = _rand.Next(-50, 50);

                    C_Move dummyMovePacket = new C_Move();
                    dummyMovePacket.PosInfo = dummyPosition;
                    session.Value.Send(dummyMovePacket);
                }
            }
        }
    }


}
