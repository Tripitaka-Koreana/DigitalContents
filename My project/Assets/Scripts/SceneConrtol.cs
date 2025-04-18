using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneControl : MonoBehaviour
{
    private GameStatus game_status = null;       // 게임 상태를 관리하는 객체
    private PlayerController player_control = null;  // 플레이어의 행동을 제어하는 객체
    private ItemRoot item_root = null;             // 아이템 관련 로직을 처리하는 객체

    // 게임 오버 시간을 설정 (초 단위)
    private float GAME_OVER_TIME = 60.0f;

    // UI 요소로 사용될 이미지 (타이머로 사용)
    public Image timeImage;

    // 타이머를 표시할 UI 텍스트
    public TextMeshProUGUI timer;

    // 게임의 각 상태를 나타내는 열거형
    public enum STEP
    {
        NONE = -1,   // 상태 없음
        PLAY,        // 게임 진행 중
        CLEAR,       // 게임 클리어
        GAMEOVER,    // 게임 오버
        NUM,         // 상태의 수
    };

    public STEP step = STEP.NONE;     // 현재 게임 상태
    public STEP next_step = STEP.NONE; // 다음 게임 상태
    public float step_timer = 0.0f;   // 현재 상태가 시작된 이후 경과된 시간
    private float clear_time = 0.0f;  // 게임 클리어 시 경과된 시간

    // 초기화
    void Start()
    {
        // 게임 상태, 플레이어 제어, 아이템 관리 객체를 초기화
        this.game_status = this.gameObject.GetComponent<GameStatus>();
        this.player_control = GameObject.Find("Player").GetComponent<PlayerController>();
        this.step = STEP.NONE;      // 초기 상태는 없음
        this.next_step = STEP.PLAY; // 게임을 진행 상태로 설정
    }

    // 매 프레임마다 호출되는 업데이트 함수
    void Update()
    {
        // 경과 시간 갱신
        this.step_timer += Time.deltaTime;

        // 타이머 UI 업데이트 (시간이 지나면 fillAmount가 줄어듦)
        timeImage.fillAmount = (GAME_OVER_TIME - this.step_timer) / GAME_OVER_TIME;

        // 상태 변화 대기 중
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.PLAY: // 게임 진행 상태
                    // 게임이 클리어되었으면 클리어 상태로 변경
                    if (this.game_status.isGameClear())
                    {
                        this.next_step = STEP.CLEAR;
                    }
                    // 게임 오버 상태이면 게임 오버로 변경
                    if (this.game_status.isGameOver())
                    {
                        this.next_step = STEP.GAMEOVER;
                    }
                    // 게임 시간이 끝났으면 게임 오버 상태로 변경
                    if (this.step_timer > GAME_OVER_TIME)
                    {
                        this.next_step = STEP.GAMEOVER;
                    }
                    break;

                case STEP.CLEAR:  // 게임 클리어 상태
                case STEP.GAMEOVER: // 게임 오버 상태
                    // 마우스 클릭 시 재시작
                    if (Input.GetMouseButtonDown(0))
                    {
                        Application.LoadLevel("SampleScene");  // "SampleScene"로 로드
                    }
                    break;
            }
        }

        // 상태 변화 시 처리
        while (this.next_step != STEP.NONE)
        {
            // 현재 상태를 변경하고, 대기 상태로 전환
            this.step = this.next_step;
            this.next_step = STEP.NONE;

            // 새로운 상태에 따른 처리
            switch (this.step)
            {
                case STEP.CLEAR:  // 게임 클리어 상태
                    // 플레이어 제어 비활성화 (클리어 후 더 이상 조작할 수 없도록)
                    this.player_control.enabled = false;
                    this.clear_time = this.step_timer;  // 클리어한 시간 기록
                    break;

                case STEP.GAMEOVER:  // 게임 오버 상태
                    // 플레이어 제어 비활성화 (게임 오버 후 더 이상 조작할 수 없도록)
                    this.player_control.enabled = false;
                    break;
            }

            // 게임 오버 후 타이머 초기화
            this.step_timer = 0.0f;
        }
    }
}