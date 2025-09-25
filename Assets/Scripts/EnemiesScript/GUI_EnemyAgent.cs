using System.Globalization;
using AutoPlayerScript;
using EnemiesScript.Melee;
using Unity.MLAgents;
using UnityEngine;

namespace EnemiesScript
{
    public class GUIEnemyAgent : MonoBehaviour
    {
        [SerializeField] private AutoPlayerAgent autoPlayerAgent;

        private GUIStyle _defaultStyle = new GUIStyle();
        private GUIStyle _positiveStyle = new GUIStyle();
        private GUIStyle _negativeStyle = new GUIStyle();
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            //Define GUI styles
            _defaultStyle.fontSize = 40;
            _defaultStyle.normal.textColor = Color.yellow;
        
            _positiveStyle.fontSize = 40;
            _positiveStyle.normal.textColor = Color.green;
        
            _negativeStyle.fontSize = 40;
            _negativeStyle.normal.textColor = Color.red;
        }

        private void OnGUI()
        {
            string debugEpisode = "Episode: " + autoPlayerAgent.currentEpisode + " - Step: " + autoPlayerAgent.StepCount;
            string debugReward = "Reward: " + autoPlayerAgent.cumulativeReward.ToString(CultureInfo.InvariantCulture);
        
            // Select style based on reward value
            GUIStyle rewardStyle = autoPlayerAgent.cumulativeReward < 0f ? _negativeStyle : _positiveStyle;
        
            // Display the debug text
            GUI.Label(new Rect(20, 20, 500, 30), debugEpisode, _defaultStyle);
            GUI.Label(new Rect(20, 60, 500, 30), debugReward, rewardStyle);
        }

        // Update is called once per frame
        // void Update()
        // {
        //
        // }
    }
}
