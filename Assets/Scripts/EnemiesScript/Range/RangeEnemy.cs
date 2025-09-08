using Unity.MLAgents.Actuators;
using UnityEngine;

namespace EnemiesScript.Range
{
    public class RangeEnemy:Enemy
    {
        private EnemyAttack _atk;
        float Timer = 0;
        public override void RotateAgent(float actionValue)
        {
            throw new System.NotImplementedException();
        }

        public override void Attack(int atkIndex)
        {
            _atk = Instantiate(attacks[atkIndex], gameObject.transform.position + transform.forward, gameObject.transform.rotation);
            _atk.attacker = gameObject;
            _atk.OnAttack();
        }

        public override void Specials(int actionIndex)
        {
            throw new System.NotImplementedException();
        }

        public override void MoveAgentX(float actionValue)
        {
            throw new System.NotImplementedException();
        }

        public override void MoveAgentZ(float actionValue)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnHurt(GameObject other)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnKilled()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnAttackLanded()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnKilledTarget()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            ////for testing actions comment before commit
            //MoveAgent(Random.Range(0, 5));
            //if (Timer >= 2)
            //{
            //    Attack(0);
            //    RotateAgent(Random.Range(0, 3));
            //    Debug.Log("Attack");
            //    Timer = 0;
            //}
            //Timer += Time.deltaTime;
        }
    }
}