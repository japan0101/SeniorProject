using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements.Experimental;
using System;
using UnityEngine.Events;

namespace EnemiesScript
{
    public abstract class Enemy : MonoBehaviour
    {
        [Header("Enemy Data")]
        public float hp;
        public float maxHp;
        [FormerlySerializedAs("moveSpeed")] public float baseSpeed;
        public float dashSpeed;
        public float dashCooldown;
        public float dashDuration;
        public float dashConsume;
        public float rotateSpeed;
        public float defence;
        public float energy;
        public float maxEnergy;
        public float energyRegenPerSecond;
        
        protected float dashTimer = 0;
        protected float realSpeed;
        protected bool isDashing = false;
        protected bool isBSlaming = false;
        public List<EnemyAttack> attacks = new List<EnemyAttack>();
        protected Rigidbody rb;
        public PlayerHealth _playerHealthManager;
        public Enemy opponent;
        [HideInInspector]public GameObject _player;
        protected float atkModifier = 1.0f;
        private bool missed = false;
        protected Coroutine bufftimeRoutine;
        public float groundFriction = 2.5f;

        public event Action OnPlayerHurt;
        public event Action OnPlayerDie;

        [Header("Attack Cooldowns (in seconds)")]
        public float[] cooldownDurations;
        protected float[] nextAvailableAttackTime;

        public UnityEvent<float> TrackOnPlayerDamageDealt;
        public UnityEvent TrackOnEnemyAttack;
        public UnityEvent<bool> TrackOnDeath;
        protected void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            realSpeed = baseSpeed;
            if (attacks != null)
            {
                nextAvailableAttackTime = new float[attacks.Count];
            }
        }

        // Update is called once per frame
        protected void Update()
        {
            if (isDashing) {
                if (dashTimer > dashDuration)//check for dash cooldown when is dashing
                {
                    isDashing = false;
                    dashTimer = 0;
                }
                dashTimer += Time.deltaTime;
            }
            RegenEnergy(energyRegenPerSecond);
            
            // Debug.Log(realSpeed);
        }

        public void AddListenerToTarget(GameObject _target)
        {
            if(_target.TryGetComponent<Enemy>(out Enemy result))
            {
                opponent = result;
                opponent.OnPlayerHurt += OnAttackLanded;
                opponent.OnPlayerDie += OnKilledTarget;
            }
            else
            {
                try
                {
                    _playerHealthManager = _target.GetComponent<PlayerHealth>();
                    _playerHealthManager.OnPlayerHurt += OnAttackLanded;
                    _playerHealthManager.OnPlayerDie += OnKilledTarget;
                }
                catch (Exception ex) { 
                    Debug.LogException(ex);
                }
            }
        }
        public abstract void Specials(int actionIndex);
        public abstract void MoveAgentX(float actionValue);
        public abstract void MoveAgentZ(float actionValue);
        public abstract void RotateAgent(float actionValue);
        public abstract void Attack(int atkIndex);
        private void RegenEnergy(float amountPerSecond)
        {
            energy = Mathf.Clamp(energy + amountPerSecond * Time.deltaTime , 0, maxEnergy);
        }
        public void Dash()
        {
            isDashing = true;
            energy -= dashConsume;
        }
        public void BslamOverride()
        {
            isBSlaming = !isBSlaming;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform.IsChildOf(gameObject.transform)) return;
            if (other.gameObject.layer == 9)
            {
                MainBullet bullet = other.gameObject.GetComponent<MainBullet>();
                if (bullet != null)
                {
                    //spawn hit FX then register damage
                    TakeDamage(bullet.damage);
                    OnHurt();
                }
            }else if (other.gameObject.layer == 12)
            {
                EnemyAttack _atk = other.gameObject.GetComponent<EnemyAttack>();
                if (_atk != null)
                {
                    TakeDamage(_atk.damage);
                    OnHurt();
                }
            }
        }
        protected abstract void OnHurt(); // Called when Agent getting Hurt
        protected virtual void OnKilled()
        {
            OnPlayerDie?.Invoke();
        } // Called when Agent getting Killed
        protected abstract void OnAttackLanded(); // Called when Agent Hit Something
        protected abstract void OnKilledTarget(); // Called when Agent Kill Something

        protected void TakeDamage(float damage)
        {
            //instantiate damage number with DamageTextSpawner component
            //realDmg = 
            // GetComponent<DamageTextSpawner>().SpawnDamageText(new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), damage);
            TrackOnPlayerDamageDealt?.Invoke(damage);
            hp -= damage - (defence / 100);// reduce health
            OnPlayerHurt?.Invoke();
        }

        public bool CanUseAttack(int atkIndex)
        {
            int actualIndex = atkIndex - 1;

            // Failsafe: If the index is invalid, just say no.
            if (actualIndex < 0 || actualIndex >= nextAvailableAttackTime.Length) return false;

            // Check if the current game time has passed the required wait time
            return Time.time >= nextAvailableAttackTime[actualIndex];
        }
        protected void SpeedControl()
        {
            rb.linearDamping = groundFriction;
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (isDashing)
            {
                realSpeed = dashSpeed;
            }else if (isBSlaming)
            {
                realSpeed = 50;
            }
            else
            {
                realSpeed = baseSpeed;
            }
            if (flatVel.magnitude > realSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * baseSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
            //Debug.Log(realSpeed);
        }
        private void OnDestroy()
        {
        }

        protected IEnumerator BuffTimer(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            atkModifier = 1;
        }
    }
}
