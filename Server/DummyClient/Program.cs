﻿using ServerCore;
using System.Net;

namespace DummyClient
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Connector _connector = new Connector();
            ServerSession _session = new ServerSession();

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0]; // TODO. IP List 확인하고 맞춰서 서비스
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            // Func.Invoke() (Connector의 _sessionFactory.Invoke();) 에 의해 SessionManager.Instance.Generate() 가 N번 생성됨
            // Delegate인 SessionManager.Instance.Generate()는 여기서 당장 실행 되진 않고 인자로써 넘겨준다.
            _connector.Connect(endPoint, () => {return SessionManager.Instance.Generate(); }, 5); // 더미 클라이언트 N개 접속

            while (true)
            {
                Thread.Sleep(1);
            }
        }
    }
}