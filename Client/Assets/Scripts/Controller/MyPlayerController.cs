using UnityEngine;

public class MyPlayerController : PlayerController
{
    float moveSpeed = 5f;

    void Update()
    {
        MovePlayer2();
    }

    private void MovePlayer1()
    {
        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal"); // A, D
        float vertical = Input.GetAxis("Vertical"); // W, S

        // Calculate direction relative to the player
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Apply movement
        if (direction.magnitude >= 0.1f)
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);
        }
    }

    // 무브 방법2
    private void MovePlayer2()
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

        // Apply movement
        if (direction != Vector3.zero)
        {
            transform.Translate(direction.normalized * moveSpeed * Time.deltaTime, Space.Self);
        }
    }
}
