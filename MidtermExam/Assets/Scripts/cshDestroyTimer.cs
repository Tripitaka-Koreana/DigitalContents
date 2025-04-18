using UnityEngine;

public class cshDestroyTimer : MonoBehaviour
{
    public float fDestroyTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, fDestroyTime);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
