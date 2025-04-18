using UnityEngine;

// 이벤트 타입을 정의하는 클래스
public class Event
{
    public enum TYPE
    {
        NONE = -1,   // 이벤트 없음
        ROCKET = 0,  // 로켓 수리 이벤트
        FIRE,        // 불 이벤트 (추후에 추가될 수 있음)
        NUM,         // 이벤트 타입 수 카운트용
    };
};

public class EventRoot : MonoBehaviour
{
    // 이벤트 오브젝트에서 이벤트 타입을 반환
    public Event.TYPE getEventType(GameObject event_go)
    {
        Event.TYPE type = Event.TYPE.NONE; // 기본값은 NONE으로 설정
        if (event_go != null) // event_go가 null이 아니면
        {
            if (event_go.tag == "Rocket") // "Rocket" 태그가 붙어 있으면
            {
                type = Event.TYPE.ROCKET; // ROCKET 타입으로 설정
            }
        }
        return (type); // 이벤트 타입 반환
    }

    // 특정 아이템이 주어졌을 때, 해당 아이템으로 이벤트를 발동시킬 수 있는지 확인
    public bool isEventIgnitable(Item.TYPE carried_item, GameObject event_go)
    {
        bool ret = false; // 기본값은 발동 불가능
        Event.TYPE type = Event.TYPE.NONE; // 기본값은 NONE

        if (event_go != null) // event_go가 null이 아니면
        {
            type = this.getEventType(event_go); // 이벤트 타입을 가져옴
        }

        // 이벤트 타입에 따라 처리
        switch (type)
        {
            case Event.TYPE.ROCKET: // 로켓 이벤트인 경우
                // 철(IRON) 또는 식물(PLANT) 아이템을 가지고 있으면 이벤트 발동 가능
                if (carried_item == Item.TYPE.IRON || carried_item == Item.TYPE.PLANT)
                {
                    ret = true; // 이벤트 발동 가능
                }
                break;
        }

        return (ret); // 이벤트 발동 가능 여부 반환
    }
}
