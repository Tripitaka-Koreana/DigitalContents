using UnityEngine;

public class cshDestroyObj : MonoBehaviour
{
    //������Ʈ�� �����ϴ� ���͹�
    public float deleteTime = 2.0f;

    void Start()
    {
        //������Ʈ�� ������ �� deleteTime ��ŭ �ð��� ����ϸ� ����
        Destroy(gameObject, deleteTime);
    }

}
