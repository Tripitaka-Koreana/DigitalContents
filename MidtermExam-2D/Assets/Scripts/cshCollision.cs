using UnityEngine;

public class cshCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ChickBall"))
        {
            collision.gameObject.GetComponent<cshChickBall>().division();
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("ChickBallDiv"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
