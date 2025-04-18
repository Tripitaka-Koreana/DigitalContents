using UnityEngine;
using UnityEngine.UI;

public class cshButton : MonoBehaviour
{
    public Button btnJump;         // 점프 버튼 (UI에서 연결)
    public Button btnAttack;       // 공격 버튼 (UI에서 연결)
    public cshPlayerController sPlayer; // 플레이어 스크립트 참조 (Player 오브젝트 연결)

    // 게임 시작 시 실행됨
    void Start()
    {
        // 점프 버튼 활성화
        btnJump.gameObject.SetActive(true);
        // 점프 버튼에 이전 리스너 제거
        btnJump.onClick.RemoveAllListeners();
        // 점프 버튼 클릭 시 실행할 함수 등록
        btnJump.onClick.AddListener(OnClickJumpButton);

        // 공격 버튼 비활성화 (기본은 안 보임)
        btnAttack.gameObject.SetActive(false);
        // 공격 버튼에 이전 리스너 제거
        btnAttack.onClick.RemoveAllListeners();
        // 공격 버튼 클릭 시 실행할 함수 등록
        btnAttack.onClick.AddListener(OnClickAttackButton);
    }

    // 매 프레임마다 버튼 상태 업데이트
    void Update()
    {
        UpdateButton();
    }

    // 공격 가능 여부에 따라 버튼 UI 상태를 갱신하는 함수
    private void UpdateButton()
    {
        bool canAttack = sPlayer.CanAttack(); // 공격할 수 있는 대상이 있는지 확인

        btnAttack.gameObject.SetActive(canAttack);   // 공격 가능 → 공격 버튼 ON
        btnJump.gameObject.SetActive(!canAttack);    // 공격 불가 → 점프 버튼 ON
    }

    // 점프 버튼을 눌렀을 때 호출되는 함수
    private void OnClickJumpButton()
    {
        sPlayer.OnVirtualPadJump(); // 플레이어 점프 실행
    }

    // 공격 버튼을 눌렀을 때 호출되는 함수
    private void OnClickAttackButton()
    {
        sPlayer.OnVirtualPadAttack(); // 플레이어 공격 실행
    }
}
