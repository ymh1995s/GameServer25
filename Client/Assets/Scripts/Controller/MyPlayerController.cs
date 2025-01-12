using Google.Protobuf.Protocol;
using Unity.VisualScripting;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    float moveSpeed = 5f;

    private void Start()
    {
        playerCamera = Camera.main;  // 메인 카메라를 가져옵니다.
    }

    protected override void Update()
    {
        base.Update();
        MovePlayer3();
        RotatePlayerWithMouse();    // 마우스로 회전
    }

    // 무브 방법3 (패킷)
    private void MovePlayer3()
    {
        Vector3 destinationPos = Vector3.zero;

        // Check for key inputs directly
        if (Input.GetKey(KeyCode.W))
        {
            destinationPos += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            destinationPos += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            destinationPos += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            destinationPos += Vector3.right;
        }

        if (destinationPos != Vector3.zero)
        {
            destinationPos = (destinationPos.normalized * moveSpeed); // 정규화
            destinationPos += transform.position;
        }
        else
        {
            destinationPos = transform.position;
        }

        // TODO : 어떻게 해야 매프레임 패킷을 안 보낼 수 있을까?
        PositionInfo destInfo = new PositionInfo();
        Vector3ToPosInfo(destinationPos, ref destInfo); // 프로퍼티 참조 반환하는게 별로라 그냥 한번 더 바꿔줌

        C_Move movePacket = new C_Move();
        movePacket.PosInfo = destInfo;
        MasterManager.Network.Send(movePacket);
    }

    #region 카메라
    private Camera playerCamera;  // 카메라
    private float rotationSpeed = 1f;  // 회전 속도
    private float pitch = 0f; // X축 회전 (상하)
    private float yaw = 0f;   // Y축 회전 (좌우)

    private void RotatePlayerWithMouse()
    {
        // 마우스 이동에 따라 회전
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Y축 회전 (좌우)
        yaw += mouseX;

        // X축 회전 (상하)
        pitch -= mouseY;

        // 회전 제한 (위아래 각도 제한)
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        // 회전 적용
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);  // 카메라의 상하 회전
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);  // 플레이어의 좌우 회전
    }
    #endregion camera
}
