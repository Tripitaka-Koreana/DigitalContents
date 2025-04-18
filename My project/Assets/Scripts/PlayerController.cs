using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // �����
    private GameObject closest_item = null;
    private GameObject carried_item = null;//! ������� ������.
    public ItemRoot item_root = null;

    public static float MOVE_SPEED = 5.0f;
    public static float MOVE_AREA_RADIUS = 15.0f;

    private GameObject closest_event = null;
    private EventRoot event_root = null;
    private GameObject rocket_model = null;


    private struct Key
    {
        public bool up;
        public bool down;
        public bool right;
        public bool left;
        public bool pick;
        public bool action;
    };

    public enum STEP
    {
        NONE = -1,
        MOVE = 0,
        REPAIRING,
        EATING,
    };

    private Key key;
    public STEP step = STEP.NONE;
    public STEP next_step = STEP.NONE;
    public float step_timer = 0.0f;
    void Start()
    {
        this.step = STEP.NONE;
        this.next_step = STEP.MOVE;

        this.item_root = GameObject.Find("GameRoot").GetComponent<ItemRoot>();
        this.event_root = GameObject.Find("GameRoot").GetComponent<EventRoot>();
        this.rocket_model = GameObject.Find("rocket").transform.Find("rocket_model").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        this.get_input();
        this.step_timer += Time.deltaTime;
        float eat_time = 2.0f;
        float repair_time = 2.0f;

        // ���¸� ��ȭ��Ų��
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.MOVE:
                    do
                    {
                        if (!this.key.action)
                        {
                            break;
                        }
                        if (this.closest_event != null)
                        {
                            if (!this.is_event_ignitable())
                            {
                                break;
                            }
                            Event.TYPE ignitable_event = this.event_root.getEventType(this.closest_event);
                            switch (ignitable_event)
                            {
                                case Event.TYPE.ROCKET:
                                    // �̺�Ʈ�� ������ ROCKET �̸�
                                    // REPAIRING ���·� ����
                                    this.next_step = STEP.REPAIRING;
                                    break;
                            }
                            break;
                        }
                        if (this.carried_item != null)
                        {
                            // ������ �ִ� ������ �Ǻ�
                            Item.TYPE carried_item_type = this.item_root.getItemType(this.carried_item);
                            switch (carried_item_type)
                            {
                                case Item.TYPE.APPLE:
                                case Item.TYPE.PLANT:
                                    // '�Ļ� ��' ���·� ����
                                    this.next_step = STEP.EATING;
                                    break;
                            }
                        }
                    } while (false);
                    break;

                case STEP.EATING:
                    if (this.step_timer > eat_time) // 2�� ���
                    {
                        this.next_step = STEP.MOVE;
                    }
                    break;
                case STEP.REPAIRING:
                    if (this.step_timer > repair_time)
                    {
                        this.next_step = STEP.MOVE;
                    }
                    break;
            }
        }

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
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                    }
                    break;
            }
            this.step_timer = 0.0f;
        }
        switch (this.step)
        {
            case STEP.MOVE:
                this.move_control();
                this.pick_or_drop_control();
                break;
            case STEP.REPAIRING:
                // ���ּ��� ȸ����Ų��.
                if (this.carried_item != null)
                {
                    GameObject.Destroy(this.carried_item);
                    this.carried_item = null;
                }
                this.rocket_model.transform.localRotation *= Quaternion.AngleAxis(360.0f / 10.0f * Time.deltaTime, Vector3.up);
                break;
        }
    }
    private void get_input()
    {
        this.key.up = false;
        this.key.down = false;
        this.key.right = false;
        this.key.left = false;

        this.key.up = Input.GetKey(KeyCode.UpArrow);
        this.key.down = Input.GetKey(KeyCode.DownArrow);
        this.key.right = Input.GetKey(KeyCode.RightArrow);
        this.key.left = Input.GetKey(KeyCode.LeftArrow);

        this.key.pick = Input.GetKeyDown(KeyCode.Z);
        this.key.action = Input.GetKeyDown(KeyCode.X);
    }

    void move_control()
    {
        Vector3 move_vector = Vector3.zero;
        Vector3 position = this.transform.position;
        bool is_moved = false;
        if (this.key.right)
        {
            move_vector += Vector3.right;
            is_moved = true;
        }
        if (this.key.left)
        {
            move_vector += Vector3.left;
            is_moved = true;
        }
        if (this.key.up)
        {
            move_vector += Vector3.forward;
            is_moved = true;
        }
        if (this.key.down)
        {
            move_vector += Vector3.back;
            is_moved = true;
        }

        move_vector.Normalize();
        move_vector *= MOVE_SPEED * Time.deltaTime;
        position += move_vector;
        position.y = 0.0f;

        if (position.magnitude > MOVE_AREA_RADIUS)
        {
            position.Normalize();
            position *= MOVE_AREA_RADIUS;
        }

        position.y = this.transform.position.y;
        this.transform.position = position;
        if (move_vector.magnitude > 0.01f)
        {
            Quaternion q = Quaternion.LookRotation(move_vector, Vector3.up);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, q, 0.1f);
        }
    }

    // Ʈ���ſ� ��Ʈ�� ������ ȣ��Ǵ� �޼ҵ�.
    void OnTriggerStay(Collider other)
    {
        GameObject other_go = other.gameObject;
        if (other_go.layer == LayerMask.NameToLayer("Item"))
        {
            if (this.closest_item == null)
            {
                if (this.is_other_in_view(other_go))
                {
                    // �ָ��ϰ� �ִ� ���� �װ�.
                    this.closest_item = other_go;
                }
            }
            else if (this.closest_item == other_go)
            {
                if (!this.is_other_in_view(other_go))
                {
                    this.closest_item = null;// �ָ����� �ʰ� �ȴ�.
                }
            }
        }
        else if (other_go.layer == LayerMask.NameToLayer("Event"))
        {
            if (this.closest_event == null)
            {
                if (this.is_other_in_view(other_go))
                {
                    this.closest_event = other_go;
                }
            }
            else if (this.closest_event == other_go)
            {
                if (!this.is_other_in_view(other_go))
                {
                    this.closest_event = null;
                }
            }
        }

    }

    private bool is_event_ignitable()
    {
        bool ret = false;
        do
        {
            if (this.closest_event == null)
            {
                break;
            }
            Item.TYPE carried_item_type = this.item_root.getItemType(this.carried_item);
            if (!this.event_root.isEventIgnitable(carried_item_type, this.closest_event))
            {
                break;
            }
            ret = true;
        } while (false);
        return (ret);
    }


    // Ʈ���ŷκ��� ������ ������ ȣ��Ǵ� �޼ҵ�.
    void OnTriggerExit(Collider other)
    {
        if (this.closest_item == other.gameObject)
        {
            this.closest_item = null;
        }
    }
    private void pick_or_drop_control()
    {
        do
        {
            if (!this.key.pick)
            {
                break;
            }
            if (this.carried_item == null)
            {
                // �������� ������ ���� ���� ��.
                // �����̿� �������� ������ �ƹ��͵� ���� �ʴ´�.
                if (this.closest_item == null)
                {
                    break;
                }
                // �ָ����� �������� ��� �ø���.
                this.carried_item = this.closest_item;
                // ��� �ִ� �������� �ڽ��� �ڽ����� ����
                this.carried_item.transform.parent = this.transform;
                // 2.0f ���� ��ġ(�Ӹ� ���� �̵�)
                this.carried_item.transform.localPosition = Vector3.up * 2.0f;
                // �ָ����� �������� ���ش�
                this.closest_item = null;
            }

            else // ��� �ִ� �������� ���� ���
            {
                // ��� �ִ� �������� �న(1.0f) ������ �̵����Ѽ�
                this.carried_item.transform.localPosition = Vector3.forward * 1.0f;
                this.carried_item.transform.parent = null;

                // �ڽļ����� ����
                this.closest_item = this.carried_item;
                // ��� �ִ� �������� ���ش�
                this.carried_item = null;
            }
        } while (false);
    }
    // �ٸ� ������Ʈ(������/�̺�Ʈ ����)�� �ֿ� �� �ִ� ����(�ڽ��� ����)�� �ִ°�?.
    private bool is_other_in_view(GameObject other)
    {
        bool ret = false;
        do
        {
            Vector3 heading = this.transform.TransformDirection(Vector3.forward);
            Vector3 to_other = other.transform.position - this.transform.position;
            heading.y = 0.0f;
            to_other.y = 0.0f;
            heading.Normalize();
            to_other.Normalize();
            // '�ٸ� ������Ʈ�� ����'�� '�ڽ��� ���� ����'�� ���̰� 45�� �̳�?.
            float dp = Vector3.Dot(heading, to_other);
            if (dp < Mathf.Cos(45.0f))
            {
                break;
            }
            ret = true;
        } while (false);
        return (ret);
    }

}


