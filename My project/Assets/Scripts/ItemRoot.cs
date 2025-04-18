using UnityEngine;
using System.Collections.Generic;

// 아이템 타입 정의용 클래스
public class Item
{
    public enum TYPE
    {
        NONE = -1,
        IRON = 0,
        APPLE,
        PLANT,
        NUM, // 타입 수 카운트용
    };
}
public class ItemRoot : MonoBehaviour
{
    // 리스폰 위치 리스트
    protected List<Vector3> respawn_points;

    // 아이템 프리팹 (씬에 미리 연결되어 있어야 함)
    public GameObject ironPrefab = null;
    public GameObject plantPrefab = null;
    public GameObject applePrefab = null;

    // 상태 추적용 타이머
    public float step_timer = 0.0f;

    // 아이템별 리스폰 시간 설정
    public static float RESPAWN_TIME_APPLE = 20.0f;
    public static float RESPAWN_TIME_IRON = 12.0f;
    public static float RESPAWN_TIME_PLANT = 6.0f;

    // 각각의 리스폰 타이머
    private float respawn_timer_apple = 0.0f;
    private float respawn_timer_iron = 0.0f;
    private float respawn_timer_plant = 0.0f;

    // 아이템 오브젝트의 타입을 반환
    public Item.TYPE getItemType(GameObject item_go)
    {
        Item.TYPE type = Item.TYPE.NONE;
        if (item_go != null)
        {
            switch (item_go.tag)
            {
                case "Iron": type = Item.TYPE.IRON; break;
                case "Apple": type = Item.TYPE.APPLE; break;
                case "Plant": type = Item.TYPE.PLANT; break;
            }
        }
        return (type);
    }

    // 철광석 리스폰 처리
    public void respawnIron()
    {
        GameObject iron_go = GameObject.Instantiate(this.ironPrefab) as GameObject;

        // 특정 위치에서 리스폰 (약간 랜덤 위치 조정)
        Vector3 pos = GameObject.Find("IronRespawn").transform.position;
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        iron_go.transform.position = pos;
    }

    // 식물(오이) 리스폰 처리
    public void respawnPlant()
    {
        if (this.respawn_points.Count > 0)
        {
            GameObject plant_go = GameObject.Instantiate(this.plantPrefab) as GameObject;

            // 리스폰 위치 중 랜덤 선택
            int n = Random.Range(0, this.respawn_points.Count);
            Vector3 pos = this.respawn_points[n];

            pos.y = 1.0f;
            pos.x += Random.Range(-1.0f, 1.0f);
            pos.z += Random.Range(-1.0f, 1.0f);
            plant_go.transform.position = pos;
        }
    }

    // 사과 리스폰 처리
    public void respawnApple()
    {
        GameObject apple_go = GameObject.Instantiate(this.applePrefab) as GameObject;

        Vector3 pos = GameObject.Find("AppleRespawn").transform.position;
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        apple_go.transform.position = pos;
    }

    void Start()
    {
        // 오브젝트에 지정된 'ItemRespawn' 태그들을 모두 찾아 위치 저장
        this.respawn_points = new List<Vector3>();
        GameObject[] respawns = GameObject.FindGameObjectsWithTag("ItemRespawn");
        foreach (GameObject go in respawns)
        {
            // 리스폰 지점 오브젝트는 렌더링 숨김 처리
            MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            // 위치 저장
            this.respawn_points.Add(go.transform.position);
        }

        // 사과 및 철 리스폰 위치 숨기기
        GameObject applerespawn = GameObject.Find("AppleRespawn");
        applerespawn.GetComponentInChildren<MeshRenderer>().enabled = false;

        GameObject ironrespawn = GameObject.Find("IronRespawn");
        ironrespawn.GetComponentInChildren<MeshRenderer>().enabled = false;

        // 초기 리스폰 실행
        this.respawnIron();
        this.respawnPlant();
    }

    void Update()
    {
        // 각 리스폰 타이머 증가
        respawn_timer_apple += Time.deltaTime;
        respawn_timer_iron += Time.deltaTime;
        respawn_timer_plant += Time.deltaTime;

        // 사과 리스폰 (시간 설정 오류: IRON 시간 사용 중 — 의도된 건가요?)
        if (respawn_timer_apple > RESPAWN_TIME_IRON)
        {
            respawn_timer_apple = 0.0f;
            this.respawnApple();
        }

        // 철 리스폰
        if (respawn_timer_iron > RESPAWN_TIME_IRON)
        {
            respawn_timer_iron = 0.0f;
            this.respawnIron();
        }

        // 식물 리스폰
        if (respawn_timer_plant > RESPAWN_TIME_PLANT)
        {
            respawn_timer_plant = 0.0f;
            this.respawnPlant();
        }
    }

    // 아이템 사용 시 상승하는 수리 수치 반환
    public float getGainRepairment(GameObject item_go)
    {
        float gain = 0.0f;
        if (item_go == null)
        {
            gain = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type)
            {
                case Item.TYPE.IRON:
                    gain = GameStatus.GAIN_REPARIMENT_IRON;
                    break;
                case Item.TYPE.PLANT:
                    gain = GameStatus.GAIN_REPARIMENT_PLANT;
                    break;
            }
        }
        return gain;
    }

    // 아이템 들고 이동 시 포만도(체력) 소모량 반환
    public float getConsumeSatiety(GameObject item_go)
    {
        float consume = 0.0f;
        if (item_go == null)
        {
            consume = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type)
            {
                case Item.TYPE.IRON:
                    consume = GameStatus.CONSUME_SATIETY_IRON;
                    break;
                case Item.TYPE.APPLE:
                    consume = GameStatus.CONSUME_SATIETY_APPLE;
                    break;
                case Item.TYPE.PLANT:
                    consume = GameStatus.CONSUME_SATIETY_PLANT;
                    break;
            }
        }
        return consume;
    }

    // 아이템 먹었을 때 회복되는 포만도 반환
    public float getRegainSatiety(GameObject item_go)
    {
        float regain = 0.0f;
        if (item_go == null)
        {
            regain = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type)
            {
                case Item.TYPE.APPLE:
                    regain = GameStatus.REGAIN_SATIETY_APPLE;
                    break;
                case Item.TYPE.PLANT:
                    regain = GameStatus.REGAIN_SATIETY_PLANT;
                    break;
            }
        }
        return regain;
    }
}
