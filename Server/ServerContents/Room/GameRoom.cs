using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerContents.Job;
using ServerContents.Object;
using ServerContents.Session;
using ServerCore;
using System;
using System.Threading;

namespace ServerContents.Room
{
    // 컨텐츠 로직을 게임에서 관리한다.
    public class GameRoom : JobSerializer
    {
        public int RoomId { get; set; }

        public int recvPacketCount = 0;
        public int sendPacketCount = 0;

        // 게임룸과 연결된 모든 오브젝트를 딕셔너리로 관리 (ObjectManager에서 생성된 객체 중 이 GameRoom에 있는 객체 관리)
        // TODO 종류가 여러개면 _objects를 나눌 수 있음
        Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

        // 패킷 모아보내기용 List
        List<IMessage> _pendingList = new List<IMessage>();

        // _objects 딕셔너리에 원자성을 보장하기 위해 lock을 추가해줌.
        // 개선의 여지는 있을 듯
        // 현재는 Flush(), Enter(), BroadCast()와 충돌중
        object _lock = new object();

        Random random = new Random();

        public GameRoom()
        {
#if DEBUG
            PrintProceecPacket();
#endif
        }

        public void Init()
        {
            TrapExecute(1000);
        }

        void TrapExecute(int waitms)
        {
            S_Trapexecute trapPkt = new S_Trapexecute();
            // N개의 트랩을 발동시킨다.
            for(int i = 0; i <3; i++)
            {
                trapPkt.TrapNo.Add(random.Next(0, 11)); // 0~10 사이의 정수를 추가
            }
            Broadcast(trapPkt);
            PushAfter(waitms, TrapExecute, waitms);
        }


        // OnConnected 될 때 호출한다.
        public void EnterGame(GameObject gameObject)
        {
            lock (_lock)
            {
                if (gameObject == null)
                    return;

                _objects.Add(gameObject.Id, gameObject);
                gameObject.Room = this;

                // 게임룸에 있는 모든 객체를 입장한 본인에게 전송
                {
                    // S_Enter : 자기 자신의 캐릭터 
                    S_Enter enterPacket = new S_Enter();
                    enterPacket.ObjectInfo = gameObject.Info;
                    gameObject.Session.Send(enterPacket);

                    // S_Spawn : 다른 사람의 캐릭터
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (GameObject o in _objects.Values)
                    {
                        if (gameObject != o)
                            spawnPacket.Objects.Add(o.Info);
                    }

                    gameObject.Session.Send(spawnPacket);
                }

                // 게임룸에 입장한 사실을 다른 클라이언트에게 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.Objects.Add(gameObject.Info);
                    foreach (GameObject p in _objects.Values)
                    {
                        if (p.Id != gameObject.Id)
                            p.Session.Send(spawnPacket);
                    }
                }
            }
        }

        // 관리 대상에서(딕셔너리) 삭제
        public void LeaveGame(int objectId)
        {
            GameObject go = null;
            if (_objects.Remove(objectId, out go) == false)
                return;

            go.Room = null;

            // 본인한테 정보 전송
            {
                S_Leave leavePacket = new S_Leave();
                go.Session.Send(leavePacket);
            }

            // 타인한테 정보 전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.ObjectIds.Add(objectId);
                foreach (var p in _objects.Values)
                {
                    if (p.Id != objectId)
                        p.Session.Send(despawnPacket);
                }
            }
        }

        public void Flush()
        {
            lock (_lock)
            {
                if (_pendingList.Count == 0) return;
                foreach (var s in _objects)
                {
                    SendPacketPlus();
                    s.Value.Session.Send(_pendingList);
                }
                _pendingList.Clear();
            }
        }


        public void HandleMove(GameObject go, C_Move movePacket)
        {
            if (go == null)
                return;

            // 서버에서 관리하기 위해 데이터 반영
            _objects[go.Info.ObjectId].Info.PosInfo = movePacket.PosInfo;

            // 다른 플레이어한테도 알려준다
            S_Move resMovePacket = new S_Move();
            resMovePacket.ObjectId = go.Info.ObjectId;
            resMovePacket.PosInfo = movePacket.PosInfo;
            resMovePacket.AnimInfo = movePacket.AnimInfo;
            RecvPacketPlus();
            Broadcast(resMovePacket);
        }

        public void HandleDie(GameObject go, C_Die diePacket)
        {
            if (go == null)
                return;

            LeaveGame(go.Info.ObjectId);

            go.Info.PosInfo.CurrentPosX = 0;
            go.Info.PosInfo.CurrentPosY = 0;
            go.Info.PosInfo.CurrentPosZ = 0;
            go.Info.PosInfo.DestinationPosX = 0;
            go.Info.PosInfo.DestinationPosY = 0;
            go.Info.PosInfo.DestinationPosZ = 0;

            EnterGame(go);
        }

        // 관리 대상에서(딕셔너리) 찾기
        public GameObject FindObject(Func<GameObject, bool> condition)
        {
            foreach (GameObject o in _objects.Values)
            {
                if (condition.Invoke(o))
                    return o;
            }

            return null;
        }

        // 게임룸에 있는 다른 클라이언트에게 알림
        public void Broadcast(IMessage packet)
        {
            lock (_lock)
            {
                _pendingList.Add(packet);
            }
        }

        #region 패킷 송수신 개수 개략적 확인용
        public void RecvPacketPlus()
        {
            Interlocked.Increment(ref recvPacketCount);
            //recvPacketCount++;
        }

        public void SendPacketPlus()
        {
            Interlocked.Increment(ref sendPacketCount);
            //sendPacketCount++;
        }

        private async void PrintProceecPacket()
        {
            while (true)
            {
                Console.WriteLine($"{RoomId}번 방에서 총{recvPacketCount + sendPacketCount}, recv : {recvPacketCount}개 / send : {sendPacketCount}개을 1초에 처리");

                Interlocked.Exchange(ref recvPacketCount, 0);
                Interlocked.Exchange(ref sendPacketCount, 0);
                //recvPacketCount = 0;
                //sendPacketCount = 0;

                await Task.Delay(1000); // 1초 대기 (비동기적으로 실행)
            }
        }

        #endregion 패킷 송수신 개수 개략적 확인용
    }
}
