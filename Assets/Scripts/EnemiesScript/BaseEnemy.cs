using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [Header("Enemy Data")]
    public float hp=100;
    public List<BaseEnemyAttack> attacks = new List<BaseEnemyAttack>();
    float timer = 0;

    private EnemyMeleeAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // foreach (var atk in attacks)
        // {
        //     atk.OnAttack();//try to attack
        // }
        agent = GetComponent<EnemyMeleeAgent>();
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
        GetComponent<DamageTextSpawner>().SpawnDamageText(dmgPos, damage); //instantiate damage number with DamageTextSpawner component
        hp -= damage;
        agent.TakeDamage(damage);

    }
    
    // Update is called once per frame
    void Update()
    {
        if (hp <= 0) {
            //Spawn dead FX then delete object
            agent.EndEpisode();
            hp = maxHp;
        }
        if (timer > 2)
        {
            timer = 0;
            attacks[0].OnAttack(gameObject);
        }
        timer += Time.deltaTime;
    }
}
