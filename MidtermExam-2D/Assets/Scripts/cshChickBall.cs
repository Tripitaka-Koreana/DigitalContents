using UnityEngine;

public class cshChickBall : MonoBehaviour
{
    public GameObject divChickBall;

    public void division()
    {
        GameObject chick = Instantiate(
            divChickBall,                        // 생성할 프리팹
            gameObject.transform.position,       // 현재 오브젝트 위치에서 생성
            Quaternion.identity                  // 회전 없음
        );
        chick.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.5f, 0.5f) * 300.0f); //(오른쪽 위 방향)

        chick = Instantiate(
            divChickBall,
            gameObject.transform.position,
            Quaternion.identity
        );
        chick.GetComponent<Rigidbody2D>().AddForce(new Vector2(-0.5f, 0.5f) * 300.0f); //(왼쪽 위 방향으로 힘을 줌)

        // 원래 ChickBall 오브젝트 제거
        Destroy(gameObject);
    }
}
