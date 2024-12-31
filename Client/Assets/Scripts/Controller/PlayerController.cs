using Google.Protobuf.Protocol;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int Id { get; set; }

    public PositionInfo PosInfo { get; set; }

    public void SyncPos()
    {
        transform.position = new Vector3(PosInfo.CurrentPosX, PosInfo.CurrentPosY, PosInfo.CurrentPosZ);
    }

}
