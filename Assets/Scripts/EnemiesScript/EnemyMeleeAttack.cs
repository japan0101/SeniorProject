using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript
{
    public class EnemyMeleeAttack : BaseEnemyAttack
    {

        [FormerlySerializedAs("Slash")] public GameObject slash;
        private GameObject _atk;

        private void Update()
        {
            if (_atk)
            {
                //if there are attack hitbox available, then rotate it around a parent object
                _atk.transform.RotateAround(transform.position, transform.up, 1440f * Time.deltaTime/4f);
            }
        }

        // Call this to start the slash (e.g., in an animation event or attack script)
        public override void OnAttack(GameObject attackComp)
        {
            if (_atk) return;
            slash.GetComponent<EnemyMeleeAttack>().attacker = attackComp;
            _atk = Instantiate(slash, attackComp.transform); //create attack hitbox
            Destroy(_atk, 0.5f); //despawn after a certain time
            
        }
    }
}
