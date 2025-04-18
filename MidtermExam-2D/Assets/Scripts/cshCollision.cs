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
        // �浹�� ������Ʈ�� �±װ� "ChickBall"�� ��� ����
        if (collision.gameObject.CompareTag("ChickBall"))
        {
            // �浹�� ������Ʈ���� cshChickBall ������Ʈ�� ã�� division() �Լ��� ����
            collision.gameObject.GetComponent<cshChickBall>().division();
            Destroy(gameObject);
        }

        // �浹�� ������Ʈ�� �±װ� "ChickBallDiv"�� ��� ����
        if (collision.gameObject.CompareTag("ChickBallDiv"))
        {
            // �浹�� ������Ʈ�� �ı�
            Destroy(collision.gameObject);
            // ���� ������Ʈ�� �ı�
            Destroy(gameObject);
        }
    }
}
