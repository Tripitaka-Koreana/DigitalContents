using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneControl : MonoBehaviour
{
    private GameStatus game_status = null;       // ���� ���¸� �����ϴ� ��ü
    private PlayerController player_control = null;  // �÷��̾��� �ൿ�� �����ϴ� ��ü
    private ItemRoot item_root = null;             // ������ ���� ������ ó���ϴ� ��ü

    // ���� ���� �ð��� ���� (�� ����)
    private float GAME_OVER_TIME = 60.0f;

    // UI ��ҷ� ���� �̹��� (Ÿ�̸ӷ� ���)
    public Image timeImage;

    // Ÿ�̸Ӹ� ǥ���� UI �ؽ�Ʈ
    public TextMeshProUGUI timer;

    // ������ �� ���¸� ��Ÿ���� ������
    public enum STEP
    {
        NONE = -1,   // ���� ����
        PLAY,        // ���� ���� ��
        CLEAR,       // ���� Ŭ����
        GAMEOVER,    // ���� ����
        NUM,         // ������ ��
    };

    public STEP step = STEP.NONE;     // ���� ���� ����
    public STEP next_step = STEP.NONE; // ���� ���� ����
    public float step_timer = 0.0f;   // ���� ���°� ���۵� ���� ����� �ð�
    private float clear_time = 0.0f;  // ���� Ŭ���� �� ����� �ð�

    // �ʱ�ȭ
    void Start()
    {
        // ���� ����, �÷��̾� ����, ������ ���� ��ü�� �ʱ�ȭ
        this.game_status = this.gameObject.GetComponent<GameStatus>();
        this.player_control = GameObject.Find("Player").GetComponent<PlayerController>();
        this.step = STEP.NONE;      // �ʱ� ���´� ����
        this.next_step = STEP.PLAY; // ������ ���� ���·� ����
    }

    // �� �����Ӹ��� ȣ��Ǵ� ������Ʈ �Լ�
    void Update()
    {
        // ��� �ð� ����
        this.step_timer += Time.deltaTime;

        // Ÿ�̸� UI ������Ʈ (�ð��� ������ fillAmount�� �پ��)
        timeImage.fillAmount = (GAME_OVER_TIME - this.step_timer) / GAME_OVER_TIME;

        // ���� ��ȭ ��� ��
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.PLAY: // ���� ���� ����
                    // ������ Ŭ����Ǿ����� Ŭ���� ���·� ����
                    if (this.game_status.isGameClear())
                    {
                        this.next_step = STEP.CLEAR;
                    }
                    // ���� ���� �����̸� ���� ������ ����
                    if (this.game_status.isGameOver())
                    {
                        this.next_step = STEP.GAMEOVER;
                    }
                    // ���� �ð��� �������� ���� ���� ���·� ����
                    if (this.step_timer > GAME_OVER_TIME)
                    {
                        this.next_step = STEP.GAMEOVER;
                    }
                    break;

                case STEP.CLEAR:  // ���� Ŭ���� ����
                case STEP.GAMEOVER: // ���� ���� ����
                    // ���콺 Ŭ�� �� �����
                    if (Input.GetMouseButtonDown(0))
                    {
                        Application.LoadLevel("SampleScene");  // "SampleScene"�� �ε�
                    }
                    break;
            }
        }

        // ���� ��ȭ �� ó��
        while (this.next_step != STEP.NONE)
        {
            // ���� ���¸� �����ϰ�, ��� ���·� ��ȯ
            this.step = this.next_step;
            this.next_step = STEP.NONE;

            // ���ο� ���¿� ���� ó��
            switch (this.step)
            {
                case STEP.CLEAR:  // ���� Ŭ���� ����
                    // �÷��̾� ���� ��Ȱ��ȭ (Ŭ���� �� �� �̻� ������ �� ������)
                    this.player_control.enabled = false;
                    this.clear_time = this.step_timer;  // Ŭ������ �ð� ���
                    break;

                case STEP.GAMEOVER:  // ���� ���� ����
                    // �÷��̾� ���� ��Ȱ��ȭ (���� ���� �� �� �̻� ������ �� ������)
                    this.player_control.enabled = false;
                    break;
            }

            // ���� ���� �� Ÿ�̸� �ʱ�ȭ
            this.step_timer = 0.0f;
        }
    }
}