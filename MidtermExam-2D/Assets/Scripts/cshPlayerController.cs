using UnityEngine;

// �� ��ũ��Ʈ�� Player ������Ʈ�� �̵�, ���� ȸ��, �׸��� CannonBall �߻縦 ����մϴ�.
public class cshPlayerController : MonoBehaviour
{
    public float speed = 8f; // Player �̵� �ӵ�
    public float moveableRange = 5.5f; // Player�� �̵��� �� �ִ� ���� (���� ����)
    public float power = 1000f; // ��ź(CannonBall)�� �߻��� �� ������ ��

    // ĳ�� ���� ����
    public GameObject cannonBall;
    public Transform spawnPoint;  // ��ź�� ������ ��ġ 

    public GameObject cannon; 

    void Update()
    {
        // �¿� ����Ű �Ǵ� A/D Ű�� Player �̵�
        float moveX = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        transform.Translate(moveX, 0, 0);

        // �̵� ����: �̵� ������ ������ ����� �ʵ��� x��ǥ ����
        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, -moveableRange, moveableRange),
            transform.position.y
        );
        // Mathf.Clamp(value, min, max): ���� min�� max ���̷� ���ѵ�

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot(); // Shoot �Լ� ȣ��
        }

        if (Input.GetKey(KeyCode.Q))
        {
            cannon.transform.Rotate(0, 0, -1); // z�� �������� �ݽð� ���� ȸ��
        }

        if (Input.GetKey(KeyCode.E))
        {
            cannon.transform.Rotate(0, 0, 1); // z�� �������� �ð� ���� ȸ��
        }
    }

    // ��ź�� �����ϰ�, ���� ����(spawnPoint.up)���� ���� ���ؼ� ����
    void Shoot()
    {
        GameObject newBullet = Instantiate(cannonBall, spawnPoint.position, Quaternion.identity);
        // spawnPoint.up�� ������ ���� �� ���� (ȸ���� ������ �ݿ���)
        newBullet.GetComponent<Rigidbody2D>().AddForce(spawnPoint.up * power);
    }
}
