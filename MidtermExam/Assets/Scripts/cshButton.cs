using UnityEngine;
using UnityEngine.UI;

public class cshButton : MonoBehaviour
{
    public Button btnJump;         // ���� ��ư (UI���� ����)
    public Button btnAttack;       // ���� ��ư (UI���� ����)
    public cshPlayerController sPlayer; // �÷��̾� ��ũ��Ʈ ���� (Player ������Ʈ ����)

    // ���� ���� �� �����
    void Start()
    {
        // ���� ��ư Ȱ��ȭ
        btnJump.gameObject.SetActive(true);
        // ���� ��ư�� ���� ������ ����
        btnJump.onClick.RemoveAllListeners();
        // ���� ��ư Ŭ�� �� ������ �Լ� ���
        btnJump.onClick.AddListener(OnClickJumpButton);

        // ���� ��ư ��Ȱ��ȭ (�⺻�� �� ����)
        btnAttack.gameObject.SetActive(false);
        // ���� ��ư�� ���� ������ ����
        btnAttack.onClick.RemoveAllListeners();
        // ���� ��ư Ŭ�� �� ������ �Լ� ���
        btnAttack.onClick.AddListener(OnClickAttackButton);
    }

    // �� �����Ӹ��� ��ư ���� ������Ʈ
    void Update()
    {
        UpdateButton();
    }

    // ���� ���� ���ο� ���� ��ư UI ���¸� �����ϴ� �Լ�
    private void UpdateButton()
    {
        bool canAttack = sPlayer.CanAttack(); // ������ �� �ִ� ����� �ִ��� Ȯ��

        btnAttack.gameObject.SetActive(canAttack);   // ���� ���� �� ���� ��ư ON
        btnJump.gameObject.SetActive(!canAttack);    // ���� �Ұ� �� ���� ��ư ON
    }

    // ���� ��ư�� ������ �� ȣ��Ǵ� �Լ�
    private void OnClickJumpButton()
    {
        sPlayer.OnVirtualPadJump(); // �÷��̾� ���� ����
    }

    // ���� ��ư�� ������ �� ȣ��Ǵ� �Լ�
    private void OnClickAttackButton()
    {
        sPlayer.OnVirtualPadAttack(); // �÷��̾� ���� ����
    }
}
