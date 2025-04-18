using UnityEngine;

public class cshChickGenerator : MonoBehaviour
{
    public GameObject obj; //ChickBallPrefab 설정
    public float interval = 3.0f; //다음에 함수가 호출될 인터벌

    void Start()
    {
        //SpawnObj함수를 게임이 실행된 0.1초 후에 호출, 이후 interval초 마다 호출 된다.
        InvokeRepeating("SpawnObj", 0.1f, interval);
    }

    //SpawnObj함수는 ChickBallPrefab을 생성한다.
    void SpawnObj()
    {
        Instantiate(obj, transform.position, transform.rotation);
    }
}
