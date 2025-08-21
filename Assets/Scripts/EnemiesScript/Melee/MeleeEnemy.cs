using Unity.MLAgents.Actuators;
using UnityEngine;

namespace EnemiesScript.Melee
{
    public class MeleeEnemy:Enemy
    {
        private EnemyAttack _atk;
        float Timer = 0;
        protected override void Attack(int atkIndex)
        {
            _atk = Instantiate(attacks[atkIndex], gameObject.transform);
            Destroy(_atk.gameObject, _atk.lifetime);
        }

        protected override void MoveAgent(int actionIndex)
        {
            switch (actionIndex)
            {
                case 1: // Move forward
                    rb.AddForce(transform.forward * moveSpeed * 10f, ForceMode.Force);
                    // transform.position += transform.forward * _moveSpeed * Time.deltaTime;
                    break;
                case 2: // Move Backward
                    rb.AddForce(-transform.forward * moveSpeed * 10f, ForceMode.Force);
                    // transform.position -= transform.forward * _moveSpeed * Time.deltaTime;
                    break;
                case 3: // Stride Right
                    rb.AddForce(transform.right * moveSpeed * 10f, ForceMode.Force);
                    break;
                case 4: // Stride Left
                    rb.AddForce(-transform.right * moveSpeed * 10f, ForceMode.Force);
                    break;
            }
            SpeedControl();
        }

        protected override void OnHit(GameObject other)
        {
            Debug.Log("Agent sense hit");
        }

        protected override void OnKilled(GameObject other)
        {
            Debug.Log("Agent sense a kill");
        }
        protected override void RotateAgent(int actionIndex)
        {
            switch (actionIndex)
            {
                case 1: // Rotate left
                    transform.Rotate(0f, -rotateSpeed * Time.deltaTime, 0f);
                    break;
                case 2: // Rotate Right
                    transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
                    break;
            }
        }
        public void Update()
        {
            //for testing attack comment before commit
            //if(Timer >= 2)
            //{
            //    Attack(0);
            //    Debug.Log("Attack");
            //    Timer = 0;
            //}
            //Timer += Time.deltaTime;
        }
    }
}