using UnityEngine;

public class GUI_EnemyAgent : MonoBehaviour
{
    [SerializeField] private EnemyMeleeAgent _enemyMeleeAgent;

    private GUIStyle _defaultStyle = new GUIStyle();
    private GUIStyle _positiveStyle = new GUIStyle();
    private GUIStyle _negativeStyle = new GUIStyle();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
        string debugEpisode = "Episode: " + _enemyMeleeAgent.CurrentEpisode + " - Step: " + _enemyMeleeAgent.StepCount;
        string debugReward = "Reward: " + _enemyMeleeAgent.CumulativeReward.ToString();
        
        // Select style based on reward value
        GUIStyle rewardStyle = _enemyMeleeAgent.CumulativeReward < 0f ? _negativeStyle : _positiveStyle;
        
        // Display the debug text
        GUI.Label(new Rect(20, 20, 500, 30), debugEpisode, _defaultStyle);
        GUI.Label(new Rect(20, 60, 500, 30), debugReward, rewardStyle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
