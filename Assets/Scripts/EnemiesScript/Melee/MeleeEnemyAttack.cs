using UnityEngine;

namespace EnemiesScript.Melee
{
    public class MeleeEnemyAttack : EnemyAttack
    {
        [Header("Melee config")] public float attack_speed = 1440f;

        private void Update()
        {
            transform.RotateAround(transform.position, transform.up, attack_speed * Time.deltaTime / 4f);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                isMissed = false; //For using in OnDestroyed checks weather the attack hit a player
        }

        public override void OnAttack(float dmgModifier)
        {
            Debug.Log("Slashing");
        }
    }
}