using UnityEngine;

public class cshChickGenerator : MonoBehaviour
{
    public GameObject obj; //ChickBallPrefab ����
    public float interval = 3.0f; //������ �Լ��� ȣ��� ���͹�

    void Start()
    {
        //SpawnObj�Լ��� ������ ����� 0.1�� �Ŀ� ȣ��, ���� interval�� ���� ȣ�� �ȴ�.
        InvokeRepeating("SpawnObj", 0.1f, interval);
    }

    //SpawnObj�Լ��� ChickBallPrefab�� �����Ѵ�.
    void SpawnObj()
    {
        Instantiate(obj, transform.position, transform.rotation);
    }
}
