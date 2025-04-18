using UnityEngine;
using System.Collections.Generic;

public class Item
{
    public enum TYPE
    {
        NONE = -1,
        IRON = 0,
        APPLE,
        PLANT,
        NUM,
    };
}

public class ItemRoot : MonoBehaviour
{
    // 클래스 내 전역변수 추가
    // iron_respawn
    protected List<Vector3> respawn_points;

    public GameObject ironPrefab = null;
    public GameObject plantPrefab = null;
    public GameObject applePrefab = null;

    public float step_timer = 0.0f;
    public static float RESPAWN_TIME_APPLE = 20.0f;
    public static float RESPAWN_TIME_IRON = 12.0f;
    public static float RESPAWN_TIME_PLANT = 6.0f;
    private float respawn_timer_apple = 0.0f;
    private float respawn_timer_iron = 0.0f;
    private float respawn_timer_plant = 0.0f;

    // 아이템 타입을 조사한다.
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
    // 철광석 리스폰----------------------------.
    public void respawnIron()
    {
        GameObject iron_go = GameObject.Instantiate(this.ironPrefab) as GameObject;
        Vector3 pos = GameObject.Find("IronRespawn").transform.position;
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        iron_go.transform.position = pos;
    }

    // 오이 리스폰----------------------------.
    public void respawnPlant()
    {
        if (this.respawn_points.Count > 0)
        {
            GameObject iron_go = GameObject.Instantiate(this.plantPrefab) as GameObject;
            int n = Random.Range(0, this.respawn_points.Count);
            Vector3 pos = this.respawn_points[n];
            pos.y = 1.0f;
            pos.x += Random.Range(-1.0f, 1.0f);
            pos.z += Random.Range(-1.0f, 1.0f);
            iron_go.transform.position = pos;
        }
    }
    // 사과 리스폰------------------------------------------------
    public void respawnApple()
    {
        GameObject iron_go = GameObject.Instantiate(this.applePrefab) as GameObject;
        Vector3 pos = GameObject.Find("AppleRespawn").transform.position;
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        iron_go.transform.position = pos;
    }
    void Start()
    {
        // 리스폰 포인트의 게임 오브젝트를 찾아서 좌표를 배열로 해둔다.
        this.respawn_points = new List<Vector3>();// list
        GameObject[] respawns = GameObject.FindGameObjectsWithTag("ItemRespawn");// ItemRespawn(TAG)
        foreach (GameObject go in respawns)
        {
            // 메시는 비표시로 한다.
            MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
            this.respawn_points.Add(go.transform.position);
        }

        GameObject applerespawn = GameObject.Find("AppleRespawn");
        applerespawn.GetComponentInChildren<MeshRenderer>().enabled = false; GameObject ironrespawn = GameObject.Find("IronRespawn");
        ironrespawn.GetComponentInChildren<MeshRenderer>().enabled = false;

        this.respawnIron();
        this.respawnPlant();
    }
    void Update()
    {
        respawn_timer_apple += Time.deltaTime;
        respawn_timer_iron += Time.deltaTime;
        respawn_timer_plant += Time.deltaTime;

        if (respawn_timer_apple > RESPAWN_TIME_IRON)
        {
            respawn_timer_apple = 0.0f;
            this.respawnApple();
        }
        if (respawn_timer_iron > RESPAWN_TIME_IRON)
        {
            respawn_timer_iron = 0.0f;
            this.respawnIron();
        }

        if (respawn_timer_plant > RESPAWN_TIME_PLANT)
        {
            respawn_timer_plant = 0.0f;
            this.respawnPlant();
        }
    }

}

