using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public Canvas canvas;
    public GameObject[] Trap;

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

    public void GameClear()
    {
        canvas.gameObject.SetActive(true);
    }
}
