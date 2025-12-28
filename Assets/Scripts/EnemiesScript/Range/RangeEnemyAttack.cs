using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript.Range
{
    public class RangeEnemyAttack:EnemyAttack
    {
        [Header("Range config")]
        public float power;
        //public float attack_speed = 1440f;
        public override void OnAttack(float dmgModifier)
        {
            Debug.Log("Shooting");
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(attacker.transform.forward * power * 100, ForceMode.Force);
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player" | other.gameObject.tag == "Untagged")
            {
                isMissed = false;//For using in OnDestroyed checks weather the attack hit a player
                Destroy(gameObject);
            }
        }
        void Update()
        {
            
        }
    }
}