using TMPro;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
    // ���� �� ����ϴ� ������(�������� 0.0f���� 1.0f ����)
    public static float GAIN_REPARIMENT_IRON = 0.15f; //!< ö�� ������ �� ����ϴ� ������.
    public static float GAIN_REPARIMENT_PLANT = 0.05f; //!< �Ĺ��� ������ �� ����ϴ� ������.

    // �������� ����� �� 1�ʴ� �Ҹ�Ǵ� ü��.
    public static float CONSUME_SATIETY_IRON = 0.15f / 2.0f; //!< ö�� ����� �� �Ҹ�Ǵ� ü��.
    public static float CONSUME_SATIETY_APPLE = 0.1f / 2.0f; //!< ����� ����� �� �Ҹ�Ǵ� ü��.
    public static float CONSUME_SATIETY_PLANT = 0.1f / 2.0f; //!< �Ĺ��� ����� �� �Ҹ�Ǵ� ü��.

    // �������� �Ծ��� �� ȸ���ϴ� ü��.
    public static float REGAIN_SATIETY_APPLE = 0.6f; //!< ����� �Ծ��� �� ȸ���Ǵ� ü��.
    public static float REGAIN_SATIETY_PLANT = 0.2f; //!< �Ĺ��� �Ծ��� �� ȸ���Ǵ� ü��.

    // ���� �ð����� �׻� �Ҹ�Ǵ� ü��
    public static float CONSUME_SATIETY_ALAWAYS = 0.03f; //!< �׻� �Ҹ�Ǵ� ü��.

    // ================================================================ //
    public float repairment = 0.0f; //!< ���� ������(0.0f ~ 1.0f).
    public float satiety = 1.0f; //!< ���� ������(ü��, 0.0f ~ 1.0f).

    // UI �ؽ�Ʈ ��ҵ� (�������� ������ ���¸� ǥ��)
    public TextMeshProUGUI Repairment; //!< ������ UI ǥ��.
    public TextMeshProUGUI Satiety; //!< ������(UI) ǥ��.

    // �� �����Ӹ��� ȣ��Ǵ� Update �޼���
    private void Update()
    {
        // �������� ������ ���� UI�� ������Ʈ
        Repairment.text = "Repairment : " + repairment.ToString("F2");
        Satiety.text = "Satiety : " + satiety.ToString("F2");
    }

    // �������� ������Ű�ų� ���ҽ�Ŵ (0.0f ~ 1.0f ���� ���� ����)
    public void addRepairment(float add)
    {
        this.repairment = Mathf.Clamp01(this.repairment + add); // 0�� 1 ���̷� ������ ����
    }

    // �������� ������Ű�ų� ���ҽ�Ŵ (0.0f ~ 1.0f ���� ���� ����)
    public void addSatiety(float add)
    {
        this.satiety = Mathf.Clamp01(this.satiety + add); // 0�� 1 ���̷� ������ ����
    }

    // ���� Ŭ���� ���� Ȯ�� (�������� 1.0f�̸� Ŭ����)
    public bool isGameClear()
    {
        bool is_clear = false;
        if (this.repairment >= 1.0f) // �������� 1.0f �̻��̸� Ŭ����
        {
            is_clear = true;
        }
        return is_clear;
    }

    // ���� ���� ���� Ȯ�� (�������� 0 �����̸� ���� ����)
    public bool isGameOver()
    {
        bool is_over = false;
        if (this.satiety <= 0.0f) // �������� 0 �����̸� ���� ����
        {
            is_over = true;
        }
        return is_over;
    }

    // �׻� ���� �ð����� �Ҹ�Ǵ� ������
    public void alwaysSatiety()
    {
        this.satiety = Mathf.Clamp01(this.satiety - CONSUME_SATIETY_ALAWAYS * Time.deltaTime); // �׻� ���� ������ ������ ����
    }
}
