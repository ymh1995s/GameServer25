using Google.Protobuf.Protocol;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int Id { get; set; }

    //public PositionInfo PosInfo { get; set; }
    public float moveSpeed { get; private set; } = 5;
    private Vector3 destinationPos = new Vector3();

    protected virtual void Update()
    {
        SyncPos();
    }

    public void SetDestination(S_Move move)
    {
        PosInfoToVector3(move.PosInfo, ref destinationPos);
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
        float distance = Vector3.Distance(transform.position, destinationPos);

        // 목표지점에 거의 다왔으면 목표 위치로 순간이동
        if(distance < 0.1f)
        {
            transform.position = destinationPos;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, destinationPos, moveSpeed * Time.deltaTime);
        }
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
