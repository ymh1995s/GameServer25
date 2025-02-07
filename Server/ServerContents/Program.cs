using ServerContents.Data;
using ServerContents.Room;
using ServerContents.Session;
using ServerCore;
using System.Net;

namespace ServerContents
{
    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            // JSON 데이터 관리 활용
            ConfigManager.LoadConfig();
            DataManager.LoadData();

            // 인게임 로직은 실행할 방 하나만 판다.
            GameRoom room = RoomManager.Instance.Add(1);

            Console.WriteLine("Server Start!");
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0]; // TODO. IP List 확인하고 맞춰서 서비스
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            // Func.Invoke() (Listener의 _sessionFactory.Invoke();) 에 의해 SessionManager.Instance.Generate() 가 N번 생성됨
            // Delegate인 SessionManager.Instance.Generate()는 여기서 당장 실행 되진 않고 인자로써 넘겨준다.
            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });

            Console.WriteLine($"This Server IP is {endPoint.Address.ToString()}");
            Console.WriteLine("Server Listening...");

            while (true)
            {
                room.Push(room.Flush);
                Thread.Sleep(100);
            }
        }
    }
}
