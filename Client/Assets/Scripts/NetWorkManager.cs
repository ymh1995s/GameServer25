using Google.Protobuf.Protocol;
using ServerCore;
using System.Net;
using UnityEngine;

public class NetWorkManager : MonoBehaviour
{
    ServerSession _session = new ServerSession();
    public PositionInfo dummyPosition { get; private set; } = new PositionInfo();

    void Start()
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint,
            () => { return _session; },
            1);


        dummyPosition.DestinationPosX = Random.Range(-50, 50);
        dummyPosition.DestinationPosY = Random.Range(0, 5);
        dummyPosition.DestinationPosZ = Random.Range(-50, 50);

        C_Move dummyMovePacket = new C_Move();
        dummyMovePacket.PosInfo = dummyPosition;
        _session.Send(dummyMovePacket);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
