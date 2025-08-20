using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript.Melee
{
    public class MeleeEnemyAttack:EnemyAttack
    {
        public GameObject slash;
        private GameObject _atk;
        
        public override void OnAttack(GameObject attacker)
        {
            if (_atk) return;
            slash.GetComponent<EnemyMeleeAttack>().attacker = attacker;
            _atk = Instantiate(slash, attacker.transform); //create attack hitbox
            Destroy(_atk, 0.5f); //despawn after a certain time
        }

        void Update()
        {
            if (_atk)
            {
                _atk.transform.RotateAround(transform.position, transform.up, 1440f * Time.deltaTime/4f);
            }
        }
    }
}