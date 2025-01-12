using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections.Generic;
using System;
using System.Net;
using UnityEngine;

public class NetworkManager
{
    ServerSession _session = new ServerSession();
    //public PositionInfo dummyPosition { get; private set; } = new PositionInfo();

    public void Init()
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


        //dummyPosition.DestinationPosX = Random.Range(-50, 50);
        //dummyPosition.DestinationPosY = Random.Range(0, 5);
        //dummyPosition.DestinationPosZ = Random.Range(-50, 50);

        //C_Move dummyMovePacket = new C_Move();
        //dummyMovePacket.PosInfo = dummyPosition;
        //_session.Send(dummyMovePacket);
    }

    public void Send(IMessage packet)
    {
        _session.Send(packet);
    }

    public void Update()
    {
        List<PacketMessage> list = PacketQueue.Instance.PopAll();
        foreach (PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            if (handler != null)
                handler.Invoke(_session, packet.Message);
        }
    }
}
