using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerContents.Object;
using ServerContents.Room;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ServerContents.Session
{
    public class ClientSession : PacketSession
    {
        public GameObject MyPlayer { get; set; }

        public int SessionId { get; set; }

        public void Send(IMessage packet)
        {
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4]; // 4 : 헤더(2 바이트) + 패킷종류(2 바이트)
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort)); // ushort : 헤더(2 바이트)
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));      // ushort : 패킷종류(2 바이트)
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);                                // size : 헤더 패킷을 제외한 패킷 데이터 크기 
            Send(new ArraySegment<byte>(sendBuffer));

            Console.WriteLine($"Send {msgName} message to client");
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine( $"client {endPoint} is connected to the server. Here is server" );

            MyPlayer = ObjectManager.Instance.Add();
            {
                MyPlayer.Info.Name = $"Player_{MyPlayer.Info.ObjectId}";
                MyPlayer.Info.PosInfo.CurrentPosX = 1;
                MyPlayer.Info.PosInfo.CurrentPosY = 2;
                MyPlayer.Info.PosInfo.CurrentPosZ = 3;

                MyPlayer.Session = this;
            }

            Console.WriteLine($"{endPoint} Object Added in Dic, Send Enter packet To Server...");

            GameRoom room = RoomManager.Instance.Find(1);
            if(room ==null)
            {
                Console.WriteLine( $"N번 방이 존재하지 않습니다. 클라이언트 접속 종료" );

                // Disconnect 내부에서 원자적으로 확인하고 있으므로 스레드 세이프
                Disconnect();
                return;
            }
            room.Push(room.EnterGame, MyPlayer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"client {endPoint} is disconnected from the server. Here is server");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            Console.WriteLine("Receive message from client");
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine("Send message to client");
        }
    }
}
