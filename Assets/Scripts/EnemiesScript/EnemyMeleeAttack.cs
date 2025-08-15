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
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player" && gameObject.tag != "EnemyAttacks")//check if the collided objects are player
            {
                PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();//get playerHealth component to deal damage to player
                if (player)//in case playerHealth is not there
                {
                    //The damage are stored in children's Atttack component
                    player.TakeDamage(collision.contacts[0].point, GetComponentInChildren<Attacks>().damage);
                    if (player.IsDead())//ask if the player is dead to the player when hit
                    {
                        GetComponentInParent<EnemyMeleeAgent>().OnKillPlayer();//if the player should be dead then add points
                        return;
                    }
                    //The parent are the agent who execute the attack
                    GetComponentInParent<EnemyMeleeAgent>().OnAttackSuccess();
                }
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
