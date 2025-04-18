using UnityEngine;

public class cshCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트의 태그가 "ChickBall"일 경우 실행
        if (collision.gameObject.CompareTag("ChickBall"))
        {
            // 충돌한 오브젝트에서 cshChickBall 컴포넌트를 찾아 division() 함수를 실행
            collision.gameObject.GetComponent<cshChickBall>().division();
            Destroy(gameObject);
        }

        // 충돌한 오브젝트의 태그가 "ChickBallDiv"일 경우 실행
        if (collision.gameObject.CompareTag("ChickBallDiv"))
        {
            // 충돌한 오브젝트를 파괴
            Destroy(collision.gameObject);
            // 현재 오브젝트도 파괴
            Destroy(gameObject);
        }
    }
}
