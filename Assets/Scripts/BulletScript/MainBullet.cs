using UnityEngine;

public class MainBullet : MonoBehaviour
{
    [Header("Bullet data")]
    public float lifetime;

    private int groundLayer = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.ToString());
        //Debug.Log(groundLayer);
        if (collision.gameObject.layer == groundLayer) {
            //Debug.Log("Ground hit");
            Destroy(gameObject);
        }
        if (collision.gameObject.layer == 11)
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
