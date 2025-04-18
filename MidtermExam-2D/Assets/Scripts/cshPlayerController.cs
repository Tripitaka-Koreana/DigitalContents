using UnityEngine;

public class cshPlayerController : MonoBehaviour
{
    public float speed = 8f;//Player�� �̵� �ӵ�
    public float moveableRange = 5.5f; // �̵� ������ ����
    public float power = 1000f; // CannonBall�� �߻��ϴ� ��

    // ��ź �߻� ���� �߰�
    public GameObject cannonBall; //Player���� �߻��� CannonBall
    public Transform spawnPoint; //Cannon �߻� ����

    public GameObject cannon;


    void Update()
    {
        //Player �̵� (�̵� ������ movableRange�� ����)
        transform.Translate(Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime, 0, 0);
        transform.position
        = new Vector2(Mathf.Clamp(transform.position.x, -moveableRange, moveableRange), transform.position.y);
        //Mathf.Clamp(value, min, max) : value�� �������� ����

        // ��ź �߻� ��� �߰�  
        if (Input.GetKeyDown(KeyCode.Space))  //�����̽�Ű ��ư �̺�Ʈ ó��
        {
            Shoot(); //Shoot�Լ� ȣ��
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
        //�� cannonBall�� �����Ͽ� newBullet�� �Ҵ�
        GameObject newBullet = Instantiate(cannonBall, spawnPoint.position, Quaternion.identity) as GameObject; //Quaternion: ȸ��
        //newBullet�� Rigidbody2D�� �����Ͽ� AddForce �Լ��� ���������� �߻�
        newBullet.GetComponent<Rigidbody2D>().AddForce(spawnPoint.transform.up * power);
    }


}
