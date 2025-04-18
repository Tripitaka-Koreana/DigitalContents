using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 변수 선언부
    private GameObject closest_item = null; // 가장 가까운 아이템
    private GameObject carried_item = null; // 현재 운반 중인 아이템
    public ItemRoot item_root = null; // 아이템 관리 클래스 참조
    private GameStatus game_status = null; // 게임 상태 관리 클래스 참조

    public static float MOVE_SPEED = 7.0f; // 이동 속도
    public static float MOVE_AREA_RADIUS = 15.0f; // 이동 가능한 반지름 영역 제한

    private GameObject closest_event = null; // 가장 가까운 이벤트 오브젝트
    private EventRoot event_root = null; // 이벤트 관리 클래스 참조
    private GameObject rocket_model = null; // 로켓 모델 (회전 효과용)

    // 키 입력 상태 구조체
    private struct Key
    {
        public bool up;
        public bool down;
        public bool right;
        public bool left;
        public bool pick;   // Z키: 아이템 줍기/버리기
        public bool action; // X키: 이벤트 수행
    };

    // 상태 머신 정의
    public enum STEP
    {
        NONE = -1,  // 아무 상태 아님
        MOVE = 0,   // 이동 중
        REPAIRING,  // 수리 중
        EATING,     // 식사 중
    };

    private Key key; // 키 입력 상태
    public STEP step = STEP.NONE; // 현재 상태
    public STEP next_step = STEP.NONE; // 다음 상태
    public float step_timer = 0.0f; // 현재 상태 유지 시간 측정용 타이머

    void Start()
    {
        // 초기 상태 설정
        this.step = STEP.NONE;
        this.next_step = STEP.MOVE;

        // 게임 내 오브젝트들 참조 초기화
        this.item_root = GameObject.Find("GameRoot").GetComponent<ItemRoot>();
        this.event_root = GameObject.Find("GameRoot").GetComponent<EventRoot>();
        this.rocket_model = GameObject.Find("rocket").transform.Find("rocket_model").gameObject;
        this.game_status = GameObject.Find("GameRoot").GetComponent<GameStatus>();
    }

    void Update()
    {
        this.get_input(); // 키 입력 처리
        this.step_timer += Time.deltaTime;

        float eat_time = 0.5f;    // 식사에 걸리는 시간
        float repair_time = 0.5f; // 수리에 걸리는 시간

        // 다음 상태가 지정되지 않은 경우 현재 상태에 따라 다음 상태를 판단
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.MOVE:
                    do
                    {
                        if (!this.key.action) break; // 행동 키 누르지 않았으면 탈출
                        if (this.closest_event != null)
                        {
                            if (!this.is_event_ignitable()) break;

                            Event.TYPE ignitable_event = this.event_root.getEventType(this.closest_event);
                            switch (ignitable_event)
                            {
                                case Event.TYPE.ROCKET:
                                    // 로켓 이벤트: 수리 모드로 전환
                                    this.next_step = STEP.REPAIRING;
                                    break;
                            }
                            break;
                        }

                        // 아이템을 들고 있을 때만 식사 가능
                        if (this.carried_item != null)
                        {
                            Item.TYPE carried_item_type = this.item_root.getItemType(this.carried_item);
                            switch (carried_item_type)
                            {
                                case Item.TYPE.APPLE:
                                case Item.TYPE.PLANT:
                                    this.next_step = STEP.EATING; // 먹기 상태로 전환
                                    break;
                            }
                        }
                    } while (false);
                    break;

                case STEP.EATING:
                    // 일정 시간 지나면 다시 MOVE 상태로 전환
                    if (this.step_timer > eat_time)
                    {
                        this.next_step = STEP.MOVE;
                    }
                    break;

                case STEP.REPAIRING:
                    // 일정 시간 지나면 다시 MOVE 상태로 전환
                    if (this.step_timer > repair_time)
                    {
                        this.next_step = STEP.MOVE;
                    }
                    break;
            }
        }

        // 상태 전이 처리
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = STEP.NONE;

            switch (this.step)
            {
                case STEP.MOVE:
                    break;

                case STEP.EATING:
                    if (this.carried_item != null)
                    {
                        // 포만감 증가 후 아이템 제거
                        this.game_status.addSatiety(this.item_root.getRegainSatiety(this.carried_item));
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                    }
                    break;

                case STEP.REPAIRING:
                    if (this.carried_item != null)
                    {
                        // 수리도 증가 후 아이템 제거
                        this.game_status.addRepairment(this.item_root.getGainRepairment(this.carried_item));
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        this.closest_event = null;
                    }
                    break;
            }
            this.step_timer = 0.0f; // 상태 전환 후 타이머 초기화
        }

        // 현재 상태에 따른 처리
        switch (this.step)
        {
            case STEP.MOVE:
                this.move_control();        // 이동 처리
                this.pick_or_drop_control(); // 아이템 줍기/버리기
                this.game_status.alwaysSatiety(); // 시간 경과에 따른 포만감 감소
                break;

            case STEP.REPAIRING:
                if (this.carried_item != null)
                {
                    GameObject.Destroy(this.carried_item);
                    this.carried_item = null;
                }
                // 로켓 회전 애니메이션
                this.rocket_model.transform.localRotation *= Quaternion.AngleAxis(360.0f / 10.0f * Time.deltaTime, Vector3.up);
                break;
        }

        // Rigidbody가 멋대로 회전하지 않도록 회전 속도 초기화
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    // 키 입력 처리
    private void get_input()
    {
        this.key.up = Input.GetKey(KeyCode.UpArrow);
        this.key.down = Input.GetKey(KeyCode.DownArrow);
        this.key.right = Input.GetKey(KeyCode.RightArrow);
        this.key.left = Input.GetKey(KeyCode.LeftArrow);
        this.key.pick = Input.GetKeyDown(KeyCode.Z);
        this.key.action = Input.GetKeyDown(KeyCode.X);
    }

    // 이동 처리
    void move_control()
    {
        Vector3 move_vector = Vector3.zero;
        Vector3 position = this.transform.position;
        bool is_moved = false;

        // 방향키 입력에 따라 이동 벡터 설정
        if (this.key.right) { move_vector += Vector3.right; is_moved = true; }
        if (this.key.left) { move_vector += Vector3.left; is_moved = true; }
        if (this.key.up) { move_vector += Vector3.forward; is_moved = true; }
        if (this.key.down) { move_vector += Vector3.back; is_moved = true; }

        move_vector.Normalize(); // 방향 보정
        move_vector *= MOVE_SPEED * Time.deltaTime;
        position += move_vector;

        // 이동 영역 제한
        position.y = 0.0f;
        if (position.magnitude > MOVE_AREA_RADIUS)
        {
            position.Normalize();
            position *= MOVE_AREA_RADIUS;
        }

        // 최종 위치 적용
        position.y = this.transform.position.y;
        this.transform.position = position;

        // 이동 시 회전 방향 보정
        if (move_vector.magnitude > 0.01f)
        {
            Quaternion q = Quaternion.LookRotation(move_vector, Vector3.up);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, q, 0.2f);
        }

        // 이동 중 포만감 소모
        if (is_moved)
        {
            float consume = this.item_root.getConsumeSatiety(this.carried_item);
            this.game_status.addSatiety(-consume * Time.deltaTime);
        }
    }

    // 트리거 영역 안에 머무는 동안 계속 호출되는 메소드
    void OnTriggerStay(Collider other)
    {
        GameObject other_go = other.gameObject;

        // 아이템 오브젝트인 경우
        if (other_go.layer == LayerMask.NameToLayer("Item"))
        {
            if (this.closest_item == null)
            {
                // 아직 주목하고 있는 아이템이 없고, 시야 내에 있다면
                if (this.is_other_in_view(other_go))
                {
                    this.closest_item = other_go; // 이 아이템을 주목 대상으로 설정
                }
            }
            else if (this.closest_item == other_go)
            {
                // 이미 주목 중인 아이템이지만, 시야를 벗어난 경우
                if (!this.is_other_in_view(other_go))
                {
                    this.closest_item = null; // 주목 해제
                }
            }
        }
        // 이벤트 오브젝트인 경우 (ex. 로켓)
        else if (other_go.layer == LayerMask.NameToLayer("Event"))
        {
            if (this.closest_event == null)
            {
                if (this.is_other_in_view(other_go))
                {
                    this.closest_event = other_go; // 주목 이벤트 설정
                }
            }
            else if (this.closest_event == other_go)
            {
                if (!this.is_other_in_view(other_go))
                {
                    this.closest_event = null; // 주목 해제
                }
            }
        }
    }

    // 현재 들고 있는 아이템이 해당 이벤트에 사용 가능한가?
    private bool is_event_ignitable()
    {
        bool ret = false;
        do
        {
            if (this.closest_event == null)
            {
                break; // 주목 중인 이벤트가 없다면 false
            }

            // 들고 있는 아이템의 타입을 가져옴
            Item.TYPE carried_item_type = this.item_root.getItemType(this.carried_item);

            // 아이템과 이벤트가 호환되지 않으면 false
            if (!this.event_root.isEventIgnitable(carried_item_type, this.closest_event))
            {
                break;
            }

            ret = true; // 이벤트 실행 가능
        } while (false);

        return ret;
    }

    // 트리거에서 벗어났을 때 호출되는 메소드
    void OnTriggerExit(Collider other)
    {
        if (this.closest_item == other.gameObject)
        {
            this.closest_item = null; // 아이템과 떨어지면 주목 해제
        }
    }

    // 아이템 줍기 및 버리기 로직 (Z키로 조작)
    private void pick_or_drop_control()
    {
        do
        {
            if (!this.key.pick)
            {
                break; // Z키가 눌리지 않았으면 아무것도 하지 않음
            }

            // 아이템을 들고 있지 않을 때
            if (this.carried_item == null)
            {
                if (this.closest_item == null)
                {
                    break; // 주위에 아이템이 없으면 아무것도 하지 않음
                }

                // 아이템 줍기
                this.carried_item = this.closest_item;

                // 아이템을 플레이어의 자식으로 설정
                this.carried_item.transform.parent = this.transform;

                // 아이템을 머리 위에 배치
                this.carried_item.transform.localPosition = Vector3.up * 2.0f;

                // 주목 대상에서 제거
                this.closest_item = null;
            }
            else
            {
                // 아이템을 들고 있는 경우: 내려놓기
                this.carried_item.transform.localPosition = Vector3.forward * 1.0f; // 약간 앞으로
                this.carried_item.transform.parent = null; // 부모 해제

                // 다시 주목 대상으로 설정
                this.closest_item = this.carried_item;

                // 들고 있는 상태 해제
                this.carried_item = null;
            }
        } while (false);
    }

    // 오브젝트가 시야 범위(정면 45도)에 있는지 판별
    private bool is_other_in_view(GameObject other)
    {
        bool ret = false;
        do
        {
            // 현재 바라보는 방향 벡터
            Vector3 heading = this.transform.TransformDirection(Vector3.forward);
            // 대상 오브젝트까지의 방향 벡터
            Vector3 to_other = other.transform.position - this.transform.position;

            // 높이 무시하고 평면 기준으로 계산
            heading.y = 0.0f;
            to_other.y = 0.0f;

            heading.Normalize();
            to_other.Normalize();

            // 두 벡터 간의 내적을 통해 각도 비교 (45도 이내인지)
            float dp = Vector3.Dot(heading, to_other);
            if (dp < Mathf.Cos(45.0f * Mathf.Deg2Rad)) // 45도 각도 이하인지 확인
            {
                break;
            }

            ret = true; // 시야 내에 있음
        } while (false);
        return ret;
    }
}
