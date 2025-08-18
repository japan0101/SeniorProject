using EnemiesScript;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;

    [Header("References")]
    public TextMeshProUGUI HPDisplay;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisplayHP();
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Check if collision is with EnemyAttack layer
        //Debug.Log(collision.gameObject);

        if (collision.gameObject.tag == "EnemyAttacks")
        {
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
                //attacker.OnHitPlayer();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnemyAttacks")
        {
            MainBullet bullet = other.gameObject.GetComponent<MainBullet>();
            if (bullet != null)
            {
                //spawn hit FX then register damage
                TakeDamage(other.ClosestPointOnBounds(other.transform.position), bullet.damage);
            }
            Attacks attacks = other.gameObject.GetComponent<Attacks>();
            if (attacks != null)
            {
                TakeDamage(other.ClosestPointOnBounds(other.transform.position), attacks.damage);
                //attacker.OnHitPlayer();
            }
        }
    }

    public void TakeDamage(Vector3 dmgPos, float damage)
    {
        //GetComponent<DamageTextSpawner>().SpawnDamageText(dmgPos, damage);
        _stats.ModifyHP(-damage);
        Debug.Log(_stats.currentHP);
        DisplayHP();
    }
    public bool IsDead()
    {//for use in enemy attaack detection
        return _stats.currentHP <= 0;
    }
    private void DisplayHP()
    {
        HPDisplay.text = "Health: " + _stats.currentHP + " / " + _stats.maxHP;
    }
    // Update is called once per frame
    void Update()
    {
        if (_stats.currentHP <= 0)
        {
            _stats.currentHP = _stats.maxHP;
        }
    }
}
