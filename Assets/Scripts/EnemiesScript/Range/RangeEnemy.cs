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

        // public override void MoveAgent(int actionIndex)
        // {
        //     switch (actionIndex)
        //     {
        //         case 1: // Move forward
        //             rb.AddForce(transform.forward * moveSpeed * 10f, ForceMode.Force);
        //             // transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        //             break;
        //         case 2: // Move Backward
        //             rb.AddForce(-transform.forward * moveSpeed * 10f, ForceMode.Force);
        //             // transform.position -= transform.forward * _moveSpeed * Time.deltaTime;
        //             break;
        //         case 3: // Stride Right
        //             rb.AddForce(transform.right * realSpeed * 10f, ForceMode.Force);
        //             break;
        //         case 4: // Stride Left
        //             rb.AddForce(-transform.right * realSpeed * 10f, ForceMode.Force);
        //             break;
        //     }
        //     SpeedControl();
        // }

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

        protected override void OnHit(GameObject other)
        {
            Debug.Log("Agent sense hit");
        }

        protected override void OnKilled(GameObject other)
        {
            Debug.Log("Agent sense a kill");
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