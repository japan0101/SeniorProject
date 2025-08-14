using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;

    [Header("Refferences")]
    public TextMeshProUGUI HPDisplay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisplayHP();
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Check if collision is with EnemyAttack layer
        //Debug.Log(collision.gameObject.layer);
        if (collision.gameObject.layer == 12)
        {
            EnemyMeleeAgent attacker = collision.gameObject.GetComponent<EnemyMeleeAttack>().attacker.GetComponent<EnemyMeleeAgent>();
            Debug.Log(attacker);
            if (attacker)
            {
                Debug.Log("Player detect hits");
                attacker.OnHitPlayer();
            }
            MainBullet bullet = collision.gameObject.GetComponent<MainBullet>();
            if (bullet != null)
            {
                //spawn hit FX then register damage
                TakeDamage(collision.contacts[0].point, bullet.damage);
            }
            Attacks attacks = collision.gameObject.GetComponent<Attacks>();
            if (attacks != null)
            {
                TakeDamage(collision.contacts[0].point, attacks.damage);
                attacker.OnHitPlayer();
            }
        }
    }

    public void TakeDamage(Vector3 dmgPos, float damage)
    {
        //GetComponent<DamageTextSpawner>().SpawnDamageText(dmgPos, damage);
        _stats.ModifyHP(-damage);
        DisplayHP();
    }

    private void DisplayHP()
    {
        HPDisplay.text = "Health: " + _stats.currentHP + " / " + _stats.maxHP;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
