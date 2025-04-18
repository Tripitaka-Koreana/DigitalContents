using UnityEngine;

public class cshPlayerController : MonoBehaviour
{
    public float speed = 8f;//Player의 이동 속도
    public float moveableRange = 5.5f; // 이동 가능한 범위
    public float power = 1000f; // CannonBall을 발사하는 힘

    // 포탄 발사 변수 추가
    public GameObject cannonBall; //Player에서 발사할 CannonBall
    public Transform spawnPoint; //Cannon 발사 지점

    public GameObject cannon;


    void Update()
    {
        //Player 이동 (이동 범위를 movableRange로 제한)
        transform.Translate(Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime, 0, 0);
        transform.position
        = new Vector2(Mathf.Clamp(transform.position.x, -moveableRange, moveableRange), transform.position.y);
        //Mathf.Clamp(value, min, max) : value를 범위내로 제한

        // 포탄 발사 기능 추가  
        if (Input.GetKeyDown(KeyCode.Space))  //스페이스키 버튼 이벤트 처리
        {
            Shoot(); //Shoot함수 호출
        }

        if (Input.GetKey(KeyCode.Q))
        {
            cannon.transform.Rotate(0, 0, -1);
        }

        if (Input.GetKey(KeyCode.E))
        {
            cannon.transform.Rotate(0, 0, 1);
        }
    }

    void Shoot()
    {
        //새 cannonBall을 생성하여 newBullet에 할당
        GameObject newBullet = Instantiate(cannonBall, spawnPoint.position, Quaternion.identity) as GameObject; //Quaternion: 회전
        //newBullet의 Rigidbody2D를 참조하여 AddForce 함수로 물리적으로 발사
        newBullet.GetComponent<Rigidbody2D>().AddForce(spawnPoint.transform.up * power);
    }


}
