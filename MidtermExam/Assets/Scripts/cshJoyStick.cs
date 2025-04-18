using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// IDragHandler, IPointerDownHandler, IPointerUpHandler �������̽��� ���� ��ġ/�巡�� �̺�Ʈ ó��
public class cshJoyStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image imgBG;         // ���̽�ƽ�� ��� �̹���
    private Image imgJoystick;   // ���̽�ƽ�� ��ƽ �̹��� (�����̴� �κ�)
    private Vector3 vInputVector; // ���� �Է� ���� (�÷��̾� �̵� ���� ������ ���)

    // ��ġ & �巡�� ���� �� ȣ��Ǵ� �Լ�
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Joystick >>> OnDrag()");
        Vector2 pos;

        // ��ġ�� ��ġ�� ���̽�ƽ ����� ���� ��ǥ�� ��ȯ
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                imgBG.rectTransform,                     // ����� RectTransform
                eventData.position,                      // ��ġ�� ȭ�� ��ǥ
                eventData.pressEventCamera,              // �ش� ��ġ�� ������ ī�޶�
                out pos))                                // ��ȯ�� ��ǥ ����
        {
            // ��� ũ�⿡ ����� -1 ~ 1 ���̷� ����ȭ
            pos.x = (pos.x / imgBG.rectTransform.sizeDelta.x);
            pos.y = (pos.y / imgBG.rectTransform.sizeDelta.y);

            // �Է� ���� ���� (z�� 0)
            vInputVector = new Vector3(pos.x, pos.y, 0);

            // �ִ� ũ�⸦ ����� ����ȭ�ؼ� �� ���� �ȿ� ����
            vInputVector = (vInputVector.magnitude > 1.0f)
                ? vInputVector.normalized
                : vInputVector;

            // ��ƽ �̹��� ��ġ �̵� (���̽�ƽ �ð��� ����)
            imgJoystick.rectTransform.anchoredPosition = new Vector3(
                vInputVector.x * (imgBG.rectTransform.sizeDelta.x / 2),
                vInputVector.y * (imgBG.rectTransform.sizeDelta.y / 2));
        }
    }

    // ��ġ ���� �� ȣ���
    public void OnPointerDown(PointerEventData eventData)
    {
        // �巡�׿� ������ ó�� ����
        OnDrag(eventData);
    }

    // ��ġ ������ �� ȣ���
    public void OnPointerUp(PointerEventData eventData)
    {
        // �Է� ���� �ʱ�ȭ
        vInputVector = Vector3.zero;
        // ���̽�ƽ ��ƽ ��ġ �ʱ�ȭ (�߾����� ����)
        imgJoystick.rectTransform.anchoredPosition = Vector3.zero;
    }

    void Start()
    {
        imgBG = GetComponent<Image>(); // ���� ������Ʈ�� ��� �̹���
        imgJoystick = transform.GetChild(0).GetComponent<Image>(); // �ڽ� ������Ʈ���� ��ƽ �̹��� ��������
    }

    // �ܺο��� �Է� ���� �޾ư� �� �ֵ��� �����ϴ� �Լ���
    public float GetHorizontalValue()
    {
        return vInputVector.x; // �¿� �Է°� (-1 ~ 1)
    }

    public float GetVerticalValue()
    {
        return vInputVector.y; // ���� �Է°� (-1 ~ 1)
    }
}
