using UnityEngine;

public class cshChickBall : MonoBehaviour
{
    public GameObject divChickBall;

    public void division()
    {
        GameObject chick = Instantiate(divChickBall, gameObject.transform.position, Quaternion.identity);
        chick.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.5f, 0.5f) * 300.0f);
        chick = Instantiate(divChickBall, gameObject.transform.position, Quaternion.identity);
        chick.GetComponent<Rigidbody2D>().AddForce(new Vector2(-0.5f, 0.5f) * 300.0f);
        Destroy(gameObject);
    }
}
