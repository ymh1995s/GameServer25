using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

// 매니저 총괄 클래스
public class MasterManager : MonoBehaviour
{
    static MasterManager s_instance;
    static MasterManager Instance { get {return s_instance; } }

    ObjectManager _obj = new ObjectManager();
    NetworkManager _network = new NetworkManager();
    ResourceManager _resource = new ResourceManager();

    public static ObjectManager Object { get { return Instance._obj; } }
    public static NetworkManager Network { get { return Instance._network; } }
    public static ResourceManager Resource { get { return Instance._resource; } }

    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            s_instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
    }

    void Init()
    {
        Screen.SetResolution(640, 480, false);
        Application.targetFrameRate = 60; // 60프레임 고정
        s_instance._network.Init();
    }

    // 메시지 수신한 스레드가 직접 패킷을 처리하면 유니티 스레드 정책상 문제가 생기므로
    // 패킷큐를 활용해 유니티 메인스레드에서 실행되도록 유도
    void Update()
    {
        _network.Update();
    }
}
