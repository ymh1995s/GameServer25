using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "MyPlayer")
        {
            // CLEAR CANVAS ON
            GameManager.Instance.GameClear();
        }
    }
}
