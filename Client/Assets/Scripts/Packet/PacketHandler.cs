using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using UnityEngine;

public class PacketHandler
{
    public static void S_EnterHandler(PacketSession session, IMessage packet)
    {
        S_Enter enterPacket = packet as S_Enter;
        MasterManager.Object.Add(enterPacket.ObjectInfo, myPlayer: true);
    }

    public static void S_LeaveHandler(PacketSession session, IMessage packet)
    {
        // TODO 관리 대상에서 제거

        S_Leave LeavePacket = packet as S_Leave;
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;

        GameObject go = MasterManager.Object.FindById(movePacket.ObjectId);
        if (go == null)
            return;

        PlayerController pc = go.GetComponent<PlayerController>();
        if (pc == null)
            return;

        pc.SetDestination(movePacket);
        //pc.PosInfo = movePacket.PosInfo;
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;
        foreach (ObjectInfo obj in spawnPacket.Objects)
        {
            MasterManager.Object.Add(obj, myPlayer: false);
        }
    }
}
