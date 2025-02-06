using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public Canvas clearCanvas;
    public Canvas realTimeCanvas;
    public GameObject[] Trap;

    public int deathCount;
    public float usedTime;

    public Text deathCountText;
    public Text UsedTimeText;

    public Text realTimeDeathCountText;
    public Text realTImeUsedTimeText;

    private void Awake()
    {
        // Singleton 초기화
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 GameManager가 있으면 제거
        }
    }

    private void Start()
    {
        usedTime = Time.time; // 현재 시간을 새로운 시작점으로 설정
    }

    private void Update()
    {
        realTimeDeathCountText.text = "Death : " + deathCount.ToString();

        float elapsedTime = Time.time - usedTime;
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(elapsedTime);
        realTImeUsedTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
    }

    public void GameClear()
    {
        realTimeCanvas.gameObject.SetActive(false);
        clearCanvas.gameObject.SetActive(true);

        float elapsedTime = Time.time - usedTime;
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(elapsedTime);
        UsedTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
        deathCountText.text = deathCount.ToString();
    }

    public void OnClickRetry()
    {
        realTimeCanvas.gameObject.SetActive(true);
        clearCanvas.gameObject.SetActive(false);
        deathCount = 0;
        usedTime = Time.time; // 현재 시간을 새로운 시작점으로 설정

        C_Die diePacket = new C_Die();
        MyPlayerController player = FindFirstObjectByType<MyPlayerController>();
        diePacket.ObjectId = player.Id;
        MasterManager.Network.Send(diePacket);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
