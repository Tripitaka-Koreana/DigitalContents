using UnityEngine;

public class cshPlayerController : MonoBehaviour
{
    // �ִϸ�����: �÷��̾� �ִϸ��̼� �����
    private Animator m_animator;

    // �÷��̾��� �̵� ���� �� ���� ������
    private Vector3 m_velocity;
    private bool m_isGrounded = true;
    private bool m_jumpOn = false;

    // ���̽�ƽ �Է��� �ޱ� ���� Ŭ���� (���� �е�)
    public cshJoyStick sJoystick;

    // �̵� �ӵ�, ���� �� ������ ����
    public float m_moveSpeed = 4.0f;
    public float m_jumpForce = 5.0f;

    // ���� ������ ����ϴ� ������Ʈ
    private cshAttackArea m_attackArea = null;

    // Start�� ���� ���� �� �� ���� �����
    void Start()
    {
        m_animator = GetComponent<Animator>(); // Animator ������Ʈ ��������
        m_attackArea = GetComponentInChildren<cshAttackArea>(); // �ڽ� ������Ʈ���� ���� ���� ������Ʈ ��������
    }

    // ���� ������ ����� �ִ��� Ȯ���ϴ� �Լ�
    public bool CanAttack()
    {
        return 0 < m_attackArea.colliders.Count;
    }

    // �� �����Ӹ��� ȣ���
    void Update()
    {
        PlayerMove(); // �̵� ó��
        m_animator.SetBool("Jump", !m_isGrounded); // ���� �ִϸ��̼� ����
    }

    // ���� ���� ��ư�� ������ �� ����Ǵ� �Լ�
    public void OnVirtualPadJump()
    {
        if (this == null) { return; }

        // �÷��̾� �Ʒ��� ª�� �Ÿ���ŭ Ray�� ���� ���� ����ִ��� Ȯ��
        const float rayDistance = 0.2f;
        var ray = new Ray(transform.localPosition + new Vector3(0.0f, 0.1f, 0.0f), Vector3.down);
        if (Physics.Raycast(ray, rayDistance))
        {
            m_jumpOn = true; // ���� �÷��� ON (PlayerMove���� ó����)
        }
    }

    // ������ �÷��̾ �����̴� ����
    private void PlayerMove()
    {
        CharacterController controller = GetComponent<CharacterController>();
        float gravity = 20.0f;

        // �� ���� ���� ��쿡�� ���� ����
        if (controller.isGrounded)
        {
            // ���� ���̽�ƽ�� ����/���� �Է°� ��������
            float h = sJoystick.GetHorizontalValue();
            float v = sJoystick.GetVerticalValue();
            m_velocity = new Vector3(h, 0, v).normalized;

            // �̵� �ִϸ��̼� �� ����
            m_animator.SetFloat("Move", m_velocity.magnitude);

            // ���� ��ư�� �������� ���� ����
            if (m_jumpOn)
            {
                m_velocity.y = m_jumpForce;
                m_jumpOn = false;
            }
            // �ȴ� ���̸� ���� ��ȯ
            else if (m_velocity.magnitude > 0.5f)
            {
                transform.LookAt(transform.position + m_velocity);
            }
        }

        // �߷� ����
        m_velocity.y -= gravity * Time.deltaTime;

        // �̵� ó��
        controller.Move(m_velocity * m_moveSpeed * Time.deltaTime);

        // �ٴڿ� ��Ҵ��� ���� ������Ʈ
        m_isGrounded = controller.isGrounded;
    }

    // ���� ���� ��ư�� ������ �� ȣ��Ǵ� �Լ�
    public void OnVirtualPadAttack()
    {
        if (this == null) { return; }

        // ���� �ִϸ��̼� ���
        m_animator.SetTrigger("Attack");

        Vector3 center = Vector3.zero; // Ÿ�� �߽� ��ġ ����
        int cnt = m_attackArea.colliders.Count; // ���� ���� �� ��� ��
        int cntBreak = 0; // �ı��� ������Ʈ ��

        // ���� ���� ���� �ִ� ��� ������Ʈ ó��
        for (int i = 0; i < m_attackArea.colliders.Count; ++i)
        {
            var collider = m_attackArea.colliders[i];
            center += collider.transform.localPosition;

            // �μ��� �� �ִ� ������Ʈ ó��
            var obj = collider.GetComponent<cshBreakableObject>();
            if (obj != null)
            {
                obj.PlayEffect(); // �ı� ����Ʈ ���
                cntBreak++;
            }

            // ��(enemy)�� ��� ������ ó��
            var enemy = collider.GetComponent<cshEnemyController>();
            if (enemy != null)
            {
                enemy.Damage(); // ������ ����
                if (enemy.GetHP() <= 0)
                    m_attackArea.colliders.Clear(); // ���� ������ ����Ʈ �ʱ�ȭ
            }
            else
            {
                // �Ϲ� ������Ʈ��� �׳� �ı�
                Destroy(collider.gameObject);
            }
        }

        // �μ��� �� �ϳ��� ������ ����Ʈ �ʱ�ȭ
        if (cntBreak > 0) m_attackArea.colliders.Clear();

        // ���� ������ Ÿ�� �߽� �������� ȸ��
        center /= cnt;
        center.y = transform.localPosition.y;
        transform.LookAt(center);
    }
}
