using TMPro;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
    // 수리 시 상승하는 수리도(수리도는 0.0f에서 1.0f 사이)
    public static float GAIN_REPARIMENT_IRON = 0.15f; //!< 철을 수리할 때 상승하는 수리도.
    public static float GAIN_REPARIMENT_PLANT = 0.05f; //!< 식물을 수리할 때 상승하는 수리도.

    // 아이템을 운반할 때 1초당 소모되는 체력.
    public static float CONSUME_SATIETY_IRON = 0.15f / 2.0f; //!< 철을 운반할 때 소모되는 체력.
    public static float CONSUME_SATIETY_APPLE = 0.1f / 2.0f; //!< 사과를 운반할 때 소모되는 체력.
    public static float CONSUME_SATIETY_PLANT = 0.1f / 2.0f; //!< 식물을 운반할 때 소모되는 체력.

    // 아이템을 먹었을 때 회복하는 체력.
    public static float REGAIN_SATIETY_APPLE = 0.6f; //!< 사과를 먹었을 때 회복되는 체력.
    public static float REGAIN_SATIETY_PLANT = 0.2f; //!< 식물을 먹었을 때 회복되는 체력.

    // 일정 시간마다 항상 소모되는 체력
    public static float CONSUME_SATIETY_ALAWAYS = 0.03f; //!< 항상 소모되는 체력.

    // ================================================================ //
    public float repairment = 0.0f; //!< 현재 수리도(0.0f ~ 1.0f).
    public float satiety = 1.0f; //!< 현재 포만도(체력, 0.0f ~ 1.0f).

    // UI 텍스트 요소들 (수리도와 포만도 상태를 표시)
    public TextMeshProUGUI Repairment; //!< 수리도 UI 표시.
    public TextMeshProUGUI Satiety; //!< 포만도(UI) 표시.

    // 매 프레임마다 호출되는 Update 메서드
    private void Update()
    {
        // 수리도와 포만도 값을 UI에 업데이트
        Repairment.text = "Repairment : " + repairment.ToString("F2");
        Satiety.text = "Satiety : " + satiety.ToString("F2");
    }

    // 수리도를 증가시키거나 감소시킴 (0.0f ~ 1.0f 범위 내로 제한)
    public void addRepairment(float add)
    {
        this.repairment = Mathf.Clamp01(this.repairment + add); // 0과 1 사이로 수리도 제한
    }

    // 포만도를 증가시키거나 감소시킴 (0.0f ~ 1.0f 범위 내로 제한)
    public void addSatiety(float add)
    {
        this.satiety = Mathf.Clamp01(this.satiety + add); // 0과 1 사이로 포만도 제한
    }

    // 게임 클리어 여부 확인 (수리도가 1.0f이면 클리어)
    public bool isGameClear()
    {
        bool is_clear = false;
        if (this.repairment >= 1.0f) // 수리도가 1.0f 이상이면 클리어
        {
            is_clear = true;
        }
        return is_clear;
    }

    // 게임 오버 여부 확인 (포만도가 0 이하이면 게임 오버)
    public bool isGameOver()
    {
        bool is_over = false;
        if (this.satiety <= 0.0f) // 포만도가 0 이하이면 게임 오버
        {
            is_over = true;
        }
        return is_over;
    }

    // 항상 일정 시간마다 소모되는 포만도
    public void alwaysSatiety()
    {
        this.satiety = Mathf.Clamp01(this.satiety - CONSUME_SATIETY_ALAWAYS * Time.deltaTime); // 항상 일정 비율로 포만도 감소
    }
}
