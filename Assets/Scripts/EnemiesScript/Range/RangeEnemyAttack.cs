using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript.Range
{
    public class RangeEnemyAttack:EnemyAttack
    {
        [Header("Range config")]
        public float power;
        //public float attack_speed = 1440f;
        public override void OnAttack()
        {
            Debug.Log("Shooting");
            Rigidbody rb = GetComponent<Rigidbody>();
            //rb.freezeRotation = true;
            Vector3 direction = new Vector3(attacker.transform.forward.x, attacker.transform.forward.y, attacker.transform.forward.z);
            rb.AddForce(direction * power * 100, ForceMode.Force);
            Destroy(gameObject, lifetime);
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