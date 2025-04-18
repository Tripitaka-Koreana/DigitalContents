using UnityEngine;

// �̺�Ʈ Ÿ���� �����ϴ� Ŭ����
public class Event
{
    public enum TYPE
    {
        NONE = -1,   // �̺�Ʈ ����
        ROCKET = 0,  // ���� ���� �̺�Ʈ
        FIRE,        // �� �̺�Ʈ (���Ŀ� �߰��� �� ����)
        NUM,         // �̺�Ʈ Ÿ�� �� ī��Ʈ��
    };
};

public class EventRoot : MonoBehaviour
{
    // �̺�Ʈ ������Ʈ���� �̺�Ʈ Ÿ���� ��ȯ
    public Event.TYPE getEventType(GameObject event_go)
    {
        Event.TYPE type = Event.TYPE.NONE; // �⺻���� NONE���� ����
        if (event_go != null) // event_go�� null�� �ƴϸ�
        {
            if (event_go.tag == "Rocket") // "Rocket" �±װ� �پ� ������
            {
                type = Event.TYPE.ROCKET; // ROCKET Ÿ������ ����
            }
        }
        return (type); // �̺�Ʈ Ÿ�� ��ȯ
    }

    // Ư�� �������� �־����� ��, �ش� ���������� �̺�Ʈ�� �ߵ���ų �� �ִ��� Ȯ��
    public bool isEventIgnitable(Item.TYPE carried_item, GameObject event_go)
    {
        bool ret = false; // �⺻���� �ߵ� �Ұ���
        Event.TYPE type = Event.TYPE.NONE; // �⺻���� NONE

        if (event_go != null) // event_go�� null�� �ƴϸ�
        {
            type = this.getEventType(event_go); // �̺�Ʈ Ÿ���� ������
        }

        // �̺�Ʈ Ÿ�Կ� ���� ó��
        switch (type)
        {
            case Event.TYPE.ROCKET: // ���� �̺�Ʈ�� ���
                // ö(IRON) �Ǵ� �Ĺ�(PLANT) �������� ������ ������ �̺�Ʈ �ߵ� ����
                if (carried_item == Item.TYPE.IRON || carried_item == Item.TYPE.PLANT)
                {
                    ret = true; // �̺�Ʈ �ߵ� ����
                }
                break;
        }

        return (ret); // �̺�Ʈ �ߵ� ���� ���� ��ȯ
    }
}
