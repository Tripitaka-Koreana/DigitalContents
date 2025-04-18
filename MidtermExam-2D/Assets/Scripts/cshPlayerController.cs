using UnityEngine;

// 이 스크립트는 Player 오브젝트의 이동, 포신 회전, 그리고 CannonBall 발사를 담당합니다.
public class cshPlayerController : MonoBehaviour
{
    public float speed = 8f; // Player 이동 속도
    public float moveableRange = 5.5f; // Player가 이동할 수 있는 범위 (양쪽 제한)
    public float power = 1000f; // 포탄(CannonBall)을 발사할 때 적용할 힘

    // 캐논볼 관련 변수
    public GameObject cannonBall;
    public Transform spawnPoint;  // 포탄이 생성될 위치 

    public GameObject cannon; 

    void Update()
    {
        // 좌우 방향키 또는 A/D 키로 Player 이동
        float moveX = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        transform.Translate(moveX, 0, 0);

        // 이동 제한: 이동 가능한 범위를 벗어나지 않도록 x좌표 제한
        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, -moveableRange, moveableRange),
            transform.position.y
        );
        // Mathf.Clamp(value, min, max): 값이 min과 max 사이로 제한됨

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot(); // Shoot 함수 호출
        }

        if (Input.GetKey(KeyCode.Q))
        {
            cannon.transform.Rotate(0, 0, -1); // z축 기준으로 반시계 방향 회전
        }

        if (Input.GetKey(KeyCode.E))
        {
            cannon.transform.Rotate(0, 0, 1); // z축 기준으로 시계 방향 회전
        }
    }

    // 포탄을 생성하고, 위쪽 방향(spawnPoint.up)으로 힘을 가해서 날림
    void Shoot()
    {
        GameObject newBullet = Instantiate(cannonBall, spawnPoint.position, Quaternion.identity);
        // spawnPoint.up은 포신의 현재 위 방향 (회전된 방향을 반영함)
        newBullet.GetComponent<Rigidbody2D>().AddForce(spawnPoint.up * power);
    }
}
