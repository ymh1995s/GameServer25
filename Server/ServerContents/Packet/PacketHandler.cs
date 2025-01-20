using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerContents.Object;
using ServerContents.Room;
using ServerContents.Session;
using ServerCore;
using System.Numerics;

internal class PacketHandler
{
    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        //Console.WriteLine($"C_Move ({movePacket.PosInfo.CurrentPosX}, {movePacket.PosInfo.CurrentPosY})");

        GameObject _object = clientSession.MyPlayer;
        if (_object == null)
            return;

        GameRoom room = _object.Room;
        if (room == null)
            return;

        room.Push(room.HandleMove, _object, movePacket);
    }

    public static void C_DieHandler(PacketSession session, IMessage packet)
    {
        C_Die diePacket = packet as C_Die;
        ClientSession clientSession = session as ClientSession;

        GameObject _object = clientSession.MyPlayer;
        if (_object == null)
            return;

        GameRoom room = _object.Room;
        if (room == null)
            return;

        room.Push(room.HandleDie, _object, diePacket);
    }
}