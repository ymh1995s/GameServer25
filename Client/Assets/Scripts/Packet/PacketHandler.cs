using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using Unity.VisualScripting;
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
        S_Leave LeavePacket = packet as S_Leave;
        MasterManager.Object.Clear();
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        foreach (int id in despawnPacket.ObjectIds)
        {
            MasterManager.Object.Remove(id);
        }
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

    public static void S_TrapexecuteHandler(PacketSession session, IMessage packet)
    {
        S_Trapexecute spawnPacket = packet as S_Trapexecute;

        for(int i = 0; i < spawnPacket.TrapNo.Count; i++)
        {
            int trapNo = spawnPacket.TrapNo[i];

            // 패킷 유효 범위 검증 연습
            if(trapNo > GameManager.Instance.Trap.Length)
            {
                Debug.Log("패킷 오류. 트랩 범위를 벗어납니다.");
                return;
            }

            Animator anim = null;
            try
            {
                anim = GameManager.Instance.Trap[trapNo].GetComponent<Animator>();
            }
            catch(Exception e )
            {
                return;
            }

            if(anim==null)
            {
                Debug.Log("애니메이션 컴포넌트를 찾지 못함");
                return;
            }
            anim.SetTrigger("Action");
        }
    }
}
