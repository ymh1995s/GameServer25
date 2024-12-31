using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using UnityEngine;

public class PacketHandler
{
    public static void S_EnterHandler(PacketSession session, IMessage packet)
    {
        S_Enter enterPacket = packet as S_Enter;
        MasterManager.Object.Add(enterPacket.Player, myPlayer: true);
    }

    public static void S_LeaveHandler(PacketSession session, IMessage packet)
    {
        // TODO 관리 대상에서 제거

        S_Leave LeavePacket = packet as S_Leave;
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
    }
}
