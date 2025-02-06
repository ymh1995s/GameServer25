using Google.Protobuf.Protocol;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public int Id { get; set; }

    //public PositionInfo PosInfo { get; set; }
    protected Vector3 destinationPos = new Vector3();
    protected float rotateY=90; // 초기 값 90으로 줘야 정면을 바라보고 시작

    StatInfo _stat = new StatInfo();

    Animator animator;

    #region FSM
    public enum EState
    {
        Idle,
        Run,
        Die
    }

    protected EState _state = EState.Idle;
    public virtual EState State
    {
        get { return _state; }
        set
        {
            _state = value;

            switch (_state)
            {
                case EState.Idle:
                    animator.CrossFade("Idle", 0.1f);
                    break;
                case EState.Run:
                    animator.CrossFade("Run", 0.1f);
                    break;
                case EState.Die:
                    animator.CrossFade("Die", 0.1f);
                    break;
            }
        }
    }
    #endregion FSM - TODO

    public virtual StatInfo Stat
    {
        get { return _stat; } private 
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.Life = value.Life;
            _stat.Speed = value.Speed;
        }
    }

    public float Speed
    {
        get { return Stat.Speed; }
        set { Stat.Speed = value; }
    }

    public int Life
    {
        get { return Stat.Life; }
        set { Stat.Life = value; }
    }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        SyncPos();
    }

    public void SetDestination(S_Move move)
    {
        PosInfoToVector3(move.PosInfo, ref destinationPos);
        rotateY = move.PosInfo.RotateY;
        State = (EState)move.AnimInfo.State;
    }

    public void SetDestination(PositionInfo positionInfo)
    {
        PosInfoToVector3(positionInfo, ref destinationPos);
    }

    public void SyncPosWithoutSmooth()
    {
        transform.position = new Vector3(destinationPos.x, destinationPos.y, destinationPos.z);
    }

    public void SyncPos()
    {
        // destinationPos : S_Move패킷을 통해 갱신된 목표 위치
        float distance = Vector3.Distance(transform.position, destinationPos);

        // 목표지점에 거의 다왔으면 목표 위치로 순간이동
        if(distance < 0.1f)
        {
            transform.position = destinationPos;
        }
        else
        {
            // 스르륵 이동
            transform.position = Vector3.MoveTowards(transform.position, destinationPos, Speed * Time.deltaTime);
        }

        // 부드럽게 회전한 값을 새로운 Quaternion으로 설정
        float currentRotationY = transform.rotation.eulerAngles.y;
        float smoothRotationY = Mathf.LerpAngle(currentRotationY, rotateY, 10 * Time.deltaTime);

        // 부드럽게 회전한 값을 새로운 Quaternion으로 설정
        transform.rotation = Quaternion.Euler(0, smoothRotationY, 0);

        // TODO : 보정 등
    }

    public void FirstSyncPos()
    {
        transform.position = destinationPos;
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    protected void Vector3ToPosInfo(Vector3 vec, ref PositionInfo posInfo)
    {
        posInfo.DestinationPosX = vec.x;
        posInfo.DestinationPosY = vec.y;
        posInfo.DestinationPosZ = vec.z;
    }

    protected void PosInfoToVector3(PositionInfo posInfo, ref Vector3 vec)
    {
        vec.x = posInfo.DestinationPosX;
        vec.y = posInfo.DestinationPosY;
        vec.z = posInfo.DestinationPosZ;
    }
}
