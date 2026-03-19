using System.Globalization;
using AutoPlayerScript;
using UnityEngine;

public class GUIAutoPlayerAgent : MonoBehaviour
{
    [SerializeField] private AutoPlayerAgent autoPlayerAgent;

    private readonly GUIStyle _defaultStyle = new();
    private readonly GUIStyle _negativeStyle = new();
    private readonly GUIStyle _positiveStyle = new();

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
        var debugEpisode = "Episode: " + autoPlayerAgent.currentEpisode + " - Step: " + autoPlayerAgent.StepCount;
        var debugReward = "Reward: " + autoPlayerAgent.cumulativeReward.ToString(CultureInfo.InvariantCulture);

        // Select style based on reward value
        var rewardStyle = autoPlayerAgent.cumulativeReward < 0f ? _negativeStyle : _positiveStyle;

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