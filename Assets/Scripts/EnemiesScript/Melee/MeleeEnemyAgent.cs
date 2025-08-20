using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript.Melee
{
    public class MeleeEnemyAgent:Agent
    {
        [SerializeField] private Transform target;
        [SerializeField] private Renderer groundRenderer;

        private Renderer _renderer;
        
        [HideInInspector] public int currentEpisode;
        [HideInInspector] public float cumulativeReward;

        private Color _defaultGroundColor;
        private Coroutine _flashGroundCoroutine;

        public override void Initialize()
        {
            _renderer = GetComponent<Renderer>();
            currentEpisode = 0;
            cumulativeReward = 0f;

            if (groundRenderer)
            {
                _defaultGroundColor = groundRenderer.material.color;
            }
        }
        private IEnumerator FlashGround(Color targetColor, float duration)
        {
            float elapsedTime = 0f;
        
            groundRenderer.material.color = targetColor;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                groundRenderer.material.color = Color.Lerp(targetColor, _defaultGroundColor, elapsedTime / duration);
                yield return null;
            }
        }

        public override void OnEpisodeBegin()
        {
            if (groundRenderer && cumulativeReward != 0f)
            { 
                Color flashColor = (cumulativeReward > 0f) ? Color.green : Color.red;

                if (_flashGroundCoroutine != null)
                {
                    StopCoroutine(_flashGroundCoroutine);                    
                }

                _flashGroundCoroutine = StartCoroutine(FlashGround(flashColor, 1.0f));
            }
            
            currentEpisode++;
            cumulativeReward = 0f;
        }
    }
}