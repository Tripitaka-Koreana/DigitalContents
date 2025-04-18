using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ���� �����
    private GameObject closest_item = null; // ���� ����� ������
    private GameObject carried_item = null; // ���� ��� ���� ������
    public ItemRoot item_root = null; // ������ ���� Ŭ���� ����
    private GameStatus game_status = null; // ���� ���� ���� Ŭ���� ����

    public static float MOVE_SPEED = 7.0f; // �̵� �ӵ�
    public static float MOVE_AREA_RADIUS = 15.0f; // �̵� ������ ������ ���� ����

    private GameObject closest_event = null; // ���� ����� �̺�Ʈ ������Ʈ
    private EventRoot event_root = null; // �̺�Ʈ ���� Ŭ���� ����
    private GameObject rocket_model = null; // ���� �� (ȸ�� ȿ����)

    // Ű �Է� ���� ����ü
    private struct Key
    {
        public bool up;
        public bool down;
        public bool right;
        public bool left;
        public bool pick;   // ZŰ: ������ �ݱ�/������
        public bool action; // XŰ: �̺�Ʈ ����
    };

    // ���� �ӽ� ����
    public enum STEP
    {
        NONE = -1,  // �ƹ� ���� �ƴ�
        MOVE = 0,   // �̵� ��
        REPAIRING,  // ���� ��
        EATING,     // �Ļ� ��
    };

    private Key key; // Ű �Է� ����
    public STEP step = STEP.NONE; // ���� ����
    public STEP next_step = STEP.NONE; // ���� ����
    public float step_timer = 0.0f; // ���� ���� ���� �ð� ������ Ÿ�̸�

    void Start()
    {
        // �ʱ� ���� ����
        this.step = STEP.NONE;
        this.next_step = STEP.MOVE;

        // ���� �� ������Ʈ�� ���� �ʱ�ȭ
        this.item_root = GameObject.Find("GameRoot").GetComponent<ItemRoot>();
        this.event_root = GameObject.Find("GameRoot").GetComponent<EventRoot>();
        this.rocket_model = GameObject.Find("rocket").transform.Find("rocket_model").gameObject;
        this.game_status = GameObject.Find("GameRoot").GetComponent<GameStatus>();
    }

    void Update()
    {
        this.get_input(); // Ű �Է� ó��
        this.step_timer += Time.deltaTime;

        float eat_time = 0.5f;    // �Ļ翡 �ɸ��� �ð�
        float repair_time = 0.5f; // ������ �ɸ��� �ð�

        // ���� ���°� �������� ���� ��� ���� ���¿� ���� ���� ���¸� �Ǵ�
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.MOVE:
                    do
                    {
                        if (!this.key.action) break; // �ൿ Ű ������ �ʾ����� Ż��
                        if (this.closest_event != null)
                        {
                            if (!this.is_event_ignitable()) break;

                            Event.TYPE ignitable_event = this.event_root.getEventType(this.closest_event);
                            switch (ignitable_event)
                            {
                                case Event.TYPE.ROCKET:
                                    // ���� �̺�Ʈ: ���� ���� ��ȯ
                                    this.next_step = STEP.REPAIRING;
                                    break;
                            }
                            break;
                        }

                        // �������� ��� ���� ���� �Ļ� ����
                        if (this.carried_item != null)
                        {
                            Item.TYPE carried_item_type = this.item_root.getItemType(this.carried_item);
                            switch (carried_item_type)
                            {
                                case Item.TYPE.APPLE:
                                case Item.TYPE.PLANT:
                                    this.next_step = STEP.EATING; // �Ա� ���·� ��ȯ
                                    break;
                            }
                        }
                    } while (false);
                    break;

                case STEP.EATING:
                    // ���� �ð� ������ �ٽ� MOVE ���·� ��ȯ
                    if (this.step_timer > eat_time)
                    {
                        this.next_step = STEP.MOVE;
                    }
                    break;

                case STEP.REPAIRING:
                    // ���� �ð� ������ �ٽ� MOVE ���·� ��ȯ
                    if (this.step_timer > repair_time)
                    {
                        this.next_step = STEP.MOVE;
                    }
                    break;
            }
        }

        // ���� ���� ó��
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
                        // ������ ���� �� ������ ����
                        this.game_status.addSatiety(this.item_root.getRegainSatiety(this.carried_item));
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                    }
                    break;

                case STEP.REPAIRING:
                    if (this.carried_item != null)
                    {
                        // ������ ���� �� ������ ����
                        this.game_status.addRepairment(this.item_root.getGainRepairment(this.carried_item));
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        this.closest_event = null;
                    }
                    break;
            }
            this.step_timer = 0.0f; // ���� ��ȯ �� Ÿ�̸� �ʱ�ȭ
        }

        // ���� ���¿� ���� ó��
        switch (this.step)
        {
            case STEP.MOVE:
                this.move_control();        // �̵� ó��
                this.pick_or_drop_control(); // ������ �ݱ�/������
                this.game_status.alwaysSatiety(); // �ð� ����� ���� ������ ����
                break;

            case STEP.REPAIRING:
                if (this.carried_item != null)
                {
                    GameObject.Destroy(this.carried_item);
                    this.carried_item = null;
                }
                // ���� ȸ�� �ִϸ��̼�
                this.rocket_model.transform.localRotation *= Quaternion.AngleAxis(360.0f / 10.0f * Time.deltaTime, Vector3.up);
                break;
        }

        // Rigidbody�� �ڴ�� ȸ������ �ʵ��� ȸ�� �ӵ� �ʱ�ȭ
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    // Ű �Է� ó��
    private void get_input()
    {
        this.key.up = Input.GetKey(KeyCode.UpArrow);
        this.key.down = Input.GetKey(KeyCode.DownArrow);
        this.key.right = Input.GetKey(KeyCode.RightArrow);
        this.key.left = Input.GetKey(KeyCode.LeftArrow);
        this.key.pick = Input.GetKeyDown(KeyCode.Z);
        this.key.action = Input.GetKeyDown(KeyCode.X);
    }

    // �̵� ó��
    void move_control()
    {
        Vector3 move_vector = Vector3.zero;
        Vector3 position = this.transform.position;
        bool is_moved = false;

        // ����Ű �Է¿� ���� �̵� ���� ����
        if (this.key.right) { move_vector += Vector3.right; is_moved = true; }
        if (this.key.left) { move_vector += Vector3.left; is_moved = true; }
        if (this.key.up) { move_vector += Vector3.forward; is_moved = true; }
        if (this.key.down) { move_vector += Vector3.back; is_moved = true; }

        move_vector.Normalize(); // ���� ����
        move_vector *= MOVE_SPEED * Time.deltaTime;
        position += move_vector;

        // �̵� ���� ����
        position.y = 0.0f;
        if (position.magnitude > MOVE_AREA_RADIUS)
        {
            position.Normalize();
            position *= MOVE_AREA_RADIUS;
        }

        // ���� ��ġ ����
        position.y = this.transform.position.y;
        this.transform.position = position;

        // �̵� �� ȸ�� ���� ����
        if (move_vector.magnitude > 0.01f)
        {
            Quaternion q = Quaternion.LookRotation(move_vector, Vector3.up);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, q, 0.2f);
        }

        // �̵� �� ������ �Ҹ�
        if (is_moved)
        {
            float consume = this.item_root.getConsumeSatiety(this.carried_item);
            this.game_status.addSatiety(-consume * Time.deltaTime);
        }
    }

    // Ʈ���� ���� �ȿ� �ӹ��� ���� ��� ȣ��Ǵ� �޼ҵ�
    void OnTriggerStay(Collider other)
    {
        GameObject other_go = other.gameObject;

        // ������ ������Ʈ�� ���
        if (other_go.layer == LayerMask.NameToLayer("Item"))
        {
            if (this.closest_item == null)
            {
                // ���� �ָ��ϰ� �ִ� �������� ����, �þ� ���� �ִٸ�
                if (this.is_other_in_view(other_go))
                {
                    this.closest_item = other_go; // �� �������� �ָ� ������� ����
                }
            }
            else if (this.closest_item == other_go)
            {
                // �̹� �ָ� ���� ������������, �þ߸� ��� ���
                if (!this.is_other_in_view(other_go))
                {
                    this.closest_item = null; // �ָ� ����
                }
            }
        }
        // �̺�Ʈ ������Ʈ�� ��� (ex. ����)
        else if (other_go.layer == LayerMask.NameToLayer("Event"))
        {
            if (this.closest_event == null)
            {
                if (this.is_other_in_view(other_go))
                {
                    this.closest_event = other_go; // �ָ� �̺�Ʈ ����
                }
            }
            else if (this.closest_event == other_go)
            {
                if (!this.is_other_in_view(other_go))
                {
                    this.closest_event = null; // �ָ� ����
                }
            }
        }
    }

    // ���� ��� �ִ� �������� �ش� �̺�Ʈ�� ��� �����Ѱ�?
    private bool is_event_ignitable()
    {
        bool ret = false;
        do
        {
            if (this.closest_event == null)
            {
                break; // �ָ� ���� �̺�Ʈ�� ���ٸ� false
            }

            // ��� �ִ� �������� Ÿ���� ������
            Item.TYPE carried_item_type = this.item_root.getItemType(this.carried_item);

            // �����۰� �̺�Ʈ�� ȣȯ���� ������ false
            if (!this.event_root.isEventIgnitable(carried_item_type, this.closest_event))
            {
                break;
            }

            ret = true; // �̺�Ʈ ���� ����
        } while (false);

        return ret;
    }

    // Ʈ���ſ��� ����� �� ȣ��Ǵ� �޼ҵ�
    void OnTriggerExit(Collider other)
    {
        if (this.closest_item == other.gameObject)
        {
            this.closest_item = null; // �����۰� �������� �ָ� ����
        }
    }

    // ������ �ݱ� �� ������ ���� (ZŰ�� ����)
    private void pick_or_drop_control()
    {
        do
        {
            if (!this.key.pick)
            {
                break; // ZŰ�� ������ �ʾ����� �ƹ��͵� ���� ����
            }

            // �������� ��� ���� ���� ��
            if (this.carried_item == null)
            {
                if (this.closest_item == null)
                {
                    break; // ������ �������� ������ �ƹ��͵� ���� ����
                }

                // ������ �ݱ�
                this.carried_item = this.closest_item;

                // �������� �÷��̾��� �ڽ����� ����
                this.carried_item.transform.parent = this.transform;

                // �������� �Ӹ� ���� ��ġ
                this.carried_item.transform.localPosition = Vector3.up * 2.0f;

                // �ָ� ��󿡼� ����
                this.closest_item = null;
            }
            else
            {
                // �������� ��� �ִ� ���: ��������
                this.carried_item.transform.localPosition = Vector3.forward * 1.0f; // �ణ ������
                this.carried_item.transform.parent = null; // �θ� ����

                // �ٽ� �ָ� ������� ����
                this.closest_item = this.carried_item;

                // ��� �ִ� ���� ����
                this.carried_item = null;
            }
        } while (false);
    }

    // ������Ʈ�� �þ� ����(���� 45��)�� �ִ��� �Ǻ�
    private bool is_other_in_view(GameObject other)
    {
        bool ret = false;
        do
        {
            // ���� �ٶ󺸴� ���� ����
            Vector3 heading = this.transform.TransformDirection(Vector3.forward);
            // ��� ������Ʈ������ ���� ����
            Vector3 to_other = other.transform.position - this.transform.position;

            // ���� �����ϰ� ��� �������� ���
            heading.y = 0.0f;
            to_other.y = 0.0f;

            heading.Normalize();
            to_other.Normalize();

            // �� ���� ���� ������ ���� ���� �� (45�� �̳�����)
            float dp = Vector3.Dot(heading, to_other);
            if (dp < Mathf.Cos(45.0f * Mathf.Deg2Rad)) // 45�� ���� �������� Ȯ��
            {
                break;
            }

            ret = true; // �þ� ���� ����
        } while (false);
        return ret;
    }
}
