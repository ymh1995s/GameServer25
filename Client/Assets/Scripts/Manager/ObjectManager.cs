using Google.Protobuf.Protocol;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        if (myPlayer)
        {
            GameObject go = MasterManager.Resource.Instantiate("AllyPlayer");
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            MyPlayer = go.GetComponent<MyPlayerController>();
            MyPlayer.Id = info.ObjectId;
            MyPlayer.Stat.Level = info.StatInfo.Level;
            MyPlayer.Stat.Life = info.StatInfo.Life;
            MyPlayer.Stat.Speed = info.StatInfo.Speed;
            MyPlayer.SetDestination(info.PosInfo);
            //MyPlayer.PosInfo = info.PosInfo;
            //MyPlayer.SyncPos();
        }
        else
        {
            GameObject go = MasterManager.Resource.Instantiate("EnemyPlayer");
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            PlayerController pc = go.GetComponent<PlayerController>();
            pc.Id = info.ObjectId;
            pc.Stat.Level = info.StatInfo.Level;
            pc.Stat.Life = info.StatInfo.Life;
            pc.Stat.Speed = info.StatInfo.Speed;
            pc.SetDestination(info.PosInfo);
            //pc.PosInfo = info.PosInfo;
            //pc.SyncPos();
        }
    }

    public void Remove(int id)
    {
        GameObject go = FindById(id);
        if (go == null)
            return;

        Object.Destroy(go); // Unity 메인 스레드에서 오브젝트 삭제하고
        _objects.Remove(id); // 딕셔너리에서 제거한다.
    }

    // 딕셔너리에서 대상 오브젝트를 가져온다.
    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }


    public void Clear()
    {
        foreach (GameObject obj in _objects.Values)
            Object.Destroy(obj); // Unity 메인 스레드에서 오브젝트 삭제하고

        _objects.Clear(); // 딕셔너리 클리어
        MyPlayer = null;
    }

}
