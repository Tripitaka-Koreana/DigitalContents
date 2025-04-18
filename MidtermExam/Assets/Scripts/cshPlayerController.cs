using UnityEngine;

public class cshPlayerController : MonoBehaviour
{
    private Animator m_animator;
    private Vector3 m_velocity;
    private bool m_isGrounded = true;
    private bool m_jumpOn = false;

    public cshJoyStick sJoystick;
    public float m_moveSpeed = 4.0f;
    public float m_jumpForce = 5.0f;

    private cshAttackArea m_attackArea = null;


    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_attackArea = GetComponentInChildren<cshAttackArea>();
    }

    public bool CanAttack()
    {
        return 0 < m_attackArea.colliders.Count;
    }


    void Update()
    {
        PlayerMove();
        m_animator.SetBool("Jump", !m_isGrounded);
    }

    public void OnVirtualPadJump()
    {

        if (this == null) { return; }
        const float rayDistance = 0.2f;
        var ray = new Ray(transform.localPosition + new Vector3(0.0f, 0.1f, 0.0f), Vector3.down);
        if (Physics.Raycast(ray, rayDistance))
        {
            m_jumpOn = true;
        }
    }

    private void PlayerMove()
    {
        CharacterController controller = GetComponent<CharacterController>();
        float gravity = 20.0f;

        if (controller.isGrounded)
        {
            float h = sJoystick.GetHorizontalValue();
            float v = sJoystick.GetVerticalValue();
            m_velocity = new Vector3(h, 0, v);
            m_velocity = m_velocity.normalized;

            m_animator.SetFloat("Move", m_velocity.magnitude);

            if (m_jumpOn)
            {
                m_velocity.y = m_jumpForce;
                m_jumpOn = false;
            }
            else if (m_velocity.magnitude > 0.5)
            {
                transform.LookAt(transform.position + m_velocity);
            }
        }

        m_velocity.y -= gravity * Time.deltaTime;
        controller.Move(m_velocity * m_moveSpeed * Time.deltaTime);

        m_isGrounded = controller.isGrounded;
    }

    public void OnVirtualPadAttack()
    {
        if (this == null) { return; }

        m_animator.SetTrigger("Attack");

        Vector3 center = Vector3.zero;
        int cnt = m_attackArea.colliders.Count;
        int cntBreak = 0;

        for (int i = 0; i < m_attackArea.colliders.Count; ++i)
        {
            var collider = m_attackArea.colliders[i];
            center += collider.transform.localPosition;

            var obj = collider.GetComponent<cshBreakableObject>();
            if (obj != null)
            {
                obj.PlayEffect();
                cntBreak++;
            }
            var enemy = collider.GetComponent<cshEnemyController>();
            if (enemy != null)
            {
                enemy.Damage();
                if (enemy.GetHP() <= 0) m_attackArea.colliders.Clear();
            }
            else
            {
                Destroy(collider.gameObject);
            }
        }
        if (cntBreak > 0) m_attackArea.colliders.Clear();

        center /= cnt;
        center.y = transform.localPosition.y;
        transform.LookAt(center);
    }

}

