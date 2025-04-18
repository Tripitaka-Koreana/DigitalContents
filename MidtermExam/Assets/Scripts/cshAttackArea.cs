using System.Collections.Generic;
using UnityEngine;

// 플레이어 공격 범위를 감지하고, 해당 범위 내의 오브젝트 목록을 관리하는 스크립트
public class cshAttackArea : MonoBehaviour
{
    // 현재 공격 범위 내에 있는 Collider 리스트를 외부에서 조회할 수 있도록 속성으로 제공
    public List<Collider> colliders
    {
        get
        {
            if (0 < colliderList.Count)
            {
                // colliderList 안에 null(파괴된 오브젝트) 값이 있으면 제거하고 반환
                colliderList.RemoveAll(c => c == null);
            }
            return colliderList;
        }
    }

    // 실제로 내부에서 사용하는 충돌체 리스트
    private List<Collider> colliderList = new List<Collider>();

    // 공격 범위(Trigger)에 오브젝트가 들어왔을 때 호출됨
    private void OnTriggerEnter(Collider other)
    {
        // 태그가 'BreakableObject' 또는 'Enemy'인 경우만 리스트에 추가
        if (other.CompareTag("BreakableObject") || other.CompareTag("Enemy"))
        {
            colliders.Add(other); // 위의 속성은 내부적으로 colliderList를 반환하므로 사용 가능
        }
    }

    // 공격 범위에서 오브젝트가 나갔을 때 호출됨
    private void OnTriggerExit(Collider other)
    {
        // 나간 오브젝트가 대상이면 리스트에서 제거
        if (other.CompareTag("BreakableObject") || other.CompareTag("Enemy"))
        {
            colliders.Remove(other);
        }
    }
}
