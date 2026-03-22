using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EnemiesScript.Boss
{
    public class BossEnemyAgent : EnemyAgent
    {
        private const int basicslashIndex = 1;
        private const int thrustIndex = 2;
        private const int warcryIndex = 3;
        private const int bodyslamIndex = 4;
        private const int jumpslamIndex = 5;
        private const int evadeslashIndex = 6;

        // Health thresholds (percentage)
        private const float phase2Threshold = 0.75f; // 75% HP - unlocks thrust & warcry
        private const float phase3Threshold = 0.50f; // 50% HP - unlocks bodyslam
        private const float phase4Threshold = 0.25f; // 25% HP - unlocks jumpslam & evadeslash
        public float reward;
        private BossEnemy bossEnemy;

        private new void Awake()
        {
            base.Awake();
            agent._player = GameObject.FindGameObjectWithTag("Player");
            bossEnemy = GetComponent<BossEnemy>();

            if (agent._player == null)
                Debug.LogWarning("BossEnemyAgent: No GameObject with tag 'Player' found!");
        }

        public override void Initialize()
        {
            base.Initialize();
            if (isTraining)
            {
                currentEpisode = 0;
                cumulativeReward = 0f;
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActions = actionsOut.ContinuousActions;
            var discreteActionsOut = actionsOut.DiscreteActions;
            continuousActions[0] = Input.GetAxis("Horizontal");
            continuousActions[1] = Input.GetAxis("Vertical");

            discreteActionsOut[0] = 0; // Do nothing
            discreteActionsOut[1] = 0; // Do nothing

            if (Input.GetKey(KeyCode.LeftShift)) discreteActionsOut[0] = 1; //Dash

            if (Input.GetKey(KeyCode.Keypad1)) discreteActionsOut[1] = basicslashIndex;
            if (Input.GetKey(KeyCode.Keypad2)) discreteActionsOut[1] = thrustIndex;
            if (Input.GetKey(KeyCode.Keypad3)) discreteActionsOut[1] = warcryIndex;
            if (Input.GetKey(KeyCode.Keypad4)) discreteActionsOut[1] = bodyslamIndex;
            if (Input.GetKey(KeyCode.Keypad5)) discreteActionsOut[1] = jumpslamIndex;
            if (Input.GetKey(KeyCode.Keypad6)) discreteActionsOut[1] = evadeslashIndex;

            if (Input.GetKey(KeyCode.Space)) discreteActionsOut[1] = basicslashIndex;
            if (Input.GetKey(KeyCode.Q)) continuousActions[2] = -1;
            if (Input.GetKey(KeyCode.E)) continuousActions[2] = 1;
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            if (agent._player == null) return;

            var distance = Vector3.Distance(transform.position, agent._player.transform.position);

            // Get current health percentage (adjust agent.health & agent.maxHealth to your actual property names)
            var healthPercent = agent.hp / agent.maxHp;

            var isCloseRange = distance < 3f;
            var isMidRange = distance >= 3f && distance <= 6f;
            var isFarRange = distance > 6f;

            // Phase 1 (100% - 75% HP): Basic slash & Thrust only
            actionMask.SetActionEnabled(1, basicslashIndex, isCloseRange && bossEnemy.CanUseAttack(basicslashIndex));
            actionMask.SetActionEnabled(1, thrustIndex, (isCloseRange || isMidRange) && bossEnemy.CanUseAttack(thrustIndex));

            // Phase 2 (below 75% HP): Unlock Warcry
            actionMask.SetActionEnabled(1, warcryIndex, healthPercent <= phase2Threshold && bossEnemy.CanUseAttack(warcryIndex));

            // Phase 3 (below 50% HP): Unlock Bodyslam
            actionMask.SetActionEnabled(1, bodyslamIndex, healthPercent <= phase3Threshold && bossEnemy.CanUseAttack(bodyslamIndex));

            // Phase 4 (below 25% HP): Unlock Jumpslam & EvadeSlash
            actionMask.SetActionEnabled(1, jumpslamIndex, healthPercent <= phase4Threshold && bossEnemy.CanUseAttack(jumpslamIndex));
            actionMask.SetActionEnabled(1, evadeslashIndex, healthPercent <= phase4Threshold && bossEnemy.CanUseAttack(evadeslashIndex));
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if (agent._player != null)
            {
                //Direction vector to player
                var toPlayer = agent._player.transform.position - transform.position;
                sensor.AddObservation(toPlayer.normalized);
                sensor.AddObservation(toPlayer.magnitude / 30f);
            }
            else
            {
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(0f);
            }

            //current forward direction
            sensor.AddObservation(transform.forward);
            sensor.AddObservation(agent.energy);

            // FIX: Use local position instead of world position
            sensor.AddObservation(transform.localPosition);

            //It's own Velocity
            var rb = GetComponent<Rigidbody>();
            if (rb != null)
                sensor.AddObservation(rb.angularVelocity.y / 10f);
            else
                sensor.AddObservation(0f);

            // Add health percent so agent knows which attacks are available
            sensor.AddObservation(agent.hp / agent.maxHp);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            var moveX = actions.ContinuousActions[0];
            var moveZ = actions.ContinuousActions[1];
            var rotation = actions.ContinuousActions[2];

            var special = actions.DiscreteActions[0];
            var attack = actions.DiscreteActions[1];

            agent.MoveAgentX(moveX);
            agent.MoveAgentZ(moveZ);
            agent.RotateAgent(rotation);
            agent.Specials(special);

            _attackedThisStep = false;

            if (attack > 0)
            {
                agent.Attack(attack);
                _attackedThisStep = true;
            }

            base.OnActionReceived(actions);


                var player = agent._player;
                Debug.Log("Player position: " + player.transform.position);
                var currentDistance = Vector3.Distance(transform.position, player.transform.position);

                // Penalty for being outside optimal attack range
                var optimalRange = 2.5f;
                var distanceFromOptimal = Mathf.Abs(currentDistance - optimalRange);
                AddReward(-distanceFromOptimal * 0.003f);

                // Facing reward: +0.05 when fully facing player, -0.05 when facing away
                // This is the ONLY rotation signal — no per-step rotation penalty
                // (stacked penalties caused reward collapse)
                var toPlayer = (player.transform.position - transform.position).normalized;
                var facingDot = Vector3.Dot(transform.forward, toPlayer);
                AddReward(facingDot * 0.05f);

                _lastDistance = currentDistance;
                cumulativeReward = GetCumulativeReward();
        }

        public override void OnAttack()
        {
            if (!isTraining) return;
            // AddReward(-0.02f);
            cumulativeReward = GetCumulativeReward();
        }

        public override void OnSpecial()
        {
            if (!isTraining) return;
        }

        public override void
            OnAttackMissed() //Called by Enemy attack event listener to notify that the attack launched did not and on a player
        {
            if (!isTraining) return;
        }

        public override void OnAttackLanded() // Called when Agent Hit Something
        {
            AddReward(1f);
            cumulativeReward = GetCumulativeReward();
        }

        public override void OnKilledTarget() // Called when Agent Kill Something
        {
            if (!isTraining) return;
            AddReward(5f);
            cumulativeReward = GetCumulativeReward();
        }

        public override void OnKilled()
        {
            // Getting Killed
            if (!isTraining) return;
            AddReward(-1f);
            cumulativeReward = GetCumulativeReward();
            arenaController?.EnemyDefeated(this);
        }

        public override void OnHurt()
        {
            if (!isTraining) return;
            AddReward(-0.01f);
            cumulativeReward = GetCumulativeReward();
        }


        public override void OnEpisodeBegin()
        {
            if (!isTraining) return;

            currentEpisode++;
            cumulativeReward = 0f;

            // Using the TrainingController the Handle the Spawn Logic
            // SpawnPlayer();
            // if (agent._player != null)
            //     _lastDistance = Vector3.Distance(transform.position, agent._player.transform.position);
        }
    }
}