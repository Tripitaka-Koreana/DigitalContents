using UnityEngine;

public class cshPlayerController : MonoBehaviour
{
    // 애니메이터: 플레이어 애니메이션 제어용
    private Animator m_animator;

    // 플레이어의 이동 벡터 및 상태 변수들
    private Vector3 m_velocity;
    private bool m_isGrounded = true;
    private bool m_jumpOn = false;

    // 조이스틱 입력을 받기 위한 클래스 (가상 패드)
    public cshJoyStick sJoystick;

    // 이동 속도, 점프 힘 조절용 변수
    public float m_moveSpeed = 4.0f;
    public float m_jumpForce = 5.0f;

    // 공격 범위를 담당하는 컴포넌트
    private cshAttackArea m_attackArea = null;

    // Start는 게임 시작 시 한 번만 실행됨
    void Start()
    {
        m_animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
        m_attackArea = GetComponentInChildren<cshAttackArea>(); // 자식 오브젝트에서 공격 영역 컴포넌트 가져오기
    }

    // 공격 가능한 대상이 있는지 확인하는 함수
    public bool CanAttack()
    {
        return 0 < m_attackArea.colliders.Count;
    }

    // 매 프레임마다 호출됨
    void Update()
    {
        PlayerMove(); // 이동 처리
        m_animator.SetBool("Jump", !m_isGrounded); // 점프 애니메이션 설정
    }

    // 가상 점프 버튼이 눌렸을 때 실행되는 함수
    public void OnVirtualPadJump()
    {
        if (this == null) { return; }

        // 플레이어 아래로 짧은 거리만큼 Ray를 쏴서 땅에 닿아있는지 확인
        const float rayDistance = 0.2f;
        var ray = new Ray(transform.localPosition + new Vector3(0.0f, 0.1f, 0.0f), Vector3.down);
        if (Physics.Raycast(ray, rayDistance))
        {
            m_jumpOn = true; // 점프 플래그 ON (PlayerMove에서 처리됨)
        }
    }

    // 실제로 플레이어가 움직이는 로직
    private void PlayerMove()
    {
        CharacterController controller = GetComponent<CharacterController>();
        float gravity = 20.0f;

        // 땅 위에 있을 경우에만 조작 가능
        if (controller.isGrounded)
        {
            // 가상 조이스틱의 수평/수직 입력값 가져오기
            float h = sJoystick.GetHorizontalValue();
            float v = sJoystick.GetVerticalValue();
            m_velocity = new Vector3(h, 0, v).normalized;

            // 이동 애니메이션 값 설정
            m_animator.SetFloat("Move", m_velocity.magnitude);

            // 점프 버튼이 눌렸으면 위로 점프
            if (m_jumpOn)
            {
                m_velocity.y = m_jumpForce;
                m_jumpOn = false;
            }
            // 걷는 중이면 방향 전환
            else if (m_velocity.magnitude > 0.5f)
            {
                transform.LookAt(transform.position + m_velocity);
            }
        }

        // 중력 적용
        m_velocity.y -= gravity * Time.deltaTime;

        // 이동 처리
        controller.Move(m_velocity * m_moveSpeed * Time.deltaTime);

        // 바닥에 닿았는지 상태 업데이트
        m_isGrounded = controller.isGrounded;
    }

    // 가상 공격 버튼을 눌렀을 때 호출되는 함수
    public void OnVirtualPadAttack()
    {
        if (this == null) { return; }

        // 공격 애니메이션 재생
        m_animator.SetTrigger("Attack");

        Vector3 center = Vector3.zero; // 타겟 중심 위치 계산용
        int cnt = m_attackArea.colliders.Count; // 공격 범위 내 대상 수
        int cntBreak = 0; // 파괴된 오브젝트 수

        // 공격 범위 내에 있는 모든 오브젝트 처리
        for (int i = 0; i < m_attackArea.colliders.Count; ++i)
        {
            var collider = m_attackArea.colliders[i];
            center += collider.transform.localPosition;

            // 부서질 수 있는 오브젝트 처리
            var obj = collider.GetComponent<cshBreakableObject>();
            if (obj != null)
            {
                obj.PlayEffect(); // 파괴 이펙트 재생
                cntBreak++;
            }

            // 적(enemy)일 경우 데미지 처리
            var enemy = collider.GetComponent<cshEnemyController>();
            if (enemy != null)
            {
                enemy.Damage(); // 데미지 입힘
                if (enemy.GetHP() <= 0)
                    m_attackArea.colliders.Clear(); // 적이 죽으면 리스트 초기화
            }
            else
            {
                // 일반 오브젝트라면 그냥 파괴
                Destroy(collider.gameObject);
            }
        }

        // 부서진 게 하나라도 있으면 리스트 초기화
        if (cntBreak > 0) m_attackArea.colliders.Clear();

        // 공격 방향을 타겟 중심 방향으로 회전
        center /= cnt;
        center.y = transform.localPosition.y;
        transform.LookAt(center);
    }
}
