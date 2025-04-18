using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// IDragHandler, IPointerDownHandler, IPointerUpHandler 인터페이스를 통해 터치/드래그 이벤트 처리
public class cshJoyStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image imgBG;         // 조이스틱의 배경 이미지
    private Image imgJoystick;   // 조이스틱의 스틱 이미지 (움직이는 부분)
    private Vector3 vInputVector; // 최종 입력 벡터 (플레이어 이동 방향 등으로 사용)

    // 터치 & 드래그 중일 때 호출되는 함수
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Joystick >>> OnDrag()");
        Vector2 pos;

        // 터치한 위치를 조이스틱 배경의 로컬 좌표로 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                imgBG.rectTransform,                     // 배경의 RectTransform
                eventData.position,                      // 터치한 화면 좌표
                eventData.pressEventCamera,              // 해당 터치를 감지한 카메라
                out pos))                                // 변환된 좌표 저장
        {
            // 배경 크기에 비례해 -1 ~ 1 사이로 정규화
            pos.x = (pos.x / imgBG.rectTransform.sizeDelta.x);
            pos.y = (pos.y / imgBG.rectTransform.sizeDelta.y);

            // 입력 벡터 구성 (z는 0)
            vInputVector = new Vector3(pos.x, pos.y, 0);

            // 최대 크기를 벗어나면 정규화해서 원 범위 안에 유지
            vInputVector = (vInputVector.magnitude > 1.0f)
                ? vInputVector.normalized
                : vInputVector;

            // 스틱 이미지 위치 이동 (조이스틱 시각적 반응)
            imgJoystick.rectTransform.anchoredPosition = new Vector3(
                vInputVector.x * (imgBG.rectTransform.sizeDelta.x / 2),
                vInputVector.y * (imgBG.rectTransform.sizeDelta.y / 2));
        }
    }

    // 터치 시작 시 호출됨
    public void OnPointerDown(PointerEventData eventData)
    {
        // 드래그와 동일한 처리 수행
        OnDrag(eventData);
    }

    // 터치 끝났을 때 호출됨
    public void OnPointerUp(PointerEventData eventData)
    {
        // 입력 벡터 초기화
        vInputVector = Vector3.zero;
        // 조이스틱 스틱 위치 초기화 (중앙으로 복귀)
        imgJoystick.rectTransform.anchoredPosition = Vector3.zero;
    }

    void Start()
    {
        imgBG = GetComponent<Image>(); // 현재 오브젝트의 배경 이미지
        imgJoystick = transform.GetChild(0).GetComponent<Image>(); // 자식 오브젝트에서 스틱 이미지 가져오기
    }

    // 외부에서 입력 값을 받아갈 수 있도록 제공하는 함수들
    public float GetHorizontalValue()
    {
        return vInputVector.x; // 좌우 입력값 (-1 ~ 1)
    }

    public float GetVerticalValue()
    {
        return vInputVector.y; // 상하 입력값 (-1 ~ 1)
    }
}
