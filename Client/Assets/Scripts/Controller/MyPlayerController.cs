using Google.Protobuf.Protocol;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    Vector3 localDestinationPos;
    AnimInfo animInfo =  new AnimInfo();

    void Start()
    {
        playerCamera = Camera.main;  // 메인 카메라를 가져옵니다.
        StartCoroutine(CoSendMovePacket());
    }

    protected override void Update()
    {
        base.Update();
        MovePlayer();
        RotatePlayerWithMouse();    // 마우스로 회전
    }

    private void MovePlayer()
    {
        Vector3 direction = Vector3.zero;

        // Check for key inputs directly
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }

        localDestinationPos = transform.position;

        if (direction != Vector3.zero)
        {
            direction = transform.TransformDirection(direction.normalized); // 플레이어의 로컬 방향 기준으로 변환
            localDestinationPos += direction * Speed; // 이동하려는 위치 계산
            animInfo.State = (int)EState.Run; // Run
        }
       
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            localDestinationPos = transform.position; // 키를 뗐을 때만 자신의 위치로 설정
            animInfo.State = (int)EState.Idle; // Idle

            // 키를 땠을 때는 정보를 바로 송신한다.
            SendMovepacket();
        }
    }

    private IEnumerator CoSendMovePacket()
    {
        while (true)
        {
            SendMovepacket();
            yield return new WaitForSeconds(0.05f); // 0.25초마다 실행
        }
    }

    void SendMovepacket()
    {
        PositionInfo destInfo = new PositionInfo();
        Vector3ToPosInfo(localDestinationPos, ref destInfo); // 프로퍼티 참조 반환하는게 별로라 그냥 한번 더 바꿔줌

        C_Move movePacket = new C_Move();
        movePacket.PosInfo = destInfo;
        movePacket.AnimInfo = animInfo;
        MasterManager.Network.Send(movePacket);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Fog")
        {
            C_Die diePacket = new C_Die();
            diePacket.ObjectId = Id;
            MasterManager.Network.Send(diePacket);
            Debug.Log("Die Handler 송신");
        }
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
