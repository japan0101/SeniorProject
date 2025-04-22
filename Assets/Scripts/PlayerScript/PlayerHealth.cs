using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Attributes")]
    public float maxHP;
    private float currentHP;

    [Header("Refferences")]
    public TextMeshProUGUI HPDisplay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
        DisplayHP();
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Check if collision is with EnemyAttack layer
        Debug.Log(collision.gameObject.layer);
        if (collision.gameObject.layer == 12)
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
            }
        }
    }

    public void TakeDamage(Vector3 dmgPos, float damage)
    {
        //GetComponent<DamageTextSpawner>().SpawnDamageText(dmgPos, damage);
        currentHP -= damage;
        DisplayHP();
    }

    private void DisplayHP()
    {
        HPDisplay.text = "Health: " + currentHP + " / " + maxHP;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
