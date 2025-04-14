using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [Header("Enemy Data")]
    public float hp=10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Check if collision is with PlayerAttack layer
        if (collision.gameObject.layer == 9)
        {
            MainBullet bullet = collision.gameObject.GetComponent<MainBullet>();
            if (bullet != null)
            {
                //spawn hit FX then register damage
                TakeDamage(collision.contacts[0].point, bullet.damage);
            }
        }
    }

    public void TakeDamage(Vector3 dmgPos, float damage)
    {
        GetComponent<DamageTextSpawner>().SpawnDamageText(dmgPos, damage);
        hp -= damage;
    }
    // Update is called once per frame
    void Update()
    {
        if (hp <= 0) {
            //Spawn dead FX then delete object
            Destroy(gameObject);
        }
    }
}
