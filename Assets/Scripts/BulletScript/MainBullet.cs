using UnityEngine;

public class MainBullet : MonoBehaviour
{
    public LayerMask whatIsGround;
    public LayerMask whatIsEnemy;
    public LayerMask whatIsPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == whatIsGround) {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
