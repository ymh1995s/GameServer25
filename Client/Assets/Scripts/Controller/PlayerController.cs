using Google.Protobuf.Protocol;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int Id { get; set; }

    public PositionInfo PosInfo { get; set; }

    private void Update()
    {
        SyncPos();
    }

    public void SyncPos()
    {
        transform.position = new Vector3(PosInfo.DestinationPosX, PosInfo.DestinationPosY, PosInfo.DestinationPosZ);
    }

}
